// Assets/Scripts/Level/LevelExit.cs
using UnityEngine;

/// <summary>
/// 關卡出口觸發器。當玩家進入時觸發關卡轉移。
/// </summary>
public class LevelExit : MonoBehaviour
{
    [SerializeField] private string targetLevelName;              // 目標關卡名稱
    [SerializeField] private Vector3 playerSpawnOffset = Vector3.zero;  // 玩家生成相對於出口的偏移
    [SerializeField] private bool showDebugInfo = true;           // 是否顯示調試訊息

    private Collider triggerCollider;  // 觸發器碰撞體

    private void Start()
    {
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
        {
            // 若無碰撞體則自動添加箱型觸發器
            triggerCollider = gameObject.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
        }
    }

    /// <summary>
    /// 當玩家進入觸發區域時觸發。
    /// </summary>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !LevelManager.Instance.IsTransitioning())
        {
            if (showDebugInfo)
                Debug.Log($"玩家進入出口，轉移至 {targetLevelName}");

            // 計算玩家生成位置（出口位置 + 偏移）
            Vector3 spawnPosition = transform.position + playerSpawnOffset;
            LevelManager.Instance.TransitionToLevel(targetLevelName, spawnPosition);
        }
    }

    /// <summary>
    /// 在編輯器中可視化出口位置與玩家生成點。
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, Vector3.one);  // 繪出出口邊界
        Gizmos.DrawLine(transform.position, transform.position + playerSpawnOffset);  // 繪出生成偏移
    }
}
