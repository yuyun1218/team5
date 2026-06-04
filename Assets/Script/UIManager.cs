using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("═══ 通知文字 UI（大通知保留） ═══")]
    public Text messageText;
    [Tooltip("畫面中央跳出大通知時，文字背後的黑底面板")]
    public GameObject messageBgPanel;

    [Header("═══ 面板（Panels）═══")]
    public GameObject startPanel;
    public GameObject deathPanel;
    public GameObject passPanel;
   
    [Header("═══ 按鈕（Buttons）═══")]
    public Button playButton;
    public Button retryButton;

    [Header("═══ 🌟 死亡選擇按鈕 ═══")]
    [Tooltip("死亡畫面中的『是 (繼續遊戲)』按鈕")]
    public Button deathYesButton;
    [Tooltip("死亡畫面中的『否 (結束遊戲)'] 按鈕")]
    public Button deathNoButton;

    [Header("═══ 🌟 重生、紀錄與攝影機設定 ═══")]
    
    [Header("═══ 2D 提示圖片（非監控視角） ═══")]
    [Header("═══ 特定攝影機（進入時關閉 2D Panel） ═══")]
public Camera[] camerasToHide2DPanel; // 拖入這些攝影機時會關閉 2D 提示 Panel
public GameObject hint2DPanel; // 在 Inspector 中拖入你的 2D 提示圖片 Panel
    [Tooltip("請拖入畫面中的 NPC 本體物件")]
    public GameObject npcObject;
    [Tooltip("請拖出場景中預先放好的重生點 (Empty Object)")]
    public Transform spawnPoint;
    [Tooltip("🌟 一號監視器（攝影機）。按 Yes 復活後會強制切換到這台攝影機")]
    public GameObject cameraOne;

    private AudioSource audioSource;
    private GameState currentGameState = GameState.Start;
    /// <summary>
/// 檢查攝影機是否在關閉列表中
/// </summary>
public bool ShouldHide2DPanel(Camera currentCamera)
{
    if (camerasToHide2DPanel == null || camerasToHide2DPanel.Length == 0) return false;
    
    foreach (Camera cam in camerasToHide2DPanel)
    {
        if (cam != null && currentCamera == cam)
        {
            Debug.Log("[UIManager] 攝影機在關閉列表中，隱藏 2D Panel");
            return true;
        }
    }
    return false;
}
    /// <summary>
/// 顯示 2D 提示圖片（供 CameraManager 調用）
/// </summary>
public void Show2DHintPanel()
{
    if (hint2DPanel != null) hint2DPanel.SetActive(true);
}

