using UnityEngine;
using UnityEngine.UI;

public class PasswordDoorTrigger : MonoBehaviour
{
    [Header("═══ 密碼設定 ═══")]
    [Tooltip("正確的通關密碼")]
    public string correctPassword = "1234"; 

    [Header("═══ 拖拽目標：過關後要開啟的觸發區域 ═══")]
    [Tooltip("控制門打開的物理範圍 / 或者是固定鏡頭的 InteractArea")]
    public GameObject doorControlArea;
    [Tooltip("自由鏡頭切換的範圍 (也就是你的 SpecialAreaTrigger 紅綠燈格子)")]
    public GameObject freeCamSwitchArea;
    [Tooltip("額外的自由鏡頭控制格")]
    public GameObject extraFreeCamControlArea; 

    [Header("═══ UI 介面連動 ═══")]
    [Tooltip("密碼輸入介面的 Canvas 或 Panel 物件")]
    public GameObject passwordUIPanel;
    [Tooltip("密碼介面裡的輸入欄位 (InputField)")]
    public InputField passwordInputField;
    [Tooltip("密碼介面裡的錯誤提示文字")]
    public Text errorNotificationText;
    [Tooltip("密碼介面裡的確認按鈕 (Button)")]
    public Button submitButton;

    [Header("═══ 內部狀態 ═══")]
    private bool isPlayerInside = false;
    private bool isUIActive = false;
    private bool isDoorUnlocked = false;

    private void Start()
    {
        // 初始化 UI 狀態
        if (passwordUIPanel != null) passwordUIPanel.SetActive(false);
        if (errorNotificationText != null) errorNotificationText.text = "";
        
        // 初始狀態：所有控制格皆隱藏
        if (doorControlArea != null) doorControlArea.SetActive(false);
        if (freeCamSwitchArea != null) freeCamSwitchArea.SetActive(false);
        if (extraFreeCamControlArea != null) extraFreeCamControlArea.SetActive(false);

        if (submitButton != null)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(CheckPassword);
        }
    }

    private void Update()
    {
        if (isDoorUnlocked) return;

        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            TogglePasswordUI();
        }

        if (isUIActive && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            CheckPassword();
        }
    }

    private void TogglePasswordUI()
    {
        isUIActive = !isUIActive;
        
        if (passwordUIPanel != null)
        {
            passwordUIPanel.SetActive(isUIActive);
            
            if (isUIActive)
            {
                if (CameraManager.Instance != null) CameraManager.Instance.LockCameraSwitching(); 

                if (passwordInputField != null)
                {
                    passwordInputField.text = "";
                    passwordInputField.ActivateInputField(); 
                }
                if (errorNotificationText != null) errorNotificationText.text = "";
                
                if (UIManager.Instance != null) UIManager.Instance.ShowInteractPrompt("按 E 關閉介面");
                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                if (CameraManager.Instance != null) CameraManager.Instance.UnlockCameraSwitching();
                if (UIManager.Instance != null) UIManager.Instance.ShowInteractPrompt("Ｅ互動");
            }
        }
    }

    public void CheckPassword()
    {
        if (passwordInputField == null || !isUIActive) return;

        if (passwordInputField.text == correctPassword)
        {
            UnlockDoorSuccess();
        }
        else
        {
            if (errorNotificationText != null) errorNotificationText.text = "密碼錯誤，請重新輸入！";
            if (passwordInputField != null)
            {
                passwordInputField.text = "";
                passwordInputField.ActivateInputField(); 
            }
        }
    }

    private void UnlockDoorSuccess()
    {
        isDoorUnlocked = true;
        
        if (CameraManager.Instance != null) CameraManager.Instance.UnlockCameraSwitching(); 

        if (passwordUIPanel != null) passwordUIPanel.SetActive(false);
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideInteractPrompt();
            UIManager.Instance.ShowMessage("密碼已解開");
        }

        // 開啟所有控制區域
        if (doorControlArea != null) doorControlArea.SetActive(true);
        if (freeCamSwitchArea != null) freeCamSwitchArea.SetActive(true);
        if (extraFreeCamControlArea != null) extraFreeCamControlArea.SetActive(true); 
        
        // 清理自身
        Destroy(GetComponent<Collider>());
        Destroy(this, 0.5f); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDoorUnlocked) return;

        if (other.CompareTag("NPC") || other.GetComponent<FreeMoveCamera>() != null || other.GetComponent<Camera>() != null)
        {
            isPlayerInside = true;
            if (!isUIActive && UIManager.Instance != null)
            {
                UIManager.Instance.ShowInteractPrompt("Ｅ互動");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC") || other.GetComponent<FreeMoveCamera>() != null || other.GetComponent<Camera>() != null)
        {
            isPlayerInside = false;
            isUIActive = false;
            
            if (CameraManager.Instance != null) CameraManager.Instance.UnlockCameraSwitching();

            if (passwordUIPanel != null) passwordUIPanel.SetActive(false);
            
            if (UIManager.Instance != null) UIManager.Instance.HideInteractPrompt();
        }
    }
}