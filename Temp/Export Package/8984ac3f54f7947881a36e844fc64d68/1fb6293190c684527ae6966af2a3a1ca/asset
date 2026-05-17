using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public Camera[] monitorCameras;
    public Text promptText;

    public bool isNPCView = false;
    private int currentMonitorIndex = 0;
    private InteractArea currentInteractArea;
    private Camera npcViewCamera = null;
    
    // ★ 添加冷却时间
    private float lastToggleTime = -1f;
    private const float TOGGLE_COOLDOWN = 0.3f;

    public Camera currentCamera => isNPCView ? npcViewCamera : monitorCameras[currentMonitorIndex];

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        foreach (var cam in monitorCameras)
            if (cam != null) cam.enabled = false;

        if (monitorCameras.Length > 0) 
            monitorCameras[0].enabled = true;
        
        if (promptText != null) promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // ★ 注意：不在 NPC 视角时才能用数字键切换监控
        if (!isNPCView)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchMonitor(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchMonitor(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchMonitor(2);
        }
        
        // ★ E 键：使用 GetKeyDown（按下时）而不是 GetKey（持续按住）
        if (Input.GetKeyDown(KeyCode.E))
        {
            // ★ 检查冷却时间
            if (Time.time - lastToggleTime >= TOGGLE_COOLDOWN)
            {
                ToggleNPCView();
                lastToggleTime = Time.time;
            }
        }
    }

    public void SwitchMonitor(int index)
    {
        if (isNPCView || index < 0 || index >= monitorCameras.Length) return;
        
        monitorCameras[currentMonitorIndex].enabled = false;
        currentMonitorIndex = index;
        monitorCameras[currentMonitorIndex].enabled = true;
    }

    public void SwitchToNPCView()
    {
        if (currentInteractArea == null)
        {
            Debug.LogError("[CameraManager] currentInteractArea 为 null");
            return;
        }

        if (currentInteractArea.fixedCamera == null)
        {
            Debug.LogError($"[CameraManager] {currentInteractArea.gameObject.name} 的 fixedCamera 为 null");
            return;
        }

        // 关闭所有监控
        foreach (var cam in monitorCameras)
            if (cam != null) cam.enabled = false;

        // 关闭旧的 NPC 摄影机
        if (npcViewCamera != null && npcViewCamera != currentInteractArea.fixedCamera)
            npcViewCamera.enabled = false;

        // 开启新的 NPC 摄影机
        npcViewCamera = currentInteractArea.fixedCamera;
        npcViewCamera.enabled = true;
        isNPCView = true;

        if (promptText != null) promptText.text = "按 E 返回监控视角";
        
        Debug.Log($"[CameraManager] 切换到 NPC 视角：{npcViewCamera.name}");
    }

    public void ReturnToMonitorView()
    {
        if (npcViewCamera != null) npcViewCamera.enabled = false;
        
        monitorCameras[currentMonitorIndex].enabled = true;
        isNPCView = false;

        if (promptText != null)
            promptText.text = currentInteractArea != null ? "按 E 进入视角" : "";
        
        Debug.Log($"[CameraManager] 返回监控视角 {currentMonitorIndex}");
    }

    public void ToggleNPCView()
    {
        if (isNPCView) 
            ReturnToMonitorView();
        else if (currentInteractArea != null) 
            SwitchToNPCView();
    }

    public void SetInteractArea(InteractArea area)
    {
        if (currentInteractArea == area) return;
        
        currentInteractArea = area;

        if (promptText != null)
        {
            if (area != null)
            {
                promptText.gameObject.SetActive(true);
                promptText.text = "按 E 进入视角";
            }
            else
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }
}