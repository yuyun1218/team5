using UnityEngine;

public class FreeCameraScopeTrigger : MonoBehaviour
{
    [Header("═══ 設定 ═══")]
    [Tooltip("將你的自由移動相機物件拖到這裡")]
    public GameObject targetFreeCamera;
    
    [Tooltip("🌟 新增：離開範圍時要關閉的區域 (如：交互區或開關)")]
    public GameObject targetAreaToDisable;

    [Header("═══ 狀態 ═══")]
    public bool disableOnExit = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FreeCamera"))
        {
            // 進入範圍：開啟相機與目標區域
            if (targetFreeCamera != null) targetFreeCamera.SetActive(true);
            if (targetAreaToDisable != null) targetAreaToDisable.SetActive(true);
            
            Debug.Log($"[ScopeTrigger] 進入範圍，已開啟相機與目標區域。");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FreeCamera"))
        {
            // 離開範圍：關閉相機與目標區域
            if (disableOnExit)
            {
                if (targetFreeCamera != null) targetFreeCamera.SetActive(false);
                if (targetAreaToDisable != null) targetAreaToDisable.SetActive(false);
                
                Debug.Log($"[ScopeTrigger] 離開範圍，已關閉相機與目標區域。");
            }
        }
    }
}