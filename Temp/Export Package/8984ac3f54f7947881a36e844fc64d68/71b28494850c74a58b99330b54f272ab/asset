using UnityEngine;

public class InteractArea : MonoBehaviour
{
    public Camera fixedCamera; // 該區域的固定鏡頭
    
    private bool npcInside = false; // ★ 防止重複觸發

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC") && !npcInside) // ★ 只在第一次進入時執行
        {
            npcInside = true;
            CameraManager.Instance.SetInteractArea(this);
            Debug.Log($"[InteractArea] 進入區��：{gameObject.name}，攝影機：{fixedCamera.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            npcInside = false; // ★ 重置標記
            CameraManager.Instance.SetInteractArea(null);
            Debug.Log($"[InteractArea] 離開區域：{gameObject.name}");
        }
    }
}