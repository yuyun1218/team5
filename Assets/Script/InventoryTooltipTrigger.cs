//using UnityEngine;
//using UnityEngine.EventSystems;

//public class InventoryTooltipTrigger : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
//{
  //  private string currentDescription;

   // public void SetDescription(string description) => currentDescription = description;

    // 🌟 改為偵測點擊事件
  //  public void OnPointerClick(PointerEventData eventData)
  //  {
        // 檢查是否為滑鼠右鍵
  //      if (eventData.button == PointerEventData.InputButton.Right)
  //      {
  //          if (TooltipManager.Instance != null && !string.IsNullOrEmpty(currentDescription))
  //          {
  //              Debug.Log($"[Tooltip] 右鍵點擊：顯示描述");
    //            TooltipManager.Instance.ShowTooltip(currentDescription);
   //         }
   //     }
   // }

    // 🌟 滑鼠離開格子時自動隱藏，避免畫面殘留
 //   public void OnPointerExit(PointerEventData eventData)
   // {
    //    if (TooltipManager.Instance != null)
        //{
   //         TooltipManager.Instance.HideTooltip();
       // }
   // }

    //private void OnDisable()
   // {
    //    if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip();
    //}
//}
