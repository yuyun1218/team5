using UnityEngine;

public class DeskLampController : MonoBehaviour
{
    [Header("═══ 檯燈基本設定 ═══")]
    [Tooltip("燈光顏色，建議使用溫暖的淡黃色或微橘色，更有 healing 療癒感")]
    public Color lampColor = new Color(1f, 0.89f, 0.63f); // 溫暖暖白光
    
    [Tooltip("燈光強度。手繪風格建議 1.5 ~ 3 之間，微亮不刺眼")]
    public float lightIntensity = 2.0f;
    
    [Tooltip("光線照射的最大距離")]
    public float lightRange = 7.0f;

    [Header("═══ 聚光燈錐角設定 ═══")]
    [Range(10f, 90f)] [Tooltip("外圈半徑：決定檯燈照亮的範圍大小")]
    public float spotAngle = 50f;
    
    [Range(0f, 100f)] [Tooltip("內圈柔和度：值越高，光圈邊緣越模糊越柔和")]
    public float innerPercent = 40f;

    [Header("═══ 鏡子防護隔離設定 ═══")]
    [Tooltip("請輸入你之前幫鏡子設定的 Layer 名稱（例如: Mirror）")]
    public string mirrorLayerName = "Mirror";

    private Light deskLight;

    private void Awake()
    {
        // 1. 自動檢查或在子物件中建立一盞 Spot Light
        deskLight = GetComponentInChildren<Light>();
        if (deskLight == null)
        {
            GameObject lightObj = new GameObject("Lamp_Spotlight");
            lightObj.transform.SetParent(this.transform);
            lightObj.transform.localPosition = Vector3.zero;
            // 預設朝下照
            lightObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); 
            deskLight = lightObj.AddComponent<Light>();
        }

        // 2. 套用完美的檯燈光線參數
        SetupDeskLight();
    }

    private void OnValidate()
    {
        // 在 Inspector 調整數值時，畫面可以即時連動更新（方便調整）
        if (deskLight == null) deskLight = GetComponentInChildren<Light>();
        if (deskLight != null) SetupDeskLight();
    }

    private void SetupDeskLight()
    {
        // 強制將燈光類型設定為 Spotlight（聚光燈）
        deskLight.type = LightType.Spot;

        // 設定顏色與強度
        deskLight.color = lampColor;
        deskLight.intensity = lightIntensity;
        deskLight.range = lightRange;

        // 設定聚光燈切角與柔和度
        deskLight.spotAngle = spotAngle;
        deskLight.innerSpotAngle = spotAngle * (innerPercent / 100f);

        // 🌟 核心：開啟 Soft Shadows（柔和陰影），讓手繪場景的陰影邊緣柔和、帶有療癒感
        deskLight.shadows = LightShadows.Soft;
        deskLight.shadowStrength = 0.65f; 

        // 🌟 核心防護：動態計算 Culling Mask，排除鏡子圖層
        // 只要這一塊運作正常，鏡子就不會反射出這盞燈的任何亮點！
        int everythingMask = ~0; // 代表原本全勾選（Everything）
        int mirrorLayer = LayerMask.NameToLayer(mirrorLayerName);

        if (mirrorLayer != -1)
        {
            // 利用位元運算，把鏡子圖層從燈光的照亮範圍中「剔除（Uncheck）」
            deskLight.cullingMask = everythingMask & ~(1 << mirrorLayer);
        }
        else
        {
            Debug.LogWarning($"[檯燈腳本] 找不到名為「{mirrorLayerName}」的 Layer！請確認你已經在 Unity 建立該圖層，否則燈光可能會照瞎鏡子。");
        }
    }
}