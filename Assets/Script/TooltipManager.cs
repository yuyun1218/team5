using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;
    public GameObject tooltipPanel; 
    public Text tooltipText;        
    public Vector2 offset = new Vector2(15, 15); 

    private void Awake()
    {
        Instance = this;
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    private void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            tooltipPanel.transform.position = (Vector2)Input.mousePosition + offset;
        }
    }

    public void ShowTooltip(string content)
    {
        if (tooltipPanel == null || tooltipText == null) return;
        tooltipText.text = content;
        tooltipPanel.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipPanel.GetComponent<RectTransform>());
    }

    public void HideTooltip() { if (tooltipPanel != null) tooltipPanel.SetActive(false); }
}
