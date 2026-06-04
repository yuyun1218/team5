using UnityEngine;
using UnityEngine.UI; 

public class InteractArea : MonoBehaviour
{
    public Camera fixedCamera; 
    
    [Header("═══ 🌟 自由相機專用設定 ═══")]
    [Tooltip("如果這個入口進入的是『可以自由移動的相機（機一）』，請把這個打勾！")]
    public bool isFreeMoveCameraEntrance = false;

    private bool npcInside = false; 

    private void Start()
    {
        SetStatusLight(false);
    }

    // 🌟 新增：提供給 CameraManager 偵測，讓它知道按 E 時要走哪種切換邏輯
    public void TriggerEntranceDisable()
    {
        if (isFreeMoveCameraEntrance)
        {
            Debug.Log($"[InteractArea] 偵測到玩家進入自由相機，正式關閉入口區域：{gameObject.name}");
            
            // 離開前先把 CameraManager 的區域清空，避免殘留提示字
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SetInteractArea(null);
            }
            
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC") && !npcInside) 
        {
            npcInside = true;
            CameraManager.Instance.SetInteractArea(this);
            SetStatusLight(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            npcInside = false; 
            CameraManager.Instance.SetInteractArea(null);
            SetStatusLight(false);
        }
    }

    private void SetStatusLight(bool canToggle)
    {
        // 保持你原本註解掉的狀態
    }
}