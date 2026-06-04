using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public enum CameraState
    {
        MonitorView,        // 監控視角 (預設)
        FixedNPCView,       // 固定 NPC 特寫視角
        FreeMoveView        // 自由移動攝影機視角
    }

    [Header("═══ 初始鏡頭設定 ═══")]
    public int defaultActiveMonitorNumber = 2;

    [Header("═══ 監控攝影機設定 ═══")]
    public Camera[] monitorCameras;
    public Text messageText;
    public GameObject messageBgPanel;

    [Header("═══ 🌟 鏡頭分類管理 (新增空格) ═══")]
    [Tooltip("請將場景中所有的『固定視角攝影機』拖到這裡")]
    public Camera[] allFixedNPCCameras; 
    [Tooltip("請將場景中所有的『自由移動攝影機』拖到這裡")]
    public GameObject[] allFreeMoveCameras;

    [Header("═══ UI 指示器 ═══")]
    [Header("═══ 2D 提示圖片（非監控視角） ═══")]
public GameObject hint2DPanel; // 在 Inspector 中拖入你的 2D 提示圖片 Panel
    public Image[] cameraIndicators;
    public Sprite[] graySprites;
    public Sprite[] colorSprites;
    public GameObject indicatorPanel;

    [Header("═══ 狀態檢視 ═══")]
    public CameraState currentCameraState = CameraState.MonitorView; 

    [Header("═══ NPC 渲染控制 ═══")]
    public GameObject npcGameObject;

    // 相容性屬性
    public bool isNPCView
    {
        get { return currentCameraState == CameraState.FixedNPCView || currentCameraState == CameraState.FreeMoveView; }
        set { if (!value) ReturnToMonitorView(); }
    }

    private bool isSwitchingLocked = false;
    private int currentMonitorIndex = 0;
    private InteractArea currentInteractArea; 
    private SpecialAreaTrigger currentSpecialAreaTrigger; 
    private Camera activeCamera = null; 
    private FreeMoveCamera cachedFreeMoveCamScript = null; 

    private float lastToggleTime = -1f;
    private const float TOGGLE_COOLDOWN = 0.3f;
    private bool isDisplayingTimedMessage = false;

    public Camera currentCamera
    {
        get
        {
            if (currentCameraState == CameraState.MonitorView && monitorCameras.Length > 0)
                return monitorCameras[currentMonitorIndex];
            return activeCamera;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
{
    foreach (var cam in monitorCameras) if (cam != null) cam.enabled = false;
    currentMonitorIndex = defaultActiveMonitorNumber - 1;
    if (currentMonitorIndex < 0 || currentMonitorIndex >= monitorCameras.Length) currentMonitorIndex = 0;
    if (monitorCameras.Length > 0 && monitorCameras[currentMonitorIndex] != null)
    {
        monitorCameras[currentMonitorIndex].enabled = true;
        activeCamera = monitorCameras[currentMonitorIndex];
        currentCameraState = CameraState.MonitorView;
    }
    if (messageBgPanel != null) messageBgPanel.SetActive(false);
    UpdateIndicators();
    SetNPCRendererVisible(true);
    
    // 👇 加入这行
    if (UIManager.Instance != null) UIManager.Instance.Hide2DHintPanel();
}
    private void Update()
{
    if (currentCameraState == CameraState.MonitorView && !isSwitchingLocked)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("[CameraManager] 按下 1");
            SwitchMonitor(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("[CameraManager] 按下 2");
            SwitchMonitor(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("[CameraManager] 按下 3");
            SwitchMonitor(2);
        }
    }

    if (Input.GetKeyDown(KeyCode.E) && !isSwitchingLocked)
    {
        if (Time.time - lastToggleTime >= TOGGLE_COOLDOWN)
        {
            ToggleCameraView();
            lastToggleTime = Time.time;
        }
    }
}

    public void ToggleCameraView()
    {
        if (isSwitchingLocked) return;

        switch (currentCameraState)
        {
            case CameraState.MonitorView:
                if (currentInteractArea != null) SwitchToFixedNPCView(currentInteractArea);
                else if (currentSpecialAreaTrigger != null) SwitchToFreeMoveView(currentSpecialAreaTrigger);
                break;

            case CameraState.FixedNPCView:
                // 🌟 核心修改：如果是從自由鏡頭進來的，則返回自由鏡頭
                if (cachedFreeMoveCamScript != null)
                {
                    if (activeCamera != null) activeCamera.enabled = false;
                    activeCamera = cachedFreeMoveCamScript.GetComponent<Camera>();
                    activeCamera.enabled = true;
                    currentCameraState = CameraState.FreeMoveView;
                    cachedFreeMoveCamScript.WakeUpFromFixedView();
                }
                else
                {
                    ReturnToMonitorView();
                }
                break;

            case CameraState.FreeMoveView:
                if (cachedFreeMoveCamScript != null)
                {
                    // 1. 優先檢查是否要進入固定特寫
                    InteractArea targetArea = cachedFreeMoveCamScript.GetTargetInteractArea();
                    if (targetArea != null)
                    {
                        SwitchToFixedNPCView(targetArea);
                        cachedFreeMoveCamScript.LockIntoFixedView();
                        return;
                    }

                    // 2. 🌟 核心修改：只有在出口區域才允許切換回監控
                    if (cachedFreeMoveCamScript.GetIsInExitZone())
                    {
                        ExitFreeMoveCameraView();
                    }
                    else
                    {
                        Debug.Log("[CameraManager] 未在出口區域，禁止切換回監控。");
                    }
                }
                break;
        }
    }

    public void SwitchToFixedNPCView(InteractArea interactArea)
{
    if (interactArea == null || interactArea.fixedCamera == null) return;
    if (activeCamera != null) activeCamera.enabled = false;
    activeCamera = interactArea.fixedCamera;
    activeCamera.enabled = true;
    currentCameraState = CameraState.FixedNPCView;
    currentInteractArea = interactArea;
    SetNPCRendererVisible(false);
    if (indicatorPanel != null) indicatorPanel.SetActive(false);
    HideInteractPrompt();
    
    // 👇 呼叫 UIManager 顯示 2D 提示
    if (UIManager.Instance != null) UIManager.Instance.Show2DHintPanel();
}

   public void SwitchToFreeMoveView(SpecialAreaTrigger trigger)
{
    if (trigger == null || trigger.freeMoveCamera == null) return;
    if (activeCamera != null) activeCamera.enabled = false;
    activeCamera = trigger.freeMoveCamera.GetComponent<Camera>();
    activeCamera.enabled = true;
    currentCameraState = CameraState.FreeMoveView;
    currentSpecialAreaTrigger = trigger;
    cachedFreeMoveCamScript = activeCamera.GetComponent<FreeMoveCamera>();
    if (cachedFreeMoveCamScript != null) 
    {
        cachedFreeMoveCamScript.enabled = true; 
        cachedFreeMoveCamScript.WakeUpFromFixedView();
    }
    SetNPCRendererVisible(false);
    if (indicatorPanel != null) indicatorPanel.SetActive(false);
    HideInteractPrompt();
    
    // 👇 呼叫 UIManager 顯示 2D 提示
    if (UIManager.Instance != null) UIManager.Instance.Show2DHintPanel();
}

    public void ReturnToMonitorView()
{
    if (activeCamera != null) activeCamera.enabled = false;
    if (cachedFreeMoveCamScript != null)
    {
        cachedFreeMoveCamScript.enabled = false;
        cachedFreeMoveCamScript.LockIntoFixedView();
        cachedFreeMoveCamScript = null;
    }
    if (monitorCameras.Length > 0 && monitorCameras[currentMonitorIndex] != null)
    {
        monitorCameras[currentMonitorIndex].enabled = true;
        activeCamera = monitorCameras[currentMonitorIndex];
    }
    currentCameraState = CameraState.MonitorView;
    SetNPCRendererVisible(true);
    if (indicatorPanel != null) indicatorPanel.SetActive(true);
    UpdateIndicators();
    if (currentInteractArea != null) SetInteractArea(currentInteractArea);
    
    // 👇 呼叫 UIManager 隱藏 2D 提示
    if (UIManager.Instance != null) UIManager.Instance.Hide2DHintPanel();
}

    public void ExitFreeMoveCameraView() { ReturnToMonitorView(); }
    public void LockCameraSwitching() { isSwitchingLocked = true; }
    public void UnlockCameraSwitching() { isSwitchingLocked = false; }
    public void ToggleNPCView() { ToggleCameraView(); }

    public void SetInteractArea(InteractArea area)
    {
        currentInteractArea = area;
        if (messageText != null && area != null && currentCameraState == CameraState.MonitorView)
        {
            if (messageBgPanel != null) messageBgPanel.SetActive(true);
            messageText.gameObject.SetActive(true);
            messageText.text = area.isFreeMoveCameraEntrance ? "按 E 進入自由視角" : "按 E 進入視角";
        }
        else HideInteractPrompt();
    }

    public void SetSpecialAreaTrigger(SpecialAreaTrigger trigger)
    {
        currentSpecialAreaTrigger = trigger;
        if (messageText != null && trigger != null && currentCameraState == CameraState.MonitorView)
        {
            if (messageBgPanel != null) messageBgPanel.SetActive(true);
            messageText.gameObject.SetActive(true);
            messageText.text = "按 E 進入自由視角";
        }
    }

  public void SwitchMonitor(int index)
{
    if (isSwitchingLocked || currentCameraState != CameraState.MonitorView) return;
    if (index < 0 || index >= monitorCameras.Length) return;
    if (activeCamera != null) activeCamera.enabled = false;
    currentMonitorIndex = index;
    monitorCameras[currentMonitorIndex].enabled = true;
    activeCamera = monitorCameras[currentMonitorIndex];
    UpdateIndicators();
    
    Debug.Log($"[CameraManager] 切換到攝影機: {activeCamera.name}");
    
    // 調用 UIManager 檢查
    if (UIManager.Instance != null)
    {
        Debug.Log("[CameraManager] UIManager 存在");
        bool shouldHide = UIManager.Instance.ShouldHide2DPanel(activeCamera);
        Debug.Log($"[CameraManager] ShouldHide2DPanel 返回: {shouldHide}");
        
        if (shouldHide)
        {
            UIManager.Instance.Hide2DHintPanel();
            Debug.Log("[CameraManager] 已呼叫 Hide2DHintPanel");
        }
    }
    else
    {
        Debug.LogError("[CameraManager] UIManager.Instance 為 null");
    }
}

    private void SetNPCRendererVisible(bool visible)
    {
        if (npcGameObject == null) return;
        Renderer[] renderers = npcGameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer r in renderers) r.enabled = visible;
    }

    public void HideInteractPrompt()
    {
        if (messageBgPanel != null) messageBgPanel.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(false);
    }

    private void UpdateIndicators()
    {
        if (cameraIndicators == null) return;
        for (int i = 0; i < cameraIndicators.Length; i++)
        {
            if (cameraIndicators[i] != null)
                cameraIndicators[i].sprite = (i == currentMonitorIndex) ? colorSprites[i] : graySprites[i];
        }
    }
}
