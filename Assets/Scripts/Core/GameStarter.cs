// Assets/Scripts/Core/GameStarter.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private string persistentSceneName = "Persistent";
    [SerializeField] private string firstLevelName = "Level_MainHall";
    [SerializeField] private Vector3 firstLevelSpawnPosition = Vector3.zero;

    private void Start()
    {
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        // 加載 Persistent 場景
        Debug.Log("加載 Persistent 場景...");
        yield return SceneManager.LoadSceneAsync(persistentSceneName, LoadSceneMode.Additive);

        // 等待管理器初始化
        yield return new WaitForSeconds(0.5f);

        // 加載第一個級別
        Debug.Log($"加載第一個級別: {firstLevelName}");
        LevelManager.Instance.TransitionToLevel(firstLevelName, firstLevelSpawnPosition);
    }
}
