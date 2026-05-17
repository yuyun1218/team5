using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InteractAreaSetupTool : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("自動匹配相鄰攝影機")]
    public void AutoMatchCamera()
    {
        InteractArea[] allAreas = FindObjectsByType<InteractArea>(FindObjectsSortMode.None);
        
        foreach (var area in allAreas)
        {
            if (area.fixedCamera != null) continue; // 跳過已指定的
            
            // 在同層級尋找攝影機
            Camera foundCamera = area.GetComponentInParent<Transform>()
                .parent?.GetComponentInChildren<Camera>();
            
            if (foundCamera == null)
            {
                // 尋找名稱相似的攝影機
                string areaName = area.gameObject.name;
                Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
                
                foreach (var cam in allCameras)
                {
                    if (cam.gameObject.name.Contains(areaName.Replace("InteractArea", "Camera")))
                    {
                        foundCamera = cam;
                        break;
                    }
                }
            }
            
            if (foundCamera != null)
            {
                area.fixedCamera = foundCamera;
                Debug.Log($"✓ '{area.gameObject.name}' 已指定攝影機：{foundCamera.name}");
            }
            else
            {
                Debug.LogWarning($"✗ '{area.gameObject.name}' 找不到對應攝影機");
            }
        }
        
        EditorUtility.SetDirty(Selection.activeObject);
    }

    [ContextMenu("檢查所有 InteractArea")]
    public void CheckAllInteractAreas()
    {
        InteractArea[] allAreas = FindObjectsByType<InteractArea>(FindObjectsSortMode.None);
        
        Debug.Log($"========== 檢查 {allAreas.Length} 個 InteractArea ==========");
        
        int validCount = 0;
        int invalidCount = 0;
        
        foreach (var area in allAreas)
        {
            if (area.fixedCamera != null)
            {
                Debug.Log($"✓ {area.gameObject.name} → {area.fixedCamera.name}");
                validCount++;
            }
            else
            {
                Debug.LogWarning($"✗ {area.gameObject.name} → 未指定攝影機");
                invalidCount++;
            }
        }
        
        Debug.Log($"========== 結果：{validCount} 個有效，{invalidCount} 個無效 ==========");
    }
#endif
}