using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    [Header("═══ 背包 UI 引用 ═══")]
    public GameObject inventoryPanel;
    public Transform slotParent;
    public GameObject slotPrefab;

    [Header("═══ 道具清單 ═══")]
    public List<ItemData> items = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    // --- 查詢數量邏輯 ---
    public int GetItemCount(ItemData data)
    {
        if (data == null) return 0;
        
        int count = 0;
        foreach (ItemData item in items)
        {
            if (item == data) count++;
        }
        return count; 
    }

    // --- 扣除道具邏輯 ---
    public void RemoveItem(ItemData data, int amount = 1)
    {
        if (data == null) return;

        for (int i = 0; i < amount; i++)
        {
            if (items.Contains(data))
            {
                items.Remove(data);
            }
        }
        UpdateUI();
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

    if (data.isMissionItem)
    {
        UIManager.Instance.AddMissionProgress();

        // 顯示取得碎片提示
        UIManager.Instance.ShowCollectPanel();
    }

    UpdateUI();
}

    public void UpdateUI()
{
    if (slotParent == null || slotPrefab == null) return;

    foreach (Transform child in slotParent) Destroy(child.gameObject);

    foreach (ItemData item in items)
    {
        GameObject slot = Instantiate(slotPrefab, slotParent);
        
        Image iconImage = null;
        
        Transform iconTransform = slot.transform.Find("Icon"); 
        if (iconTransform == null) iconTransform = slot.transform.Find("Image");

        if (iconTransform != null)
        {
            iconImage = iconTransform.GetComponent<Image>();
        }
        else
        {
            foreach (Image img in slot.GetComponentsInChildren<Image>())
            {
                if (img.gameObject != slot)
                {
                    iconImage = img;
                    break;
                }
            }
        }

        if (iconImage != null) 
        {
            iconImage.sprite = item.itemIcon;
            iconImage.color = Color.white;
            iconImage.gameObject.SetActive(true);
        }

        // 綁定左鍵點擊事件
        ItemData itemData = item;
        Button btn = slot.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => UseItem(itemData));
        }

        // 👇 加入右鍵提示功能（新增）
        InventorySlotTooltip tooltip = slot.GetComponent<InventorySlotTooltip>();
        if (tooltip == null)
        {
            tooltip = slot.AddComponent<InventorySlotTooltip>();
        }
        tooltip.SetItem(itemData);
    }
}

    public void UseItem(ItemData item)
    {
        Debug.Log($"[UseItem] 點擊了: {item?.itemName}");

        if (item == null)
        {
            Debug.LogError("[UseItem] item 為 null");
            return;
        }

        // 不檢查視角，直接使用道具
        if (item.itemName == "木棍")
        {
            Debug.Log("[UseItem] 木棍被點擊");
            if (UIManager.Instance != null) UIManager.Instance.ShowMessage("點按使用");
        }
        else if (item.itemName.Contains("平面鏡") || item.itemName.Contains("凸面鏡"))
        {
            Debug.Log($"[UseItem] {item.itemName} 被點擊，生成物品");
            if (ItemSpawner.Instance != null)
            {
                ItemSpawner.Instance.SpawnItem(item);
                Debug.Log("[UseItem] ItemSpawner 已調用");
            }
            else
            {
                Debug.LogError("[UseItem] ItemSpawner.Instance 為 null");
            }
            CloseInventory();
        }
        else
        {
            Debug.Log($"[UseItem] {item.itemName} 被點擊（其他道具）");
            if (ItemSpawner.Instance != null)
            {
                ItemSpawner.Instance.SpawnItem(item);
                Debug.Log("[UseItem] ItemSpawner 已調用");
            }
            items.Remove(item);
            UpdateUI();
            CloseInventory();
        }
    }
    
}