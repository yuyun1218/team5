using UnityEngine;

public class ItemHighlightManager : MonoBehaviour
{
    [Header("--- 拖入需要白光效果的道具 ---")]
    public GameObject[] interactableItems; 

    [Header("--- 呼吸燈效果設定 ---")]
    public float blinkSpeed = 2f;          // 閃爍速度
    [ColorUsage(true, true)]                // 支援 HDR 顏色，可以調出亮瞎眼的發光
    public Color glowColor = Color.white;   // 發光顏色（預設白光）

    private void Start()
    {
        // 遊戲開始時，自動初始化所有拖進來的道具
        foreach (var item in interactableItems)
        {
            if (item == null) continue;

            // 確保道具身上有 Collider，玩家才能互動撿取
            if (item.GetComponent<Collider>() == null)
            {
                Debug.LogWarning($"[Highlight] {item.name} 缺少 Collider，玩家可能無法撿取！");
            }
            
            // 開啟材質球的 Emission 功能
            Renderer[] renderers = item.GetComponentsInChildren<Renderer>();
            foreach (var ren in renderers)
            {
                foreach (var mat in ren.materials)
                {
                    mat.EnableKeyword("_EMISSION");
                }
            }
        }
    }

    private void Update()
    {
        // 計算呼吸燈的亮度變化 (在 0.2 到 1.0 之間循環)
        float emissionIntensity = 0.6f + Mathf.PingPong(Time.time * blinkSpeed, 0.4f);
        Color finalColor = glowColor * emissionIntensity;

        // 即時更新所有道具的發光強度
        foreach (var item in interactableItems)
        {
            if (item == null) continue;

            Renderer[] renderers = item.GetComponentsInChildren<Renderer>();
            foreach (var ren in renderers)
            {
                foreach (var mat in ren.materials)
                {
                    // 適用於 Unity URP Lit 或 Standard Shader 的標準自發光屬性
                    mat.SetColor("_EmissionColor", finalColor);
                }
            }
        }
    }
}