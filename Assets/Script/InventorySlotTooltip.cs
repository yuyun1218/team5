using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemData currentItem;
    private bool isMouseOver = false;
    private bool tooltipActive = false;

    public void SetItem(ItemData item)
    {
        currentItem = item;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    private void Update()
    {
        // 滑鼠在格子上 + 右鍵按下 = 顯示提示
        if (isMouseOver && Input.GetMouseButtonDown(1) && currentItem != null)
        {
            ShowTooltip(currentItem);
            return;
        }

        // 提示已顯示 + 滑鼠不在格子上 + 任何點擊 = 關閉提示
        if (tooltipActive && !isMouseOver && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
        {
            HideTooltip();
        }
    }

    private void ShowTooltip(ItemData item)
    {
        if (UIManager.Instance != null)
        {
            string tooltipText = $"<b>{item.itemName}</b>\n{item.itemDescription}";
            UIManager.Instance.ShowInteractPrompt(tooltipText, 5f); // 5秒後自動關閉
            tooltipActive = true;
        }
    }

    private void HideTooltip()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideInteractPrompt();
            tooltipActive = false;
        }
    }
}