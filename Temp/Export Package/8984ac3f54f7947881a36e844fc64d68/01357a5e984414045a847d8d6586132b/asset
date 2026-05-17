using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("═══ 文字 UI ═══")]
    public Text promptText;
    public Text messageText;

    [Header("═══ 面板（Panels）═══")]
    public GameObject startPanel;
    public GameObject deathPanel;
    public GameObject passPanel;
   

    [Header("═══ 按鈕（Buttons）═══")]
    public Button playButton;
    public Button retryButton;
   
    

   
    

    
    private AudioSource audioSource;
    private GameState currentGameState = GameState.Start;

    // 遊戲狀態枚舉
    public enum GameState
    {
        Start,      // 遊戲開始畫面
        Playing,    // 遊戲進行中
        Death,      // 死亡畫面
        Pass        // 過關畫面
    }

    private void Awake()
    {
        // 單例模式
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
        // 初始化
        InitializeUI();
        InitializeButtons();
        InitializeAudio();

        // 自動修復字體
        FixMissingFont(promptText);
        FixMissingFont(messageText);

        // 進入遊戲開始狀態
        SetGameState(GameState.Start);
    }

    // ═══════════════════════════════════════════════
    // 初始化方法
    // ═══════════════════════════════════════════════

    private void InitializeUI()
    {
        // 隱藏所有文字 UI
        if (promptText != null) promptText.gameObject.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(false);

        // 初始化所有面板
        SetPanelActive(startPanel, true);
        
        SetPanelActive(deathPanel, false);
        SetPanelActive(passPanel, false);

        Debug.Log("[UIManager] UI 初始化完成");
    }

    private void InitializeButtons()
    {
        // 綁定按鈕事件
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryButtonClicked);

       

       
        Debug.Log("[UIManager] 按鈕綁定完成");
    }

    private void InitializeAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void FixMissingFont(Text t)
    {
        if (t != null && t.font == null)
        {
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (t.font == null)
            {
                Debug.LogError($"[UIManager] {t.gameObject.name} 缺少字體！");
            }
        }
    }

    // ═══════════════════════════════════════════════
    // 遊戲狀態管理
    // ═══════════════════════════════════════════════

    public void SetGameState(GameState newState)
    {
        if (currentGameState == newState) return;

        currentGameState = newState;
        Debug.Log($"[UIManager] 遊戲狀態變更: {newState}");

        switch (newState)
        {
            case GameState.Start:
                ShowStartPanel();
                break;

            case GameState.Playing:
                ShowGamePanel();
                break;

            case GameState.Death:
                ShowDeathPanel();
                break;

            case GameState.Pass:
                ShowPassPanel();
                break;
        }
    }

    public GameState GetGameState()
    {
        return currentGameState;
    }

    // ═══════════════════════════════════════════════
    // 面板顯示方法
    // ═══════════════════════════════════════════════

    public void ShowStartPanel()
    {
        SetPanelActive(startPanel, true);
        
        SetPanelActive(deathPanel, false);
        SetPanelActive(passPanel, false);

       

        Debug.Log("[UIManager] 顯示開始面板");
    }

    public void ShowGamePanel()
    {
        SetPanelActive(startPanel, false);
        
        SetPanelActive(deathPanel, false);
        SetPanelActive(passPanel, false);

       

        Debug.Log("[UIManager] 顯示遊戲面板");
    }

    public void ShowDeathPanel()
    {
        SetPanelActive(startPanel, false);
        
        SetPanelActive(deathPanel, true);
        SetPanelActive(passPanel, false);

       
        ShowMessage("遊戲失敗！");

        Debug.Log("[UIManager] 顯示死亡面板");
    }

    public void ShowPassPanel()
    {
        SetPanelActive(startPanel, false);
        
        SetPanelActive(deathPanel, false);
        SetPanelActive(passPanel, true);

       
        ShowMessage("過關成功！");

        Debug.Log("[UIManager] 顯示過關面板");
    }

    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    // ═══════════════════════════════════════════════
    // 按鈕事件
    // ═══════════════════════════════════════════════

    private void OnPlayButtonClicked()
    {
        Debug.Log("[UIManager] Play 按鈕被點擊");
       

        // 觸發按鈕動畫
        TriggerButtonAnimation(playButton);

        // 開始遊戲
        SetGameState(GameState.Playing);
    }

    private void OnRetryButtonClicked()
    {
        Debug.Log("[UIManager] Retry 按鈕被點擊");
        
        // 觸發按鈕動畫
        TriggerButtonAnimation(retryButton);

        // 重新加載場景
        StartCoroutine(ReloadSceneWithDelay());
    }

    private void OnPassButtonClicked()
    {
        Debug.Log("[UIManager] Pass 按鈕被點擊");
        
    

        // 進入下一關（需要根據您的邏輯修改）
        StartCoroutine(LoadNextLevelWithDelay());
    }

    private void OnMenuButtonClicked()
    {
        Debug.Log("[UIManager] Menu 按鈕被點擊");
        

       

        // 返回菜單
        SetGameState(GameState.Start);
    }

    // ═══════════════════════════════════════════════
    // 文字提示方法
    // ═══════════════════════════════════════════════

    public void ShowInteractPrompt(string text)
    {
        if (promptText == null) return;
        promptText.text = text;
        promptText.gameObject.SetActive(true);
    }

    public void HideInteractPrompt()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    public void ShowMessage(string text)
    {
        if (messageText == null) return;
        StopAllCoroutines();
        StartCoroutine(MessageRoutine(text));
    }

    private IEnumerator MessageRoutine(string text)
    {
        messageText.text = text;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        messageText.gameObject.SetActive(false);
    }

    // ═══════════════════════════════════════════════
    // 按鈕動畫
    // ═══════════════════════════════════════════════

    private void TriggerButtonAnimation(Button button)
    {
        if (button == null) return;

        // 使用簡單的縮放動畫，不依賴 BounceScale 腳本
        StartCoroutine(ButtonClickAnimation(button.GetComponent<RectTransform>()));
    }

    private IEnumerator ButtonClickAnimation(RectTransform buttonRect)
    {
        if (buttonRect == null) yield break;

        Vector3 originalScale = buttonRect.localScale;
        float duration = 0.2f;
        float elapsedTime = 0;

        // 縮小
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (duration / 2);
            buttonRect.localScale = Vector3.Lerp(originalScale, originalScale * 0.9f, progress);
            yield return null;
        }

        elapsedTime = 0;

        // 放大回原始大小
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (duration / 2);
            buttonRect.localScale = Vector3.Lerp(originalScale * 0.9f, originalScale, progress);
            yield return null;
        }

        buttonRect.localScale = originalScale;
    }

    // ═══════════════════════════════════════════════
    // 音效播放
    // ═══════════════════════════════════════════════

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    // ═══════════════════════════════════════════════
    // 場景加載方法
    // ═══════════════════════════════════════════════

    private IEnumerator ReloadSceneWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    private IEnumerator LoadNextLevelWithDelay()
    {
        yield return new WaitForSeconds(0.5f);

        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex + 1);
    }

    // ═══════════════════════════════════════════════
    // 調試方法
    // ═══════════════════════════════════════════════

    public void DebugPrintState()
    {
        Debug.Log($"[UIManager] 當前狀態: {currentGameState}");
        Debug.Log($"[UIManager] 開始面板: {(startPanel != null ? startPanel.activeSelf : "未設置")}");
       
        Debug.Log($"[UIManager] 死亡面板: {(deathPanel != null ? deathPanel.activeSelf : "未設置")}");
        Debug.Log($"[UIManager] 過關面板: {(passPanel != null ? passPanel.activeSelf : "未設置")}");
    }
}