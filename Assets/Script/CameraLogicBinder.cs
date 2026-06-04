using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class CameraGridLogicSystem : MonoBehaviour
{
    [System.Serializable]
    public class FixedAreaMapping
    {
        public InteractArea fixedArea;
        public GameObject linkedFreeMoveCamera;
    }

    [Header("═══ 0. 核心控制器 (Manager) ═══")]
    public CameraManager cameraManager;

    [Header("═══ 1. NPC 感應器設定 ═══")]
    [Tooltip("是否需要 NPC 在範圍內才能操作此系統")]
    public bool requireNPC = true;
    [SerializeField] private bool npcInside = false;

    [Header("═══ 2. 監控視角格子 (Monitor Grid) ═══")]
    public List<Camera> monitorGrid = new List<Camera>();

    [Header("═══ 3. 自由鏡頭格子 (FreeMove Grid) ═══")]
    public List<GameObject> freeMoveGrid = new List<GameObject>();

    [Header("═══ 4. 固定鏡範圍綁定 (Fixed Area Mapping) ═══")]
    public List<FixedAreaMapping> fixedAreaMappings = new List<FixedAreaMapping>();

    [Header("═══ 5. 出口範圍格子 (Exit Zone Grid) ═══")]
    public List<SpecialAreaTrigger> exitZones = new List<SpecialAreaTrigger>();

    [Header("═══ 內部狀態 ═══")]
    [SerializeField] private bool enteredFromFreeMove = false;

    // 🌟 NPC 感應邏輯
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            npcInside = true;
            Debug.Log($"[系統通知] NPC 已進入 {gameObject.name} 感應範圍，系統已開啟。");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            npcInside = false;
            Debug.Log($"[系統通知] NPC 已離開 {gameObject.name} 感應範圍，系統已關閉。");
            
            // 如果 NPC 離開時玩家還在看這組攝影機，可以選擇是否強制踢回監控
            // if (IsCameraInMySystem(cameraManager.currentCamera)) cameraManager.ReturnToMonitorView();
        }
    }

    private void Update()
    {
        if (cameraManager == null) return;

        // 🌟 核心：如果需要 NPC 且 NPC 不在場，則完全不執行邏輯
        if (requireNPC && !npcInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // 檢查目前攝影機是否屬於這套系統
            if (!IsCameraInMySystem(cameraManager.currentCamera)) return;

            // 攔截原本 Manager
            SetPrivateField(cameraManager, "lastToggleTime", Time.time);

            HandleLogic();
        }
    }

    private void HandleLogic()
    {
        var state = cameraManager.currentCameraState;

        switch (state)
        {
            case CameraManager.CameraState.MonitorView:
                var trigger = GetPrivateField<SpecialAreaTrigger>(cameraManager, "currentSpecialAreaTrigger");
                if (trigger != null && exitZones.Contains(trigger)) {
                    cameraManager.SwitchToFreeMoveView(trigger);
                }
                break;

            case CameraManager.CameraState.FreeMoveView:
                var freeCam = GetPrivateField<FreeMoveCamera>(cameraManager, "cachedFreeMoveCamScript");
                if (freeCam == null) return;

                var targetArea = freeCam.GetTargetInteractArea();
                if (targetArea != null && IsFixedAreaInMapping(targetArea)) {
                    enteredFromFreeMove = true;
                    cameraManager.SwitchToFixedNPCView(targetArea);
                    freeCam.LockIntoFixedView();
                }
                else if (freeCam.GetIsInExitZone()) {
                    var currentExit = GetPrivateField<SpecialAreaTrigger>(cameraManager, "currentSpecialAreaTrigger");
                    if (currentExit != null && exitZones.Contains(currentExit)) {
                        cameraManager.ReturnToMonitorView();
                    }
                }
                break;

            case CameraManager.CameraState.FixedNPCView:
                if (enteredFromFreeMove) {
                    var fCamScript = GetPrivateField<FreeMoveCamera>(cameraManager, "cachedFreeMoveCamScript");
                    if (fCamScript != null) ReturnToLinkedFreeMove(fCamScript);
                } else {
                    cameraManager.ReturnToMonitorView();
                }
                break;
        }
    }

    private void ReturnToLinkedFreeMove(FreeMoveCamera freeCam)
    {
        if (freeCam == null) return;
        if (cameraManager.currentCamera != null) cameraManager.currentCamera.enabled = false;
        Camera fCam = freeCam.GetComponent<Camera>();
        if (fCam != null) {
            fCam.enabled = true;
            SetPrivateField(cameraManager, "currentCameraState", CameraManager.CameraState.FreeMoveView);
            SetPrivateField(cameraManager, "activeCamera", fCam);
            SetPrivateField(cameraManager, "cachedFreeMoveCamScript", freeCam);
            freeCam.WakeUpFromFixedView();
            enteredFromFreeMove = false;
        }
    }

    private bool IsCameraInMySystem(Camera cam)
    {
        if (cam == null) return false;
        if (monitorGrid.Contains(cam)) return true;
        foreach (var obj in freeMoveGrid) if (obj != null && obj.GetComponent<Camera>() == cam) return true;
        foreach (var m in fixedAreaMappings) if (m.fixedArea != null && m.fixedArea.fixedCamera == cam) return true;
        return false;
    }

    private bool IsFixedAreaInMapping(InteractArea area) { return fixedAreaMappings.Exists(m => m.fixedArea == area); }

    private T GetPrivateField<T>(object obj, string fieldName) {
        FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        return (field != null) ? (T)field.GetValue(obj) : default(T);
    }

    private void SetPrivateField(object obj, string fieldName, object value) {
        FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (field != null) field.SetValue(obj, value);
    }
}
