using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    [Header("道具預製物")]
    public GameObject flatMirrorPrefab;
    public GameObject convexMirrorPrefab;

    [Header("生成與拖曳設定")]
    public float distanceFromCamera = 2.0f;
    public float verticalOffset = -0.5f;
    public float dragSpeed = 2.0f;

    [Header("🌟 初始面向微調 (Inspector)")]
    public Vector3 flatMirrorRotationOffset = new Vector3(90, 0, 0);
    public Vector3 convexMirrorRotationOffset = new Vector3(0, 180, 0);

    [Header("解謎目標")]
    public GameObject hiddenPath; 

    private GameObject currentActiveItem;
    private ItemData currentItemData;
    private bool isPuzzleSolved = false;

    // 🌟 核心：追蹤這面鏡子是在哪一台攝影機視角下開啟的
    private Camera spawnedCamera;
    private int mirrorCount = 0;

    // 🌟 旋轉靈敏度設定
    [Header("🌟 鏡子旋轉速度")]
    public float mirrorRotateSpeed = 5.0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (hiddenPath != null) hiddenPath.SetActive(false);
    }

    private void Update()
    {
        // 🌟 核心機制：每幀檢查攝影機是否被切換。如果視角變了，自動回收道具回到背包（銷毀活動物件）
        CheckCameraSwitchAndRecycle();

        if (currentActiveItem == null) return;

        HandleDragTowardsCamera();
        HandleRotation();
    }

    private void CheckCameraSwitchAndRecycle()
    {
        if (currentActiveItem == null || spawnedCamera == null) return;

        // 當 CameraManager 當前的攝影機與當初生成的攝影機不同時（例如切換視角、退出監控），觸發自動回收
        if (CameraManager.Instance != null && CameraManager.Instance.currentCamera != spawnedCamera)
        {
            Debug.Log("<color=yellow>【系統偵測】玩家已切換/切回視角，自動將懸空鏡子回收至背包！</color>");
            DespawnCurrentItem();
        }
    }

    public void DespawnCurrentItem()
    {
        if (currentActiveItem != null)
        {
            Destroy(currentActiveItem);
            currentActiveItem = null;
            currentItemData = null;
            spawnedCamera = null;
        }
    }

    // 由卡槽 MirrorTarget 成功卡入固定時呼叫
    public void NotifyPuzzleSolved(Transform targetPoint)
    {
        if (currentActiveItem != null)
        {
            currentActiveItem.transform.position = targetPoint.position;
            currentActiveItem.transform.rotation = targetPoint.rotation;
            
            // 🌟 關鍵：固定成功後，解除指針追蹤！
            // 這樣切換視角時，CheckCameraSwitchAndRecycle 就不會把這面已經卡在機關上的鏡子刪掉！
            currentActiveItem = null; 
            currentItemData = null;
            spawnedCamera = null;
            Debug.Log("[ItemSpawner] 鏡子已成功固定，脫離視角監控回收機制。");
        }
        
        if (!isPuzzleSolved)
        {
            isPuzzleSolved = true;
            if (hiddenPath != null) hiddenPath.SetActive(true);
            if (UIManager.Instance != null) UIManager.Instance.ShowMessage("道路已開啟");
        }
    }

    public void SpawnItem(ItemData data)
    {
        // 🌟 道具交替使用：如果手上正飄著一面鏡子，點按別的道具時，自動把舊的洗掉
        if (currentActiveItem != null) 
        {
            // 如果重複點擊背包裡「同一個鏡子」，就直接當作「收回背包」關閉消失
            if (currentItemData != null && currentItemData.itemName == data.itemName)
            {
                DespawnCurrentItem();
                return; 
            }
            Destroy(currentActiveItem);
        }
        
        currentItemData = data;
        mirrorCount++;

        bool isFlatMirror = data.itemName.Contains("平面鏡");
        GameObject prefabToSpawn = isFlatMirror ? flatMirrorPrefab : convexMirrorPrefab;

        if (prefabToSpawn != null)
        {
            // 🌟 記錄當前攝影機狀態
            if (CameraManager.Instance != null && CameraManager.Instance.currentCamera != null)
                spawnedCamera = CameraManager.Instance.currentCamera;
            else
                spawnedCamera = Camera.main;

            Transform camTransform = spawnedCamera.transform;

            Vector3 spawnPos = camTransform.position + camTransform.forward * distanceFromCamera;
            spawnPos.y += verticalOffset;
            
            Vector3 forwardOnGround = camTransform.forward;
            forwardOnGround.y = 0; 
            if (forwardOnGround == Vector3.zero) forwardOnGround = camTransform.up; 
            
            Quaternion baseRotation = Quaternion.LookRotation(forwardOnGround);

            Quaternion finalRotation = isFlatMirror ? 
                baseRotation * Quaternion.Euler(flatMirrorRotationOffset) : 
                baseRotation * Quaternion.Euler(convexMirrorRotationOffset);

            currentActiveItem = Instantiate(prefabToSpawn, spawnPos, finalRotation);
            currentActiveItem.name = isFlatMirror ? $"FlatMirror_Parent_{mirrorCount}" : $"ConvexMirror_Parent_{mirrorCount}";
            
            Rigidbody rb = currentActiveItem.GetComponent<Rigidbody>();
            if (rb == null) rb = currentActiveItem.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void HandleDragTowardsCamera()
    {
        if (Input.GetKey(KeyCode.F))
        {
            Vector3 directionToCamera = (CameraManager.Instance.currentCamera.transform.position - currentActiveItem.transform.position).normalized;
            currentActiveItem.transform.position += directionToCamera * dragSpeed * Time.deltaTime;
        }
    }

    // 🌟 重新重構的流暢旋轉機制（完全擺脫對 mousePosition 絕對座標的依賴）
    private void HandleRotation()
    {
        // 當按住右鍵時，直接讀取滑鼠的動態物理增量（Mouse X）來旋轉道具
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            
            // 只有在滑鼠真的有移動時才去轉，極度省效能且順暢
            if (Mathf.Abs(mouseX) > 0.01f)
            {
                // 用 Space.World 確保水平旋轉方向符合玩家直覺
                currentActiveItem.transform.Rotate(Vector3.up, -mouseX * mirrorRotateSpeed * 20f * Time.deltaTime, Space.World);
            }
        }
    }
}