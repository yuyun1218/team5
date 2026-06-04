using UnityEngine;
using System.Collections.Generic;

public class IsolatedCameraLogicSystem : MonoBehaviour
{
    [Header("═══ 1. 監控視角格子 (僅管理此名單) ═══")]
    public List<Camera> monitorGrid = new List<Camera>();

    [Header("═══ 2. 自由鏡頭格子 (僅管理此名單) ═══")]
    public List<GameObject> freeMoveGrid = new List<GameObject>();

    [Header("═══ 3. 固定視角格子 (僅管理此名單) ═══")]
    public List<Camera> fixedGrid = new List<Camera>();

    [Header("═══ 內部狀態 ═══")]
    [SerializeField] private bool cameFromFreeMove = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var manager = CameraManager.Instance;
            if (manager == null) return;

            // 🌟 第一步：確認目前正在使用的攝影機是否在「格子名單」內
            Camera currentCam = manager.currentCamera;
            if (!IsCameraInMyGrids(currentCam)) return; // 如果不在名單內，完全不動，交給原本邏輯

            // 🌟 第二步：如果在名單內，則執行您要求的特定邏輯
            HandleIsolatedLogic(manager);
            
            // 🌟 第三步：為了防止原本 Manager 的 E 鍵邏輯產生二次觸發
            // 我們利用反射強行將 Manager 的切換冷卻時間重設，使其在這一幀不動作
            SetField(manager, "lastToggleTime", Time.time);
        }
    }

    private void HandleIsolatedLogic(CameraManager manager)
    {
        var state = manager.currentCameraState;

        if (state == CameraManager.CameraState.MonitorView)
        {
            // 監控 -> 自由 (E)
            var trigger = GetField<SpecialAreaTrigger>(manager, "currentSpecialAreaTrigger");
            if (trigger != null && IsObjectInGrid(trigger.freeMoveCamera, freeMoveGrid)) {
                manager.SwitchToFreeMoveView(trigger);
            } 
            // 監控 -> 固定 (E)
            else {
                var area = GetField<InteractArea>(manager, "currentInteractArea");
                if (area != null && monitorGrid.Contains(manager.currentCamera)) {
                    cameFromFreeMove = false;
                    manager.SwitchToFixedNPCView(area);
                }
            }
        }
        else if (state == CameraManager.CameraState.FreeMoveView)
        {
            var freeCam = GetField<FreeMoveCamera>(manager, "cachedFreeMoveCamScript");
            if (freeCam == null) return;

            // 自由 -> 固定 (E)
            var targetArea = freeCam.GetTargetInteractArea();
            if (targetArea != null && fixedGrid.Contains(targetArea.fixedCamera)) {
                cameFromFreeMove = true;
                manager.SwitchToFixedNPCView(targetArea);
                freeCam.LockIntoFixedView();
            }
            // 自由 -> 監控 (點位 + E)
            else if (freeCam.GetIsInExitZone()) {
                manager.ReturnToMonitorView();
            }
        }
        else if (state == CameraManager.CameraState.FixedNPCView)
        {
            // 固定 -> 自由 (E)
            if (cameFromFreeMove) {
                var fCamScript = GetField<FreeMoveCamera>(manager, "cachedFreeMoveCamScript");
                if (fCamScript != null) ReturnToFreeMove(manager, fCamScript);
            }
            // 固定 -> 監控 (E)
            else {
                manager.ReturnToMonitorView();
            }
        }
    }

    private void ReturnToFreeMove(CameraManager manager, FreeMoveCamera freeCam)
    {
        if (manager.currentCamera != null) manager.currentCamera.enabled = false;
        Camera fCam = freeCam.GetComponent<Camera>();
        if (fCam != null) fCam.enabled = true;

        SetField(manager, "currentCameraState", CameraManager.CameraState.FreeMoveView);
        SetField(manager, "activeCamera", fCam);
        freeCam.WakeUpFromFixedView();
        cameFromFreeMove = false;
    }

    // 💡 檢查攝影機是否屬於此系統管理
    private bool IsCameraInMyGrids(Camera cam)
    {
        if (cam == null) return false;
        if (monitorGrid.Contains(cam)) return true;
        if (fixedGrid.Contains(cam)) return true;
        foreach (var obj in freeMoveGrid) if (obj != null && obj.GetComponent<Camera>() == cam) return true;
        return false;
    }

    private bool IsObjectInGrid(GameObject obj, List<GameObject> grid)
    {
        return obj != null && grid.Contains(obj);
    }

    // 💡 反射工具
    private T GetField<T>(object obj, string name) {
        var f = obj.GetType().GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        return (f != null) ? (T)f.GetValue(obj) : default(T);
    }

    private void SetField(object obj, string name, object val) {
        var f = obj.GetType().GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        if (f != null) f.SetValue(obj, val);
    }
}
