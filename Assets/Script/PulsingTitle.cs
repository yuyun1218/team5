using UnityEngine;

public class PulsingTitle : MonoBehaviour
{
    private Vector3 originalScale;
    
    public float minScale = 0.95f;       // 最小縮放
    public float maxScale = 1.05f;       // 最大縮放
    public float pulseSpeed = 1.5f;      // 脈衝速度

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        // 脈衝效果
        float scale = Mathf.Lerp(minScale, maxScale, 
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        
        transform.localScale = originalScale * scale;
    }
}