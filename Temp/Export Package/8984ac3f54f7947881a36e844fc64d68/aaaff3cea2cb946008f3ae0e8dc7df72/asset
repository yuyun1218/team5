using UnityEngine;

public class MirrorTarget : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 直接檢查碰到的物件名稱，或是其父物件的名稱
        if (other.name == "FlatMirror_Parent" || other.gameObject.name.Contains("FlatMirror"))
        {
            Debug.Log("<color=green>【成功】偵測到鏡子到達目標點！</color>");
            
            if (ItemSpawner.Instance != null)
            {
                ItemSpawner.Instance.NotifyPuzzleSolved(this.transform);
            }
        }
    }
}
