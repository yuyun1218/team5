using UnityEngine;

public class LeverSystem : MonoBehaviour
{
    public static LeverSystem Instance;

    [Header("槓桿支點設定")]
    public Transform pivotPoint; 
    
    [Header("目標物品設定")]
    public GameObject targetObject;
    [Tooltip("物品最終要到達的目標點位")]
    public Transform mirrorTargetPoint;
    
    [Header("自動推動設定")]
    public float pushSpeed = 0.5f;
    public float maxTiltAngle = 45f;
    public float snapThreshold = 0.1f;

    private bool isLeverActive = false;
    private GameObject currentStick;
    private float currentPushProgress = 0f; 
    private bool movingForward = true; 
    private Vector3 initialTargetPos; 
    private bool isSolved = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetupLever(ItemData stickData)
    {
        if (pivotPoint == null || stickData == null || stickData.prefab == null || targetObject == null || mirrorTargetPoint == null) return;

        isLeverActive = true;
        isSolved = false;
        currentPushProgress = 0f;
        movingForward = true;
        
        initialTargetPos = targetObject.transform.position;
        
        // --- 核心修正：確保木棍可見 ---
        // 1. 先在世界座標生成，不設父物件，避免受父物件縮放影響
        currentStick = Instantiate(stickData.prefab, pivotPoint.position, pivotPoint.rotation);
        
        // 2. 強制重置縮放值為 (1, 1, 1) 或 Prefab 的原始縮放
        currentStick.transform.localScale = stickData.prefab.transform.localScale;
        
        // 3. 現在再設為父物件，並重置局部座標
        currentStick.transform.SetParent(pivotPoint);
        currentStick.transform.localPosition = Vector3.zero;
        currentStick.transform.localRotation = Quaternion.identity;
        
        // 4. 確保物件是啟動狀態
        currentStick.SetActive(true);
        
        // 5. 關閉碰撞避免物理衝突
        Collider stickCol = currentStick.GetComponentInChildren<Collider>();
        if (stickCol != null) stickCol.enabled = false;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowMessage("長按 F 推動槓桿");
    }

    private void Update()
    {
        if (!isLeverActive || isSolved || !Input.GetKey(KeyCode.F)) return;

        HandleAutomaticLever();
    }

    private void HandleAutomaticLever()
    {
        if (movingForward)
        {
            currentPushProgress += pushSpeed * Time.deltaTime;
            if (currentPushProgress >= 1f)
            {
                currentPushProgress = 1f;
                movingForward = false; 
            }
        }
        else
        {
            currentPushProgress -= pushSpeed * Time.deltaTime;
            if (currentPushProgress <= 0f)
            {
                currentPushProgress = 0f;
                movingForward = true; 
            }
        }

        if (targetObject != null && mirrorTargetPoint != null)
        {
            targetObject.transform.position = Vector3.Lerp(initialTargetPos, mirrorTargetPoint.position, currentPushProgress);

            float distance = Vector3.Distance(targetObject.transform.position, mirrorTargetPoint.position);
            if (distance < snapThreshold)
            {
                OnLeverComplete();
            }
        }

        if (currentStick != null)
        {
            float angle = Mathf.Lerp(-maxTiltAngle, maxTiltAngle, currentPushProgress);
            currentStick.transform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnLeverComplete()
    {
        targetObject.transform.position = mirrorTargetPoint.position;
        isSolved = true;
        isLeverActive = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMessage("物品已推到位");
        }
    }
}
