using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [Header("UI 引用")]
    public GameObject planeMirrorBtn; // 拖入背包內對應的 Button 物件

    // 道具狀態
    public bool hasPlaneMirror = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 初始狀態：玩家還沒撿到鏡子，按鈕隱藏
        if (planeMirrorBtn != null) planeMirrorBtn.SetActive(false);
    }

    public void AddItem(string type)
    {
        if (type == "Plane")
        {
            hasPlaneMirror = true;
            if (planeMirrorBtn != null) planeMirrorBtn.SetActive(true); // 撿到後按鈕顯示
            Debug.Log("系統：已獲得平面鏡，背包按鈕已開啟");
        }
    }
}