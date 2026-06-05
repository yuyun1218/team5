using UnityEngine;
using UnityEngine.SceneManagement; // 控制場景切換
using UnityEngine.UI;           // 控制 UI 按鈕

public class DeathMenuManager : MonoBehaviour
{
    [Header("═══ UI 面板物件 ═══")]
    [Tooltip("整個死亡畫面 UI 的母物件面板 (Canvas底下的Panel)，一開始設為隱藏")]
    public GameObject deathScreenPanel;

    [Header("═══ 重生與紀錄設定 ═══")]
    [Tooltip("請拖入畫面中的 NPC 本體物件")]
    public GameObject npcObject;
    [Tooltip("請拖出場景中預先放好的重生點 (Empty Object)")]
    public Transform spawnPoint;

    [Header("═══ 主選單場景名稱 ═══")]
    [Tooltip("點擊『否』時，要跳轉的遊戲開始介面場景名稱")]
    [Header("═══ 死亡原因提示 ═══")]
public GameObject deathReasonPanel; // 這是你要顯示的圖片面板
    public string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        // 遊戲一開始，確保死亡畫面是關閉的，遊戲時間正常
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(false);
        }
        Time.timeScale = 1f; 
    }

    /// <summary>
    /// 被 DeathTrigger 呼叫：顯示死亡畫面並暫停遊戲
    /// </summary>
    // 1. 呼叫此方法顯示圖片提示
public void ShowDeathReasonHint()
{
    if (deathReasonPanel != null)
    {
        deathReasonPanel.SetActive(true); // 顯示圖片
        Time.timeScale = 0f;              // 暫停
        Cursor.visible = true;
    }
}

// 2. 圖片面板上的按鈕執行此方法，點擊後跳到真正的死亡介面
public void CloseHintAndShowDeathMenu()
{
    if (deathReasonPanel != null) deathReasonPanel.SetActive(false);
    
    // 直接呼叫原本的死亡介面邏輯
    if (deathScreenPanel != null) deathScreenPanel.SetActive(true);
}

    /// <summary>
    /// 🌟 按鈕按了【是】：繼續遊戲，回到重生點並繼承紀錄
    /// </summary>
    public void OnClickRestart()
    {
        Debug.Log("[死亡系統] 玩家選擇【是】，NPC 回到重生點並繼承現有紀錄。");

        // 1. 恢復遊戲時間
        Time.timeScale = 1f;

        // 2. 將 NPC 移動到重生點
        if (npcObject != null && spawnPoint != null)
        {
            // 如果你有用 CharacterController，移動前要先關閉它，移完再打開，否則座標會被物理強行拉回原位
            CharacterController cc = npcObject.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            npcObject.transform.position = spawnPoint.position;
            npcObject.transform.rotation = spawnPoint.rotation;

            if (cc != null) cc.enabled = true;
            
            // 💡 註：因為我們『沒有重新載入場景』，只是單純把位置搬過去，
            // 玩家目前身上的背包、道具、變數、得分等所有紀錄都會「完美繼承」！
        }

        // 3. 隱藏死亡畫面
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(false);
        }

        // 4. 如果是第一人稱遊戲，可以重新把滑鼠鎖定藏起來
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    /// <summary>
    /// 🌟 按鈕按了【否】：放棄遊戲，跳回開始選單介面
    /// </summary>
    public void OnClickMainMenu()
    {
        Debug.Log("[死亡系統] 玩家選擇【否】，跳回主選單。");
        
        // ⚠️ 關鍵：跳場景前一定要把時間恢復成 1，否則主選單也會跟著暫停死當！
        Time.timeScale = 1f;

        // 切換到你指定的開始介面場景
        SceneManager.LoadScene(mainMenuSceneName);
    }
}