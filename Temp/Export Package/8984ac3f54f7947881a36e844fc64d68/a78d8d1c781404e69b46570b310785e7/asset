using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemData itemData; // 在 Inspector 拖入對應的 ItemData
    private bool isPlayerNear = false;

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            var inv = FindFirstObjectByType<InventorySystem>();
            if (inv != null)
            {
                inv.AddItem(itemData);
                if (UIManager.Instance != null) UIManager.Instance.HideInteractPrompt();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerNear = true;
            if (UIManager.Instance != null) UIManager.Instance.ShowInteractPrompt("按 F 拾取");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerNear = false;
            if (UIManager.Instance != null) UIManager.Instance.HideInteractPrompt();
        }
    }
}
