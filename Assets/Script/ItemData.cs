using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon; // 道具在背包顯示的圖片
    public GameObject prefab; // 使用時生成的模型
}
