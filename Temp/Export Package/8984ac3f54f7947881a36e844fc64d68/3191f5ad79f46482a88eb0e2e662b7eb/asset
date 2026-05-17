using UnityEngine;

public class DebugTrigger : MonoBehaviour
{
    private void Start()
    {
        // 檢查自身設定
        Collider col = GetComponent<Collider>();
        if (col == null) Debug.LogError("【診斷】錯誤：MirrorTarget 身上沒有 Collider！");
        else if (!col.isTrigger) Debug.LogWarning("【診斷】警告：MirrorTarget 的 Collider 沒有勾選 Is Trigger！");
    }

    private void OnTriggerEnter(Collider other)
    {
        // 只要有任何東西碰到，就印出資訊
        Debug.Log($"<color=cyan>【診斷】偵測到碰撞！</color> 物件名稱: {other.name}, 標籤: {other.tag}, 是否有 Rigidbody: {other.attachedRigidbody != null}");
        
        if (other.CompareTag("Mirror"))
        {
            Debug.Log("<color=green>【診斷】成功！偵測到 Mirror 標籤，應該要觸發解謎了。</color>");
        }
        else
        {
            Debug.Log("<color=yellow>【診斷】失敗：碰到東西了但標籤不是 Mirror。請檢查鏡子父物件的 Tag 設定。</color>");
        }
    }
}
