using System.Collections;
using UnityEngine;

public class MirrorTarget : MonoBehaviour
{
    [Header("--- 鏡頭設定 ---")]
    [Tooltip("機關完成時切換過去看的特寫鏡頭")]
    public Camera cutsceneCamera;          

    [Tooltip("特寫結束後，要切換回來的固定監控鏡頭 (請直接拖入該鏡頭物件)")]
    public Camera returnToCamera; 

    [Header("--- 隱藏地板設定 ---")]
    [Tooltip("場景中預先隱藏的地板物件")]
    public GameObject hiddenFloor;         

    [Header("--- 時間設定 ---")]
    [Tooltip("鏡頭停留在特寫畫面的秒數")]
    public float cameraStayDuration = 2.0f;

    private bool isPuzzleSolved = false;   

    void Start()
    {
        // 初始化：確保特寫鏡頭跟地板一開始是關閉的
        if (cutsceneCamera != null) cutsceneCamera.enabled = false;
        if (hiddenFloor != null) hiddenFloor.SetActive(false);
    }

    // 因為現在改用直接拖入的方式，所以 SetReturnCameraName 這個方法可以不需要了。
    // 但為了不讓 ItemRequiredZone 報錯，我們保留這個空方法，或者你可以去刪掉那邊的呼叫。
    public void SetReturnCameraName(string cameraName) { }

    // 物理碰撞偵測 (當鏡子外力推入時)
    private void OnTriggerEnter(Collider other)
    {
        if (isPuzzleSolved) return;

        if (other.name == "FlatMirror_Parent" || other.gameObject.name.Contains("FlatMirror"))
        {
            TriggerPuzzleCompletion(other.gameObject);
        }
    }

    // 【核心觸發方法】
    public void TriggerPuzzleCompletion(GameObject mirrorObject)
    {
        if (isPuzzleSolved) return;
        isPuzzleSolved = true;

        Debug.Log("<color=green>【成功】偵測到鏡子到達目標點！</color>");
        
        if (ItemSpawner.Instance != null)
        {
            ItemSpawner.Instance.NotifyPuzzleSolved(this.transform);
        }

        // 1. 顯示出隱藏地板
        if (hiddenFloor != null)
        {
            hiddenFloor.SetActive(true);
        }

        // 2. 開始鏡頭特寫
        StartCoroutine(CameraCutsceneSequence());
    }

    IEnumerator CameraCutsceneSequence()
    {
        // 1. 取得當前所有啟用中的鏡頭並將其關閉
        // (這樣可以確保不管你目前在哪台監控，都能切換到特寫)
        foreach (Camera cam in Camera.allCameras)
        {
            if (cam != cutsceneCamera) cam.enabled = false;
        }

        // 2. 開啟特寫鏡頭
        if (cutsceneCamera != null)
        {
            cutsceneCamera.enabled = true;
            Debug.Log("【鏡頭】切換至特寫視角");
        }

        // 3. 等待特寫秒數
        yield return new WaitForSeconds(cameraStayDuration);

        // 4. 關閉特寫鏡頭
        if (cutsceneCamera != null) cutsceneCamera.enabled = false;

        // 5. 🌟 【核心修改：切換回你設置的鏡頭】
        if (returnToCamera != null)
        {
            // 確保物件本身是 Active，且 Camera 組件是 Enabled
            returnToCamera.gameObject.SetActive(true);
            returnToCamera.enabled = true;
            Debug.Log($"<color=cyan>【鏡頭】特寫結束，已還原至指定鏡頭：{returnToCamera.gameObject.name}</color>");
        }
        else
        {
            // 防呆：如果忘記拖入鏡頭，就嘗試開啟 Camera.main
            if (Camera.main != null) Camera.main.enabled = true;
            Debug.LogError("【報錯】MirrorTarget 忘記在 Inspector 拖入 Return To Camera！已自動嘗試開啟主鏡頭。");
        }

        // 6. 功成身退
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;
        this.enabled = false;
    }
}