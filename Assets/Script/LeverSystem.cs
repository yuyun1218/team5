using UnityEngine;

public class LeverSystem : MonoBehaviour
{
    public static LeverSystem Instance;

    [Header("槓桿支點設定")]
    public Transform pivotPoint; 
    
    [Header("目標物品設定")]
    public GameObject targetObject;
    [Tooltip("物品最終要到達的目標點位（即平面鏡移動的終點）")]
    public Transform mirrorTargetPoint;
    
    [Header("自動推動設定")]
    public float pushSpeed = 0.5f;
    public float maxTiltAngle = 45f;
    [Tooltip("距離終點多近算通關（若發生推到底卻沒觸發，可適度調大此數值，例如 0.2）")]
    public float snapThreshold = 0.1f;

    private bool isLeverActive = false;
    private GameObject currentStick;
    private float currentPushProgress = 0f; 
    private Vector3 initialTargetPos; 
    private bool isSolved = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 當玩家在 ItemRequiredZone 的固定鏡頭 Panel 點擊使用木棍後，由此處正式啟動
    public void SetupLever(ItemData stickData)
    {
        if (pivotPoint == null || stickData == null || stickData.prefab == null || targetObject == null || mirrorTargetPoint == null) return;

        // 雙重防護：如果已經解開了，就不重複觸發
        if (isSolved) return;

        isLeverActive = true;
        currentPushProgress = 0f;
        initialTargetPos = targetObject.transform.position;
        
        // --- 確保木棍可見與生成 ---
        // 1. 先在世界座標生成，不設父物件，避免受父物件縮放影響
        currentStick = Instantiate(stickData.prefab, pivotPoint.position, pivotPoint.rotation);
        
        // 2. 強制重置縮放值為 Prefab 的原始縮放
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
            UIManager.Instance.ShowMessage("長按 F 推動機關");
    }

    private void Update()
    {
        // 沒啟用、已解開、或沒按住 F 鍵，就直接跳出
        if (!isLeverActive || isSolved || !Input.GetKey(KeyCode.F)) return;

        HandleAutomaticLever();
    }

    private void HandleAutomaticLever()
    {
        // 🌟 優化：改成單向推到底。長按 F 就一直前進，直到 1.0 (100%) 為止，不再自動倒退
        if (currentPushProgress < 1f)
        {
            currentPushProgress += pushSpeed * Time.deltaTime;
            if (currentPushProgress > 1f) currentPushProgress = 1f;
        }

        // 讓鏡子（targetObject）往目標點（mirrorTargetPoint）平滑移動
        if (targetObject != null && mirrorTargetPoint != null)
        {
            targetObject.transform.position = Vector3.Lerp(initialTargetPos, mirrorTargetPoint.position, currentPushProgress);

            // 動態檢查距離
            float distance = Vector3.Distance(targetObject.transform.position, mirrorTargetPoint.position);
            
            // 當前進到終點範圍內，或進度已經強行拉滿時，判定機關完成
            if (distance < snapThreshold || currentPushProgress >= 1f)
            {
                OnLeverComplete();
                return; // 觸發完成後立刻跳出，防止重複呼叫
            }
        }

        // 讓木棍隨進度產生搖擺動畫 (從 -maxTiltAngle 搖擺到 +maxTiltAngle)
        if (currentStick != null)
        {
            float angle = Mathf.Lerp(-maxTiltAngle, maxTiltAngle, currentPushProgress);
            currentStick.transform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // 當物品完全推到位時觸發
    private void OnLeverComplete()
    {
        // 強制歸位到精準終點
        targetObject.transform.position = mirrorTargetPoint.position;
        
        isSolved = true;
        isLeverActive = false;

        // 讓木棍停在最終角度
        if (currentStick != null)
        {
            currentStick.transform.localRotation = Quaternion.Euler(0, 0, maxTiltAngle);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMessage("道路已開啟");
        }

        // --- 核心串接 ---
        // 通知場景中的 MirrorTarget 開啟特殊特寫與顯現地板
        MirrorTarget mirrorTargetScript = FindFirstObjectByType<MirrorTarget>();
        if (mirrorTargetScript != null)
        {
            mirrorTargetScript.TriggerPuzzleCompletion(targetObject);
        }
        else
        {
            Debug.LogWarning("【警告】場景中找不到掛有 MirrorTarget 腳本的物件，請檢查機關擺放！");
        }
    }
}