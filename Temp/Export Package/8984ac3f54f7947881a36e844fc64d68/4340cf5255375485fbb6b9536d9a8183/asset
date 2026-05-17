using UnityEngine;

public class RotatingTitle : MonoBehaviour
{
    public float rotationSpeed = 30f;    // 旋轉速度（度/秒）
    public float maxRotation = 5f;       // 最大旋轉角度

    private void Update()
    {
        // 溫和搖晃旋轉
        float rotation = Mathf.Sin(Time.time * rotationSpeed * 0.1f) * maxRotation;
        transform.localRotation = Quaternion.Euler(0, 0, rotation);
    }
}