using UnityEngine;
using UnityEngine.EventSystems; // 引入 Event System 命名空間
using UnityEngine.AI;         // 引入 NavMesh 命名空間

public class GroundClickDetector : MonoBehaviour, IPointerClickHandler
{
    // 定義一個靜態事件，當地板被點擊時觸發，並傳遞點擊位置
    // 任何訂閱這個事件的腳本都會收到通知
    public static event System.Action<Vector3> OnGroundClicked;

    // 當滑鼠點擊到帶有此腳本的物件時，此方法會被 Event System 自動呼叫
    public void OnPointerClick(PointerEventData eventData)
    {
        // 確保是滑鼠左鍵點擊 (0 代表左鍵，1 代表右鍵，2 代表中鍵)
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Event System 會提供一個 RaycastResult，其中包含點擊的 3D 世界座標
            // 但為了確保點擊點在 NavMesh 上且可達，我們需要使用 NavMesh.SamplePosition
            // 這裡我們從 EventData 中獲取擊中點的世界座標
            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;

            NavMeshHit navHit;
            // 嘗試在點擊位置附近（半徑 1.0f）尋找最近的 NavMesh 上的點
            if (NavMesh.SamplePosition(clickPosition, out navHit, 1.0f, NavMesh.AllAreas))
            {
                // 如果找到可達的 NavMesh 點，則觸發事件，將該點傳遞給訂閱者
                OnGroundClicked?.Invoke(navHit.position);
                Debug.Log("地板被點擊，目標位置: " + navHit.position);
            }
            else
            {
                Debug.LogWarning("點擊位置不在 NavMesh 上或不可達: " + clickPosition);
            }
        }
    }
}