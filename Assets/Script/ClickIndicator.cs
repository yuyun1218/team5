using UnityEngine;

public class ClickIndicator : MonoBehaviour
{
    public float destroyDelay = 0.8f;
    public float expandSpeed = 1.5f;
    private SpriteRenderer sr;
    private Color startColor;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) startColor = sr.color;
        Destroy(gameObject, destroyDelay);
    }

    void Update()
    {
        transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;
        if (sr != null)
        {
            float alpha = Mathf.Lerp(startColor.a, 0, Time.deltaTime * 5);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        }
    }
}
