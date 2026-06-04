//using UnityEngine;

//public class LockedFixedCameraArea : MonoBehaviour
//{
    //[Header("═══ 🌟 自由相機限定設定 ═══")]
    //[Tooltip("只有拉進這個空格的自由相機（例如：機一），才能觸發這個機關！")]
   // public FreeMoveCamera allowedFreeMoveCamera;

    //private Collider myCollider;

    //private void Awake()
    //{
        // 抓取物件本身的碰撞體
       // myCollider = GetComponent<Collider>();
       //if (myCollider != null)
        //{
            // 確保它是 Trigger 狀態
           // myCollider.isTrigger = true;
        //}
    //}

   // private void OnTriggerEnter(Collider other)
   // {
        // 1. 檢查撞進來的是不是自由相機
       // FreeMoveCamera incomingCam = other.GetComponent<FreeMoveCamera>();
        
       // if (incomingCam != null)
       // {
            // 2. 如果有指定限定相機，且進來的相機「不是」我們指定的那台
          //  if (allowedFreeMoveCamera != null && incomingCam != allowedFreeMoveCamera)
           // {
                // ➔ 直接不理它！並且強行叫相機對這個區域隱藏提示，假裝這裡不存在
            //    incomingCam.HideLocalPrompt();
                
                // 💡 物理防禦：如果這台相機不配觸發，直接讓相機跟這個 Trigger 物理隔離，防止它觸發 FreeMoveCamera 內部的 OnTriggerEnter
            //    if (myCollider != null)
                //{
             //       Physics.IgnoreCollision(other, myCollider, true);
                //}
            //}
      //  }
   // }

    // 當非指定的相機離開後，重設物理隔離，確保下次判斷依然準確
   // private void OnTriggerExit(Collider other)
   //{
       // if (myCollider != null)
    //    {
        //    Physics.IgnoreCollision(other, myCollider, false);
       // }
    //}
//}