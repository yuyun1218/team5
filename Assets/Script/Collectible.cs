using UnityEngine;

public class Collectible : MonoBehaviour
{
    private bool isPlayerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC")) 
        {
            isPlayerInRange = true;
            //UIManager.Instance.ShowInteractPrompt("按 F 撿取 平面鏡碎片");
            if (other.CompareTag("NPC"))
    {
        // 觸發碎片時顯示同樣的 Panel
        UIManager.Instance.ShowPanelTemporarily();
    }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC")) 
        {
            isPlayerInRange = false;
            UIManager.Instance.HideInteractPrompt();
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            // 通知 UIManager：我被撿走了
            UIManager.Instance.RegisterItemCollected(this.gameObject);
            
            UIManager.Instance.HideInteractPrompt();
            Destroy(gameObject);
        }
    }
}