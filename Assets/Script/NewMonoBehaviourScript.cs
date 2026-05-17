using UnityEngine;

public class CameraManagerDiagnostics : MonoBehaviour
{
    [ContextMenu("診斷所有 InteractArea")]
    public void DiagnoseAllInteractAreas()
    {
        InteractArea[] allAreas = FindObjectsByType<InteractArea>(FindObjectsSortMode.None);
        
        Debug.Log($"========== 診斷開始：找到 {allAreas.Length} 個 InteractArea ==========");
        
        foreach (var area in allAreas)
        {
            string status = area.fixedCamera != null ? "✓ 有效" : "✗ 未指定";
            string cameraName = area.fixedCamera != null ? area.fixedCamera.name : "null";
            
            Debug.Log($"[{status}] {area.gameObject.name} -> fixedCamera: {cameraName}");
        }
        
        Debug.Log("========== 診斷完成 ==========");
    }
}