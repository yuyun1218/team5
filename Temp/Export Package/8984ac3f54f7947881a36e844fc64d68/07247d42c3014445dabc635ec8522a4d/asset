using UnityEngine;
using UnityEngine.AI;
// using UnityEngine.EventSystems; // 如果不需要直接處理 UI 事件，可以不引用

[RequireComponent(typeof(NavMeshAgent))] // 確保物件上有 NavMeshAgent 組件
public class NPCController : MonoBehaviour
{
    private NavMeshAgent agent; // 導航代理人組件
    private Animator animator;   // 動畫控制器組件

    [Header("狀態")]
    public bool canInteract = false;
    public InteractArea currentInteractArea; // 互動區域組件
    
    private const float WALK_SPEED_THRESHOLD = 0.1f; // 判斷是否行走的最小速度閾值

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        if (animator == null)
            Debug.LogWarning("[NPCController] 找不到 Animator 組件");

        // 確保 NavMeshAgent 存在且已啟用
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent 組件未找到！請確保物件上已附加 NavMeshAgent。");
            enabled = false; // 禁用此腳本，避免後續錯誤
        }
    }

    // 當此腳本啟用時呼叫
    private void OnEnable()
    {
        // 訂閱地板點擊事件。當 GroundClickDetector 發出 OnGroundClicked 事件時，
        // 會呼叫此腳本的 MoveTo 方法。
        GroundClickDetector.OnGroundClicked += MoveTo;
    }

    // 當此腳本禁用或銷毀時呼叫
    private void OnDisable()
    {
        // 取消訂閱地板點擊事件，防止記憶體洩漏和空引用錯誤
        GroundClickDetector.OnGroundClicked -= MoveTo;
    }

    private void Update()
    {
        // ★ 實時更新動畫
        UpdateAnimation();
    }

    // 公開的移動方法，現在會被 GroundClickDetector 的事件呼叫
    public void MoveTo(Vector3 destination)
    {
        // 確保 agent 存在且角色在 NavMesh 上，才能設定目的地
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(destination);
        }
        else
        {
            Debug.LogWarning("NavMeshAgent 不在 NavMesh 上或未準備好，無法移動到: " + destination);
        }
    }

    /// <summary>
    /// 獲取當前移動速度（用於動畫切換）
    /// </summary>
    public float GetCurrentSpeed()
    {
        // 確保 agent 存在，否則返回 0
        if (agent != null)
            return agent.velocity.magnitude;
        
        return 0f;
    }

    /// <summary>
    /// 更新角色動畫
    /// </summary>
    private void UpdateAnimation()
    {
        // 如果沒有 Animator，則不執行動畫更新
        if (animator == null) return;
        
        float speed = GetCurrentSpeed();
        
        // 根據速度切換動畫參數 "IsWalking"
        if (speed > WALK_SPEED_THRESHOLD)
            animator.SetBool("IsWalking", true);
        else
            animator.SetBool("IsWalking", false);
    }

    // 觸發器進入事件
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InteractArea"))
        {
            canInteract = true;
            currentInteractArea = other.GetComponent<InteractArea>();
            
            // 只有在監控視角才顯示 E 提示
            // 這些行因為 CameraManager 和 UIManager 未提供，所以暫時註釋掉，
            // 您可以根據您的專案實際情況取消註釋或修改。
            if (!CameraManager.Instance.isNPCView)
             {
                 UIManager.Instance.ShowInteractPrompt("按 E 切換視角");
             }
        }
    }

    // 觸發器離開事件
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("InteractArea"))
        {
            canInteract = false;
            currentInteractArea = null;
            // UIManager.Instance.HideInteractPrompt(); // 同上，暫時註釋
        }
    }
}