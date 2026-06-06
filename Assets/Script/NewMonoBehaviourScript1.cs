using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance;
    private int currentCount = 0;
    private int targetCount = 3;

    private void Awake() { Instance = this; }

    public void AddCount()
    {
        currentCount++;
        // 這裡直接呼叫 UIManager 的更新方法
        //UIManager.Instance.UpdateTaskProgress(currentCount);
    }
}