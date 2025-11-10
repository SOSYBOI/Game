// Assets/Scripts/Enemy/EnemySpawner.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    [Header("生成設定")]
    [SerializeField] private BaseEnemy enemyPrefab;              // 要生成的敵人 Prefab
    [SerializeField] private bool shouldRespawnAfterCheckpoint = true;  // 是否在檢查點休息後重生

    private BaseEnemy spawnedEnemy;   // 已生成的敵人實例
    private Vector3 spawnPosition;    // 生成點位置

    private void Start()
    {
        // 記錄生成點位置
        spawnPosition = transform.position;
        
        // 立即生成敵人
        SpawnEnemy();

        // 訂閱檢查點休息事件
        EventManager.StartListening("OnCheckpointRest", OnCheckpointRest);
    }

    private void OnDestroy()
    {
        // 取消訂閱事件
        EventManager.StopListening("OnCheckpointRest", OnCheckpointRest);
    }

    /// <summary>
    /// 在生成點生成敵人。
    /// </summary>
    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: 未指定敵人 Prefab!");
            return;
        }

        // 實例化敵人 Prefab
        spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // 將敵人移到當前級別場景
        Scene currentLevelScene = gameObject.scene;
        if (currentLevelScene.IsValid())
        {
            SceneManager.MoveGameObjectToScene(spawnedEnemy.gameObject, currentLevelScene);
            Debug.Log($"Enemy spawned in scene: {currentLevelScene.name}");
        }
        else
        {
            Debug.LogWarning("Could not assign enemy to current level scene");
        }
        
        // 將敵人註冊到 EnemyManager
        EnemyManager.Instance.RegisterEnemy(spawnedEnemy);

        Debug.Log($"已在 {spawnPosition} 生成 {enemyPrefab.name}");
    }

    /// <summary>
    /// 玩家在檢查點休息時的回調。
    /// 若設定允許重生且敵人已死亡，則復活敵人。
    /// </summary>
    private void OnCheckpointRest()
    {
        if (shouldRespawnAfterCheckpoint && spawnedEnemy != null && spawnedEnemy.IsDead)
        {
            // 重置敵人狀態
            spawnedEnemy.ResetEnemy();
            
            // 重新啟用敵人
            spawnedEnemy.gameObject.SetActive(true);
            
            Debug.Log($"檢查點休息後重生 {enemyPrefab.name}");
        }
    }

    /// <summary>
    /// 在編輯器中可視化生成點（綠色線框球體）。
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
