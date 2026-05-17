using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    // --- 核心修正：加入 Instance 定義 ---
    public static InventorySystem Instance;

    public GameObject inventoryPanel;
    public Transform slotParent;
    public GameObject slotPrefab;
    public List<ItemData> items = new List<ItemData>();

    private void Awake()
    {
        // --- 核心修正：初始化 Instance ---
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf) UpdateUI();
    }

    public void CloseInventory() => inventoryPanel.SetActive(false);

    public void AddItem(ItemData data)
    {
        if (data == null) return;
        items.Add(data);
    }

    public void UpdateUI()
    {
        if (slotParent == null || slotPrefab == null) return;

        foreach (Transform child in slotParent) Destroy(child.gameObject);

        foreach (ItemData item in items)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);
            Image icon = slot.GetComponentInChildren<Image>();
            if (icon != null) icon.sprite = item.itemIcon;

            slot.GetComponent<Button>().onClick.AddListener(() => UseItem(item));
        }
    }

    public void UseItem(ItemData item)
    {
        // 1. 限制：玩家僅能在 NPC 視角使用道具 (對接 CameraManager)
        if (CameraManager.Instance == null || !CameraManager.Instance.isNPCView)
        {
            if (UIManager.Instance != null) UIManager.Instance.ShowMessage("目前無法使用");
            return;
        }

        // 2. 限制：檢查是否在規定範圍 (例如木棍需在槓桿區域)
        if (!IsInRangeFor(item))
        {
            if (UIManager.Instance != null) UIManager.Instance.ShowMessage("目前無法使用");
            return;
        }

        // 3. 執行使用邏輯
        if (item.itemName == "木棍")
        {
            // 觸發木棍槓桿邏輯 (需搭配 LeverSystem)
            if (LeverSystem.Instance != null)
            {
                LeverSystem.Instance.SetupLever(item);
                items.Remove(item);
                UpdateUI();
                CloseInventory();
            }
            else
            {
                if (UIManager.Instance != null) UIManager.Instance.ShowMessage("目前無法使用");
            }
        }
        else if (ItemSpawner.Instance != null)
        {
            // 呼叫生成 (ItemSpawner 會自動回收舊道具，包含平面鏡邏輯)
            ItemSpawner.Instance.SpawnItem(item);
            
            // 從背包清單移除
            items.Remove(item);
            UpdateUI();
            CloseInventory();
        }
    }

    // 輔助方法：判斷是否在可使用的規定範圍
    private bool IsInRangeFor(ItemData item)
    {
        // 根據您的需求，NPC 視角僅在特殊區域切換
        // 因此只要 CameraManager.Instance.isNPCView 為 true，通常代表已在規定範圍
        // 若未來需要更精細判斷 (例如木棍只能在 A 洞口)，可在此擴充邏輯
        return CameraManager.Instance.isNPCView;
    }
}
