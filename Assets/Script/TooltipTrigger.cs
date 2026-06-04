using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("═══ 道具描述 ═══")]
    [TextArea(3, 10)]
    public string itemDescription; // 在這裡輸入該道具的描述

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 滑鼠進入時顯示
        if (TooltipManager.Instance != null && !string.IsNullOrEmpty(itemDescription))
        {
            TooltipManager.Instance.ShowTooltip(itemDescription);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 滑鼠離開時隱藏
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }

    // 防止在道具被摧毀或格子被關閉時提示框殘留
    private void OnDisable()
    {
        if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip();
    }
}
