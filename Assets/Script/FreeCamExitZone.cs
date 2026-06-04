using UnityEngine;

//public class FreeCamExitZone : MonoBehaviour
//{
    //[Header("═══ 🌟 拖拽目標 ═══")]
    //[Tooltip("請把你的自由飄移相機 (機一) 拖進這個格子裡綁定")]
    //public FreeMoveCamera targetFreeMoveCamera;

   // private void Start()
    //{
        // 確保出口感應區的 Collider 是 Trigger 狀態
       // Collider col = GetComponent<Collider>();
       // if (col != null)
        //{
        //    col.isTrigger = true;
        //}
    //}

    //private void OnTriggerEnter(Collider other)
   // {
        // 檢查進來的是不是我們綁定的那一台自由相機
       // FreeMoveCamera cam = other.GetComponent<FreeMoveCamera>();
       // if (cam != null && cam == targetFreeMoveCamera)
       // {
            // 運用相機原本就有的提示功能，秀出提示字
         //   cam.ShowLocalPrompt("按 E 離開此區域 (返回監控)");
            
            // 💡 運用相機自帶的變數，安全地告訴它：你現在站在出口了！
            // 這樣在 CameraManager 檢查時，GetIsInExitZone() 就會正確回傳 true
           // cam.GetType()
            //   .GetField("isInExitZone", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
           //    ?.SetValue(cam, true);
        //}
  //  }

    //private void OnTriggerExit(Collider other)
    //{
       // FreeMoveCamera cam = other.GetComponent<FreeMoveCamera>();
       // if (cam != null && cam == targetFreeMoveCamera)
        //{
            // 離開區域，隱藏提示字
         //   cam.HideLocalPrompt();
            
            // 告訴相機：你離開出口了，不准切換回監控！
           // cam.GetType()
           //    .GetField("isInExitZone", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            //   ?.SetValue(cam, false);
       // }//
    //}
//}