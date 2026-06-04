using UnityEngine;

public class FieldItemPrompt : MonoBehaviour
{
    [Header("═══ 道具專屬 2D 提示圖 ═══")]
    [Tooltip("請把這個道具自己的 2D 提示圖物件直接拖進來")]
    public GameObject my2DImage;

    [Header("═══ 偵測標籤（NPC） ═══")]
    public string targetTag = "NPC";

    private void Start()
    {
        // 遊戲一開始，先把這個道具的 2D 提示圖藏起來
        if (my2DImage != null)
        {
            my2DImage.SetActive(false);
        }
    }

    // 🌟 核心：當玩家（NPC）碰到道具範圍就打開 2D 圖
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && my2DImage != null)
        {
            my2DImage.SetActive(true);
            Debug.Log($"[道具] {other.name} 進入範圍，開啟 {gameObject.name} 的 2D 提示圖");
        }
    }

    // 當玩家（NPC）離開道具範圍就關閉
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag) && my2DImage != null)
        {
            my2DImage.SetActive(false);
            Debug.Log($"[道具] {other.name} 離開範圍，關閉 {gameObject.name} 的 2D 提示圖");
        }
    }

    // ═══════════════════════════════════════════════
    // 🌟 核心修正：解決道具被撿取後，2D 圖示殘留的問題
    // ═══════════════════════════════════════════════

    /// <summary>
    /// 當道具被拾取、隱藏 (SetActive(false)) 時自動觸發
    /// </summary>
    private void OnDisable()
    {
        if (my2DImage != null)
        {
            my2DImage.SetActive(false);
            Debug.Log($"[道具安全機制] 道具 {gameObject.name} 被關閉，已同步隱藏 2D 提示圖");
        }
    }

    /// <summary>
    /// 當道具被撿取、遭到銷毀 (Destroy) 時自動觸發
    /// </summary>
    private void OnDestroy()
    {
        if (my2DImage != null)
        {
            // 如果 2D 圖是道具的子物件，它會隨著道具一起死掉；
            // 如果 2D 圖是放在 UI Canvas 獨立層級，這裡會幫忙把它關掉。
            my2DImage.SetActive(false);
            Debug.Log($"[道具安全機制] 道具 {gameObject.name} 被銷毀，已同步隱藏 2D 提示圖");
        }
    }
}