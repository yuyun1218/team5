using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public GameObject prefab;
    public string itemDescription = "道具描述"; // 👈 加這行
    public bool isMissionItem;   //是否為任務道具
}