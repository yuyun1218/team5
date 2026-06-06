using UnityEngine;
using System.Collections;

public class PanelTimer : MonoBehaviour
{
    [Header("設定")]
    public GameObject targetPanel; // 拖入你想顯示的 Panel
    public float displayDuration = 3.0f; // 顯示幾秒

    // 這是給按鈕綁定的方法
    public void ShowPanelWithTimer()
    {
        if (targetPanel != null)
        {
            // 停止之前的計時，避免連續點擊導致 Bug
            StopAllCoroutines();
            StartCoroutine(ShowAndHideRoutine());
        }
    }

    private IEnumerator ShowAndHideRoutine()
    {
        targetPanel.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        targetPanel.SetActive(false);
    }
}