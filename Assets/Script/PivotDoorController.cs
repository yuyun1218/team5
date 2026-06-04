using UnityEngine;

public class PivotDoorController : MonoBehaviour
{
    [Header("═══ 旋轉軸心物件（手動建立的空物件） ═══")]
    [Tooltip("請把擺在牆角門鈕位置的空物件 (Door_Pivot) 拖進來")]
    public Transform doorPivot;

    [Header("═══ 🌟 正面靠近時的【軸心旋轉角度】 ═══")]
    [Tooltip("當 NPC 從【正面】靠近時，軸心要轉動的角度 (可以自由微調 X, Y, Z)")]
    public Vector3 frontOpenRotation = new Vector3(0, -90f, 0);

    [Header("═══ 🌟 背面靠近時的【軸心旋轉角度】 ═══")]
    [Tooltip("當 NPC 從【背面】靠近時，軸心要轉動的角度 (可以自由微調 X, Y, Z)")]
    public Vector3 backOpenRotation = new Vector3(0, 90f, 0);

    [Header("═══ 開門速度 ═══")]
    public float doorSpeed = 5f;

    [Header("═══ 偵測標籤（NPC） ═══")]
    public string targetTag = "NPC";

    private Quaternion closeRotation;
    private Quaternion targetRotation;

    private void Start()
    {
        if (doorPivot == null)
        {
            Debug.LogError($"[{gameObject.name}] 忘記拖入負責旋轉的 Door_Pivot 物件了！");
            return;
        }

        // 紀錄遊戲一開始時，軸心空物件的原始關門角度
        closeRotation = doorPivot.localRotation;
        targetRotation = closeRotation;
    }

    private void Update()
    {
        // 🌟 平滑地將軸心空物件旋轉到目標角度（連帶帶動底下的所有門零件）
        if (doorPivot != null)
        {
            doorPivot.localRotation = Quaternion.Slerp(doorPivot.localRotation, targetRotation, Time.deltaTime * doorSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && doorPivot != null)
        {
            // 用 Trigger 的前方向 (Forward) 計算 NPC 來的方位
            Vector3 directionToNPC = other.transform.position - transform.position;
            float dotProduct = Vector3.Dot(transform.forward, directionToNPC.normalized);

            Vector3 chosenAngles;

            if (dotProduct > 0)
            {
                chosenAngles = frontOpenRotation;
                Debug.Log($"[軸心門] {other.name} 從【正面】靠近，軸心往 frontOpenRotation 旋轉！");
            }
            else
            {
                chosenAngles = backOpenRotation;
                Debug.Log($"[軸心門] {other.name} 從【背面】靠近，軸心往 backOpenRotation 旋轉！");
            }

            // 設定軸心要轉過去的目標角度
            targetRotation = closeRotation * Quaternion.Euler(chosenAngles);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag) && doorPivot != null)
        {
            // NPC 離開，軸心自動轉回原始關門角度
            targetRotation = closeRotation;
            Debug.Log("[軸心門] NPC 離開，軸心恢復原樣（自動關門）。");
        }
    }
}