/// <summary>
/// 隱藏 2D 提示圖片（供 CameraManager 調用）
/// </summary>
public void Hide2DHintPanel()
{
    Debug.Log($"[UIManager] Hide2DHintPanel 被呼叫");
    Debug.Log($"[UIManager] hint2DPanel 是否為 null: {hint2DPanel == null}");
    Debug.Log($"[UIManager] hint2DPanel.activeSelf: {hint2DPanel?.activeSelf}");
    
    if (hint2DPanel != null)
    {
        hint2DPanel.SetActive(false);
        Debug.Log($"[UIManager] hint2DPanel 已設為 inactive");
    }
    else
    {
        Debug.LogError("[UIManager] hint2DPanel 為 null！請在 Inspector 中拖入 panel");
    }
}
    public enum GameState
    {
        Start,      
        Playing,    
        Death,      
        Pass,
        Respawn     
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeUI();
        InitializeButtons();
        InitializeAudio();

        FixMissingFont(messageText);

        // 🌟 核心修正：遊戲剛啟動時，狀態必須是 Start，這樣 Start Panel 才會顯示出來！
        SetGameState(GameState.Start);
    }

   private void InitializeUI()
{
    if (messageBgPanel != null) messageBgPanel.SetActive(false);
    else if (messageText != null) messageText.gameObject.SetActive(false);

    SetPanelActive(startPanel, false);
    SetPanelActive(deathPanel, false);
    SetPanelActive(passPanel, false);
    
    // 👇 加入这行
    SetPanelActive(hint2DPanel, false);
}
    // ═══════════════════════════════════════════════
    // 🌟 真正實作：對接密碼門與通用機關的提示文字
    // ═══════════════════════════════════════════════
    
    /// <summary>
    /// 開啟並顯示互動提示文字（例如：『Ｅ互動』）
    /// </summary>
   public void ShowInteractPrompt(string text, float duration = 2.0f) 
    {
        if (messageText == null) return;
        
        // 1. 停止之前的提示協程，確保不會互相干擾
        StopAllCoroutines(); 
        
        // 2. 顯示 UI
        messageText.text = text;
        if (messageBgPanel != null) messageBgPanel.SetActive(true);
        messageText.gameObject.SetActive(true);

        // 3. 啟動自動關閉的協程
        StartCoroutine(HidePromptRoutine(duration));
    }
    private IEnumerator HidePromptRoutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        HideInteractPrompt();
    }

    /// <summary>
    /// 關閉互動提示文字
    /// </summary>
    public void HideInteractPrompt() 
    {
        if (messageBgPanel != null) messageBgPanel.SetActive(false);
        else if (messageText != null) messageText.gameObject.SetActive(false);
    }

    // ═══════════════════════════════════════════════
    // 🌟 遊戲狀態控制
    // ═══════════════════════════════════════════════
    public void SetGameState(GameState newState)
    {
        currentGameState = newState;

        switch (newState)
        {
            case GameState.Start:
                Time.timeScale = 1f;
                SetPanelActive(startPanel, true);
                SetPanelActive(deathPanel, false);
                SetPanelActive(passPanel, false);
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                SetPanelActive(startPanel, false);
                SetPanelActive(deathPanel, false);
                SetPanelActive(passPanel, false);
                break;

            case GameState.Death:
                SetPanelActive(startPanel, false);
                SetPanelActive(deathPanel, true);
                SetPanelActive(passPanel, false);
                ShowMessage("遊戲失敗！");
                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
                Time.timeScale = 0f; 
                break;

            case GameState.Pass:
                Time.timeScale = 0f;
                SetPanelActive(startPanel, false);
                SetPanelActive(deathPanel, false);
                SetPanelActive(passPanel, true);
                ShowMessage("過關成功！");
                break;

            case GameState.Respawn:
                Time.timeScale = 1f;
                SetPanelActive(startPanel, false);
                SetPanelActive(deathPanel, false);
                SetPanelActive(passPanel, false);
                break;
        }
    }

    public GameState GetGameState() => currentGameState;

    /// <summary>
    /// 常規大通知方法（帶有 2 秒自動關閉功能）
    /// </summary>
    public void ShowMessage(string text)
    {
        if (messageText == null) return;
        StopAllCoroutines();
        StartCoroutine(MessageRoutine(text));
    }

    private IEnumerator MessageRoutine(string text)
    {
        messageText.text = text;
        if (messageBgPanel != null) messageBgPanel.SetActive(true);
        messageText.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        if (messageBgPanel != null) messageBgPanel.SetActive(false);
        else if (messageText != null) messageText.gameObject.SetActive(false);
    }

    private void InitializeButtons()
    {
        if (playButton != null) playButton.onClick.AddListener(OnPlayButtonClicked);
        if (retryButton != null) retryButton.onClick.AddListener(OnRetryButtonClicked);

        if (deathYesButton != null)
        {
            deathYesButton.onClick.RemoveAllListeners();
            deathYesButton.onClick.AddListener(OnDeathYesClicked);
        }
        if (deathNoButton != null)
        {
            deathNoButton.onClick.RemoveAllListeners();
            deathNoButton.onClick.AddListener(OnDeathNoClicked);
        }
    }

    private void OnPlayButtonClicked()
    {
        TriggerButtonAnimation(playButton);
        SetGameState(GameState.Playing);
    }

    private void OnRetryButtonClicked()
    {
        TriggerButtonAnimation(retryButton);
        StartCoroutine(ReloadSceneWithDelay());
    }

    private void OnDeathYesClicked()
    {
        TriggerButtonAnimation(deathYesButton);
        Debug.Log("[UIManager] 點擊【是】，啟動不重置無縫傳送...");

        Time.timeScale = 1f;
        SetGameState(GameState.Respawn);

        if (npcObject != null && spawnPoint != null)
        {
            CharacterController cc = npcObject.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false; 

            npcObject.transform.position = spawnPoint.position;
            npcObject.transform.rotation = spawnPoint.rotation;

            if (cc != null) cc.enabled = true;  
            Debug.Log($"[UIManager] NPC 座標已成功移至：{spawnPoint.position}");
        }

        if (cameraOne != null)
        {
            cameraOne.SetActive(true);
        }

        StartCoroutine(SafeReturnToPlaying());
    }

    private IEnumerator SafeReturnToPlaying()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        if (currentGameState == GameState.Respawn)
        {
            currentGameState = GameState.Playing;
            Debug.Log("[UIManager] 復活程序安全完成，遊戲繼續。");
        }
    }

    private void OnDeathNoClicked()
    {
        TriggerButtonAnimation(deathNoButton);
        Debug.Log("[UIManager] 點擊【否】，正在關閉程式...");

        Time.timeScale = 1f;

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    private void InitializeAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void FixMissingFont(Text t)
    {
        if (t != null && t.font == null) t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private void TriggerButtonAnimation(Button button)
    {
        if (button == null) return;
        StartCoroutine(ButtonClickAnimation(button.GetComponent<RectTransform>()));
    }

    private IEnumerator ButtonClickAnimation(RectTransform buttonRect)
    {
        if (buttonRect == null) yield break;
        Vector3 originalScale = buttonRect.localScale;
        float duration = 0.2f; 
        float elapsedTime = 0;
        
        while (elapsedTime < duration / 2) 
        { 
            elapsedTime += Time.unscaledDeltaTime; 
            buttonRect.localScale = Vector3.Lerp(originalScale, originalScale * 0.9f, elapsedTime / (duration / 2)); 
            yield return null; 
        }
        elapsedTime = 0;
        while (elapsedTime < duration / 2) 
        { 
            elapsedTime += Time.unscaledDeltaTime; 
            buttonRect.localScale = Vector3.Lerp(originalScale * 0.9f, originalScale, elapsedTime / (duration / 2)); 
            yield return null; 
        }
        buttonRect.localScale = originalScale;
    }

    private IEnumerator ReloadSceneWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SetPanelActive(GameObject panel, bool active) 
    { 
        if (panel != null) panel.SetActive(active); 
    }
}