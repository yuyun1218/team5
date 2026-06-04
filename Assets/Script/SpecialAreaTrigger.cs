using UnityEngine;
using UnityEngine.UI;

public class SpecialAreaTrigger : MonoBehaviour
{
    [Header("═══ 🌟 監控鎖定設定（新增） ═══")]
    [Tooltip("此區域唯一對應的大監控相機編號（例如 1、2、3）。如果是互切模式，請填入對應的數字")]
    public int targetMonitorNumber = 2;

    [Header("═══ 右上角切換狀態燈 UI 連動 ═══")]
    // public Image toggleStatusImage; 
    //public Sprite redStateSprite;
    //public Sprite greenStateSprite;

    [Header("═══ 自由攝影機配置 ═══")]
    [Tooltip("請把場景中的 FreeMoveCamera 本體拖進來")]
    public GameObject freeMoveCamera;

    [Header("═══ 偵測標籤 ═══")]
    [Tooltip("偵測 NPC 的標籤，例如 'NPC'")]
    public string npcTag = "NPC";
    [Tooltip("偵測自由攝影機的標籤，例如 'FreeCamera' (請確保您的 FreeMoveCamera 物件有此標籤)")]
    public string freeCameraTag = "FreeCamera";

    [Header("═══ 內部狀態（除錯用） ═══")]
    public bool isNPCInZone = false;       
    public bool isFreeCameraInZone = false; // 新增：追蹤自由攝影機是否在區域內

    private void Start()
    {
        SetStatusLight(false);
    }

    private void Update()
    {
        // E 鍵邏輯現在統一由 CameraManager 處理，這裡不再直接處理 E 鍵
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(npcTag))
        {
            isNPCInZone = true;
            SetStatusLight(true);
            // 通知 CameraManager 有 SpecialAreaTrigger 進入範圍 (由 NPC 觸發)
            if (CameraManager.Instance != null) CameraManager.Instance.SetSpecialAreaTrigger(this);
        }
        else if (other.CompareTag(freeCameraTag))
        {
            isFreeCameraInZone = true;
            // 當自由攝影機進入區域時，自動觸發返回監控視角 (或固定視角) 的邏輯
            // 這裡直接呼叫 CameraManager 的 ExitFreeMoveCameraView，讓它處理返回邏輯
            if (CameraManager.Instance != null && CameraManager.Instance.currentCameraState == CameraManager.CameraState.FreeMoveView)
            {
                Debug.Log($"[SpecialAreaTrigger] 自由攝影機進入區域 {gameObject.name}，觸發返回！");
                CameraManager.Instance.ExitFreeMoveCameraView();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(npcTag))
        {
            isNPCInZone = false;
            SetStatusLight(false);
            // 通知 CameraManager SpecialAreaTrigger 離開範圍 (由 NPC 觸發)
            if (CameraManager.Instance != null) CameraManager.Instance.SetSpecialAreaTrigger(null);
        }
        else if (other.CompareTag(freeCameraTag))
        {
            isFreeCameraInZone = false;
            // 自由攝影機離開區域時，如果它還在自由模式，則清除 SpecialAreaTrigger 引用
            if (CameraManager.Instance != null && CameraManager.Instance.currentCameraState == CameraManager.CameraState.FreeMoveView)
            {
                // CameraManager.Instance.SetSpecialAreaTrigger(null); // 這裡不需要清除，因為 ExitFreeMoveCameraView 已經處理了
            }
        }
    }

    // 🌟 這個方法現在由 CameraManager 呼叫，用於進入自由攝影機視角
    public void TriggerSwitchToFreeMoveView()
    {
        if (freeMoveCamera == null) return;
        freeMoveCamera.SetActive(true);
        FreeMoveCamera fScript = freeMoveCamera.GetComponent<FreeMoveCamera>();
        if (fScript != null) fScript.enabled = true; // 啟用自由移動腳本

        Debug.Log($"[SpecialAreaTrigger] 進入自由移動攝影機視角：{gameObject.name}");
    }

    // 🌟 這個方法現在由 CameraManager 呼叫，用於從自由攝影機視角返回
    public void ReturnFromFreeMoveView()
    {
        if (freeMoveCamera == null) return;
        FreeMoveCamera fScript = freeMoveCamera.GetComponent<FreeMoveCamera>();
        if (fScript != null) fScript.enabled = false; // 禁用自由移動腳本
        freeMoveCamera.SetActive(false); // 禁用自由攝影機物件

        Debug.Log($"[SpecialAreaTrigger] 從自由移動攝影機視角返回：{gameObject.name}");
    }

    private void SetStatusLight(bool canToggle)
    {
        //if (toggleStatusImage == null) return;
        //toggleStatusImage.sprite = canToggle ? greenStateSprite : redStateSprite;
    }
}