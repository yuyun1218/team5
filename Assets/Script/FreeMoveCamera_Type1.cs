using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Camera))]
public class FreeMoveCamera : MonoBehaviour
{
    [Header("═══ 物理移動設定 ═══")]
    public float moveSpeed = 5.0f;
    private CharacterController characterController;

    [Header("═══ 旋轉設定 ═══")]
    public float rotateSpeed = 2.0f;
    private float rotationX = 0.0f;

    [Header("═══ 互動感知狀態 ═══")]
    private InteractArea targetInteractArea = null;
    private PickupItem targetPickupItem = null;

    [Header("═══ 出口監控切換狀態 ═══")]
    [SerializeField] private bool isInExitZone = false;

    [Header("═══ 限制狀態 ═══")]
    [SerializeField] private bool isInFixedView = false;

    [Header("═══ 🌟 出口限定設定 ═══")]
    public GameObject specificExitZoneCollider;

    public GameObject entranceTriggerArea;

    [Header("═══ 🌟 提示UI ═══")]
    public Text localMessageText;
    public GameObject localMessageBgPanel;

    //---------------------------------------------------
    // Space 傳送功能
    //---------------------------------------------------

    [Header("═══ Space觀察點 ═══")]
    public Transform teleportPoint;

    private bool canUseTeleport = false;
    private bool isAtTeleportPoint = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    //---------------------------------------------------

    public bool GetIsInExitZone()
    {
        return (specificExitZoneCollider == null) ? true : isInExitZone;
    }

    public InteractArea GetTargetInteractArea()
    {
        return targetInteractArea;
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        Vector3 currentRotation = transform.localRotation.eulerAngles;
        rotationX = currentRotation.y;
        transform.localRotation = Quaternion.Euler(0, rotationX, 0);

        targetInteractArea = null;
        targetPickupItem = null;

        isInFixedView = false;
        isInExitZone = false;

        canUseTeleport = false;
        isAtTeleportPoint = false;

        HideLocalPrompt();
    }

    void Update()
    {
        //---------------------------------------------------
        // Space 觀察點功能
        //---------------------------------------------------

        if (canUseTeleport && Input.GetKeyDown(KeyCode.Space))
        {
            if (!isAtTeleportPoint)
            {
                // 紀錄原位置
                originalPosition = transform.position;
                originalRotation = transform.rotation;

                isInFixedView = true;

                characterController.enabled = false;

                transform.position = teleportPoint.position;
                transform.rotation = teleportPoint.rotation;

                characterController.enabled = true;

                isAtTeleportPoint = true;

                UIManager.Instance.ShowInteractPrompt("按 Space 返回", 3.0f);
            }
            else
            {
                characterController.enabled = false;

                transform.position = originalPosition;
                transform.rotation = originalRotation;

                characterController.enabled = true;

                isAtTeleportPoint = false;
                isInFixedView = false;

                UIManager.Instance.ShowInteractPrompt("按 Space 靠近觀察", 3.0f);
            }
            

            return;
        }

        //---------------------------------------------------

        if (isInFixedView)
            return;

        // 滑鼠旋轉

        if (Input.GetMouseButtonDown(1))
            Cursor.lockState = CursorLockMode.Locked;

        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * rotateSpeed;
            transform.localRotation = Quaternion.Euler(0, rotationX, 0);
        }

        if (Input.GetMouseButtonUp(1))
            Cursor.lockState = CursorLockMode.None;

        // WASD移動

        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");

        Vector3 moveDirection =
            (transform.forward * moveV + transform.right * moveH);

        moveDirection.y = 0;

        if (characterController != null &&
            moveDirection != Vector3.zero)
        {
            characterController.Move(
                moveDirection.normalized *
                moveSpeed *
                Time.deltaTime
            );
        }
    }

    //---------------------------------------------------
    // 固定視角
    //---------------------------------------------------

    public void LockIntoFixedView()
    {
        isInFixedView = true;
        HideLocalPrompt();
    }

    public void WakeUpFromFixedView()
    {
        isInFixedView = false;

        if (targetInteractArea != null)
            ShowLocalPrompt("按 E 進入視角");
    }

    //---------------------------------------------------
    // UI
    //---------------------------------------------------

    public void ShowLocalPrompt(string text)
    {
        if (localMessageText != null)
        {
            localMessageText.text = text;
            localMessageText.gameObject.SetActive(true);
        }

        if (localMessageBgPanel != null)
            localMessageBgPanel.SetActive(true);
    }

    public void HideLocalPrompt()
    {
        if (localMessageText != null)
            localMessageText.gameObject.SetActive(false);

        if (localMessageBgPanel != null)
            localMessageBgPanel.SetActive(false);
    }

    //---------------------------------------------------
    // Trigger
    //---------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (isInFixedView)
            return;

        // 撿道具

        PickupItem item = other.GetComponent<PickupItem>();

        if (item != null)
        {
            targetPickupItem = item;
            ShowLocalPrompt("按 F 拾取道具");
            return;
        }

        // 固定視角

        InteractArea area = other.GetComponent<InteractArea>();

        if (area != null)
        {
            targetInteractArea = area;
            ShowLocalPrompt("按 E 進入視角");
            return;
        }

        // Space觀察區

        if (other.CompareTag("TeleportZone"))
        {
            canUseTeleport = true;
            ShowLocalPrompt("按 Space 觀察");
            return;
        }

        // 出口區

        if (specificExitZoneCollider != null &&
            other.gameObject == specificExitZoneCollider)
        {
            isInExitZone = true;

            ShowLocalPrompt("按 E 返回監控");

            if (entranceTriggerArea != null)
                entranceTriggerArea.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<InteractArea>() != null)
        {
            targetInteractArea = null;
            HideLocalPrompt();
        }

        if (other.CompareTag("TeleportZone"))
        {
            canUseTeleport = false;

            if (!isAtTeleportPoint)
                HideLocalPrompt();
        }

        if (specificExitZoneCollider != null &&
            other.gameObject == specificExitZoneCollider)
        {
            isInExitZone = false;
            HideLocalPrompt();
        }
    }
}