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

    [Header("解謎目標 (Cube (1))")]
    public GameObject hiddenPath; 

    private GameObject currentActiveItem;
    private ItemData currentItemData;
    private Vector2 lastMousePos;
    private bool isPuzzleSolved = false;

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
        if (isPuzzleSolved || currentActiveItem == null) return;

        HandleDragTowardsCamera();
        HandleRotation();
    }

    // 由 MirrorTarget 呼叫
    public void NotifyPuzzleSolved(Transform targetPoint)
    {
        if (isPuzzleSolved) return;
        isPuzzleSolved = true;
        
        if (currentActiveItem != null)
        {
            currentActiveItem.transform.position = targetPoint.position;
            currentActiveItem.transform.rotation = targetPoint.rotation;
        }
        
        if (hiddenPath != null)
        {
            hiddenPath.SetActive(true);
            Debug.Log("<color=green>【系統】解謎成功：Cube (1) 已開啟！</color>");
        }

        if (UIManager.Instance != null)
            UIManager.Instance.ShowMessage("物品已推到位，路徑已開啟");
    }

    public void SpawnItem(ItemData data)
    {
        if (currentActiveItem != null) Destroy(currentActiveItem);
        currentItemData = data;
        isPuzzleSolved = false;

        GameObject prefabToSpawn = (data.itemName.Contains("平面鏡")) ? flatMirrorPrefab : convexMirrorPrefab;

        if (prefabToSpawn != null)
        {
            Vector3 spawnPos = CameraManager.Instance.currentCamera.transform.position + 
                               CameraManager.Instance.currentCamera.transform.forward * distanceFromCamera;
            spawnPos.y += verticalOffset;
            
            currentActiveItem = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            
            // 確保名稱正確，方便 MirrorTarget 判定
            currentActiveItem.name = "FlatMirror_Parent";
            
            // 確保有 Rigidbody 才能觸發 Trigger
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

    private void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1)) lastMousePos = Input.mousePosition;
        if (Input.GetMouseButton(1))
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastMousePos;
            currentActiveItem.transform.Rotate(Vector3.up, -delta.x * 0.5f, Space.World);
            lastMousePos = Input.mousePosition;
        }
    }
}
