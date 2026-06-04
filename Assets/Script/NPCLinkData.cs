using UnityEngine;

public class NPCLinkData : MonoBehaviour
{
    [Header("═══ NPC 共享數據資料 ═══")]
    public string npcName = "Spirit_A";
    public int currentHealth = 100;
    public bool isPurified = false; // 是否已被淨化/解謎完成
    
    [Header("═══ 任務/解謎進度 ═══")]
    public int currentPuzzleStage = 0;

    /// <summary>
    /// 複製另一個 NPC 的所有數值（用於兩者切換時的資料同步）
    /// </summary>
    public void CopyDataFrom(NPCLinkData sourceData)
    {
        if (sourceData == null) return;
        
        this.npcName = sourceData.npcName;
        this.currentHealth = sourceData.currentHealth;
        this.isPurified = sourceData.isPurified;
        this.currentPuzzleStage = sourceData.currentPuzzleStage;
        
        Debug.Log($"[{gameObject.name}] 成功從 [{sourceData.gameObject.name}] 同步數據！當前解謎進度：階層 {currentPuzzleStage}");
    }
}