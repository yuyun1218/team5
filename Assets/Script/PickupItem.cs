using UnityEngine;

public class PickupItem : MonoBehaviour
{
    // 🌟 定義這三個你需要的專屬道具類型
    public enum ItemType
    {
        WoodStick,         // 木棍
        ConvexLens,        // 凸面鏡
        FlatMirrorFragment // 平面鏡碎片
    }

    [Header("--- 🌟 道具類型設定 ---")]
    [Tooltip("直接在下拉選單選擇這個物件是哪一個道具")]
    public ItemType currentItemType;

    [Header("--- 物品背包資料（保留原本系統） ---")]
    public ItemData itemData; 
    
    private bool isPlayerNear = false;

    private void Update()
    {
        // 當 NPC 在範圍內且按下 F 鍵
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            var inv = FindFirstObjectByType<InventorySystem>();
            if (inv != null)
            {
                // 1. 執行你原本的背包系統加入邏輯
                if (itemData != null)
                {
                    inv.AddItem(itemData); 
                }
                
                // 2. 🌟 根據你選的類型，直接在程式碼中抓取寫好的中文名稱
                string chineseName = GetItemChineseName(currentItemType);

                Debug.Log($"<color=cyan>【拾取】成功獲得物品：{chineseName}</color>");

                // 3. 🌟 呼叫 UIManager 彈出大通知（完美保留 2 秒，不會被按鍵秒關）
                if (UIManager.Instance != null) 
                {
                    UIManager.Instance.ShowMessage($"獲得{chineseName}");
                }
                
                // 4. 銷毀場景實體
                Destroy(gameObject); 
            }
        }
    }

    /// <summary>
    /// 🌟 這裡直接幫你把道具的中文名稱死死綁定在程式碼中
    /// </summary>
    private string GetItemChineseName(ItemType type)
    {
        switch (type)
        {
            case ItemType.WoodStick:
                return "木棍";
            case ItemType.ConvexLens:
                return "凸面鏡";
            case ItemType.FlatMirrorFragment:
                return "平面鏡碎片";
            default:
                return "未知道具";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerNear = true;
            // 靠近時保持純淨，完全不跳出任何 ShowInteractPrompt 提示字
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerNear = false;
            // 離開時也完全不干擾其他機關的提示字
        }
    }
}