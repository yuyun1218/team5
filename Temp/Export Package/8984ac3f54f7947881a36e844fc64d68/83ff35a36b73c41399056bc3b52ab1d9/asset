using UnityEngine;
using UnityEngine.SceneManagement;

public class KillOnContact : MonoBehaviour
{
    // 當實體碰撞發生時，Unity 會自動呼叫此函式
    private void OnCollisionEnter(Collision collision)
    {
        // collision.gameObject 代表撞到 A 物體的那個東西
        if (collision.gameObject.CompareTag("NPC"))
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("玩家撞到障礙物死亡！");
        
        // 範例：重新載入當前關卡
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}