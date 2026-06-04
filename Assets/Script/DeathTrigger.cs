using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    [Header("═══ 偵測標籤 ═══")]
    public string targetTag = "NPC";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log($"[死亡區域] {other.name} 踩入危險區域。");
            
            // 🌟 核心防禦：如果 NPC 踩進來，立刻在原地拔掉牠身上可能偷偷重置場景的腳本
            // 假設你的舊腳本叫 NPC_Health 或 AI_Controller，可以在這裡強行關閉牠：
            /*
            var health = other.GetComponent<NPC_Health>();
            if (health != null) health.enabled = false; 
            */

            // 透過 UIManager 的單例切換到死亡狀態
            if (UIManager.Instance != null)
            {
                UIManager.Instance.SetGameState(UIManager.GameState.Death);
            }
            else
            {
                Debug.LogError("場景中找不到 UIManager 物件！");
            }
        }
    }
}