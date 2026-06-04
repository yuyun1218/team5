using System.Collections;
using UnityEngine;
using UnityEngine.UI;          

[RequireComponent(typeof(InteractArea))]
public class ItemRequiredZone : MonoBehaviour
{
    [Header("--- 需求道具設定 ---")]
    public ItemData requiredItemData;      // 在 Inspector 拖入「木棍」的 ItemData

    [Header("--- UI 面板元件綁定 ---")]
    [Tooltip("普通的 UI Panel（放在 Canvas 底下，平常隱藏）")]
    public GameObject targetPanel;         
    [Tooltip("顯示需求道具圖片的 UI Image")]
    public Image itemIconImage;            
    [Tooltip("顯示進度的 UI Text（例如 0/1）")]
    public Text countText;                 
    [Tooltip("畫面 Panel 上的「使用道具」按鈕")]
    public Button useButton;               

    private InventorySystem inventory;
    private bool isPlayerInZone = false;
    private bool isPanelOpen = false;
    private bool isPuzzleSolved = false;   // 標記機關是否已解開

    void Start()
    {
        inventory = FindFirstObjectByType<InventorySystem>();
        
        if (targetPanel != null) targetPanel.SetActive(false);

        // 核心綁定：點擊畫面上的按鈕
        if (useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(OnUseButtonClick); 
        }

        // 初始化道具圖示
        if (itemIconImage != null && requiredItemData != null)
        {
            itemIconImage.sprite = requiredItemData.itemIcon; 
        }
    }

    void Update()
    {
        if (isPuzzleSolved) return;

        // 1. 只有在區域內時，才偵測 E 鍵來開關 UI 面板
        if (isPlayerInZone)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TogglePanel();
            }
        }

        // 2. 🌟【核心修正】只要面板當前是「開啟」的，就強制每幀更新數字與按鈕狀態！
        // 這樣在你撿到木棍的瞬間、或是點擊使用的瞬間，文字會立刻同步重新整理！
        if (targetPanel != null && targetPanel.activeSelf)
        {
            UpdateZoneUI();
        }
    }

    // 切換 Panel 的開關狀態（按 E 時觸發）
    void TogglePanel()
    {
        if (targetPanel == null) return;

        isPanelOpen = !isPanelOpen;
        targetPanel.SetActive(isPanelOpen);

        if (isPanelOpen)
        {
            UpdateZoneUI(); // 打開的瞬間先刷一次
            if (UIManager.Instance != null) UIManager.Instance.ShowInteractPrompt("使用木棍開啟機關");
        }
        else
        {
            if (UIManager.Instance != null) UIManager.Instance.ShowInteractPrompt("按E切換視角");
        }
    }

    // 更新 Panel 內部數據 [0/1] 或 [1/1]
    void UpdateZoneUI()
    {
        if (inventory == null || requiredItemData == null) return;

        // 從背包獲取即時的木棍持有數量
        int currentCount = inventory.GetItemCount(requiredItemData);
        
        // 🌟 確保格式百分之百為 "0/1" 或 "1/1"
        if (countText != null) 
        {
            countText.text = $"{currentCount}/1";
        }

        // 背包沒物品（0）時，按鈕直接變灰色（不可點擊）；有物品（>=1）時，按鈕立刻亮起（可點擊）
        if (useButton != null)
        {
            useButton.interactable = (currentCount > 0);
        }
    }

    // 滑鼠「真正點擊」畫面上的 Use Button 後觸發
    void OnUseButtonClick()
    {
        if (inventory == null || requiredItemData == null || isPuzzleSolved) return;

        int currentCount = inventory.GetItemCount(requiredItemData);
        if (currentCount <= 0) return; // 防呆

        // 1. 扣除背包木棍 1 個
        inventory.RemoveItem(requiredItemData, 1);
        
        // 2. 標記機關已完成，鎖定此區域
        isPuzzleSolved = true;

        // 3. 徹底關閉此區域的所有畫面 UI 面板與提示黑底
        ForceCloseUI();

        // 4. 正式對接 LeverSystem 啟動實體木棍與長按 F 邏輯
        if (LeverSystem.Instance != null)
        {
            LeverSystem.Instance.SetupLever(requiredItemData);
        }
        
        Debug.Log("<color=cyan>【使用成功】已扣除木棍，文字即時同步，關閉面板並啟動槓桿！</color>");
    }

    // 強制關閉所有 UI 的安全方法
    public void ForceCloseUI()
    {
        isPanelOpen = false;
        if (targetPanel != null) targetPanel.SetActive(false);
        if (UIManager.Instance != null) UIManager.Instance.HideInteractPrompt();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPuzzleSolved) return;

        if (other.CompareTag("NPC"))
        {
            isPlayerInZone = true;
            if (UIManager.Instance != null) 
            {
                UIManager.Instance.ShowInteractPrompt("按E切換視角");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerInZone = false;
            ForceCloseUI();
        }
    }
}