using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("核心引用")]
    public NPCController npcController;
    public LayerMask walkableLayer;

    [Header("滑鼠懸停回饋")]
    [Tooltip("指派場景中跟隨滑鼠的標記物件 (例如一個圓圈)")]
    public GameObject mouseIndicator; 

    [Header("距離範圍設定")]
    [Tooltip("攝影機射線的最大偵測距離")]
    public float maxRaycastDistance = 100f;
    
    [Tooltip("是否限制點擊距離必須在 NPC 周圍？")]
    public bool limitClickRange = false;
    
    [Tooltip("NPC 可被指揮的最遠距離")]
    public float maxClickRangeFromNPC = 10f;

    void Start()
    {
        // 初始時隱藏標記
        if (mouseIndicator != null) mouseIndicator.SetActive(false);
    }

    void Update()
    {
        // --- 1. 視角切換 (E 鍵) ---
        // 放在最外面，確保在任何狀態下按 E 都能觸發視角切換
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.ToggleNPCView();
            }
        }

        // --- 2. 背包開關 (Tab 鍵) ---
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (InventorySystem.Instance != null)
            {
                InventorySystem.Instance.ToggleInventory();
            }
        }

        // --- 3. 監控視角下的互動邏輯 ---
        bool isMonitoring = CameraManager.Instance != null && !CameraManager.Instance.isNPCView;
        bool isInventoryOpen = InventorySystem.Instance != null && InventorySystem.Instance.inventoryPanel.activeSelf;
        

        if (isMonitoring)
        {
            // A. 更新滑鼠懸停標記 (僅在監控視角顯示)
            UpdateMouseIndicator();

            // B. 處理點擊移動 (需背包關閉時才有效)
            if (!isInventoryOpen && Input.GetMouseButtonDown(0))
            {
                HandleMovement();
            }
        }
        else
        {
            // 非監控視角 (NPC 視角) 時隱藏標記
            if (mouseIndicator != null) mouseIndicator.SetActive(false);
        }
        
    }

    void UpdateMouseIndicator()
    {
        if (mouseIndicator == null) return;
        Camera currentCam = CameraManager.Instance.currentCamera;
        if (currentCam == null) return;

        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRaycastDistance, walkableLayer))
        {
            // 檢查距離限制邏輯
            if (limitClickRange && npcController != null)
            {
                float dist = Vector3.Distance(npcController.transform.position, hit.point);
                mouseIndicator.SetActive(dist <= maxClickRangeFromNPC);
            }
            else
            {
                mouseIndicator.SetActive(true);
            }
            
            // 讓標記跟隨碰撞點，稍微抬高避免閃爍
            mouseIndicator.transform.position = hit.point + Vector3.up * 0.05f;
        }
        else
        {
            mouseIndicator.SetActive(false);
        }
    }

    void HandleMovement()
    {
        Camera currentCam = CameraManager.Instance.currentCamera;
        if (currentCam == null) return;

        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRaycastDistance, walkableLayer))
        {
            // 點擊時的距離檢查
            if (limitClickRange && npcController != null)
            {
                float dist = Vector3.Distance(npcController.transform.position, hit.point);
                if (dist > maxClickRangeFromNPC) return; 
            }

            if (npcController != null)
            {
                npcController.MoveTo(hit.point);
            }
        }
    }
    
}
