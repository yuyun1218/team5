using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    [Header("═══ 設定 ═══")]
    public bool showAskPanel = true; // 勾選則顯示詢問介面，不勾選則直接顯示死因
    public int myReasonIndex = 0;   // 對應 UIManager 中 deathReasonPanels 的索引

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            if (showAskPanel)
            {
                // 流程 A：先跳出詢問面板 (Yes/No)
                if (UIManager.Instance.interactPanel != null)
                {
                    UIManager.Instance.interactPanel.SetActive(true);
                    // 同時把 index 暫存起來，讓「是」按鈕知道要顯示哪一個
                    UIManager.Instance.SetPendingReason(myReasonIndex); 
                }
            }
            else
            {
                // 流程 B：直接跳出死因圖
                UIManager.Instance.ShowSpecificDeathReason(myReasonIndex);
            }

            // 統一凍結遊戲
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}