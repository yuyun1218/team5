using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DraggableObject : MonoBehaviour
{
    private bool isPlayerNear = false;
    private bool isDragging = false;
    private Transform npcTransform;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // 預設不被物理影響
    }

    private void Update()
    {
        // 必須在 NPC 視角才能拖曳
        if (isPlayerNear && CameraManager.Instance.isNPCView)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isDragging = true;
                rb.isKinematic = false;
                UIManager.Instance.HideInteractPrompt();
            }
            else if (Input.GetKeyUp(KeyCode.F))
            {
                isDragging = false;
                rb.isKinematic = true;
            }
        }

        if (isDragging && npcTransform != null)
        {
            // 往 NPC 方向移動 (簡單的跟隨邏輯)
            Vector3 targetPos = npcTransform.position + npcTransform.forward * 1.5f;
            targetPos.y = transform.position.y; // 保持高度不變
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerNear = true;
            npcTransform = other.transform;
            if (CameraManager.Instance.isNPCView)
            {
                UIManager.Instance.ShowInteractPrompt("長按 F 拖曳");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerNear = false;
            isDragging = false;
            rb.isKinematic = true;
            UIManager.Instance.HideInteractPrompt();
        }
    }
}