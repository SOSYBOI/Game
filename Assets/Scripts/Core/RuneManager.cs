using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro; 
using System.Threading.Tasks; 

public class RuneManager : MonoBehaviour
{
    public static RuneManager Instance;
    
    [Header("玩家与预制体设置")]
    public Transform playerTransform;    // 【新增】你的玩家角色位置基准
    public GameObject runeStonePrefab;   // 符文石预制体
    
    [Header("留言 UI 设置 (按 M 呼出)")]
    public GameObject writeMessageUI;
    public TMP_InputField messageInputField;

    //I have switched the key bind ot F because the skill uses Q,W,E,R. You can change it back to F after.
    //Also, like keybind is switched to G.
    [Header("阅读 UI 设置 (靠近按 F 呼出)")]
    //[Header("阅读 UI 设置 (靠近按 E 呼出)")]
    public GameObject readMessageUI;
    public TMP_Text readContentText;
    public TMP_Text readLikesText;
    [SerializeField] private GameObject openMessageUI;

    private FirebaseFirestore db;
    private const int CHUNK_SIZE = 50; 
    private Dictionary<string, GameObject> spawnedStones = new Dictionary<string, GameObject>();

    void Awake() { Instance = this; }

    void Start()
    {
        // 游戏开始时隐藏所有 UI 并锁定鼠标
        if (writeMessageUI != null) writeMessageUI.SetActive(false);
        if (readMessageUI != null) readMessageUI.SetActive(false);
        if (openMessageUI != null) openMessageUI.SetActive(false);

        LockCursor(true);

        // 监听输入框的回车键提交 (玩家打完字按 Enter 直接发送)
        if (messageInputField != null) {
            messageInputField.onSubmit.AddListener(delegate { OnSubmitMessageButtonClicked(); });
        }

        // 初始化 Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available) {
                db = FirebaseFirestore.DefaultInstance;
                Debug.Log("[System] Firebase connected, scanning for nearby runes...");
                
                // 确保有玩家引用后再加载区块
                if (playerTransform != null) {
                    LoadRuneStonesInChunk(GetChunkId(playerTransform.position)); 
                } else {
                    Debug.LogWarning("Player Transform not assigned in RuneManager!");
                }
            }
        });
    }

    void Update()
    {
        if (db == null || Keyboard.current == null) return;

        // 按 M 键打开/关闭留言界面（如果在看别人留言时，禁止按 M）
        if (Keyboard.current.mKey.wasPressedThisFrame && !readMessageUI.activeSelf) {
            ToggleWriteUI();
        }
    }

    // --- UI 控制模块 ---
    private void ToggleWriteUI()
    {
        if (writeMessageUI == null || messageInputField == null) return;
        
        bool isActive = writeMessageUI.activeSelf;
        writeMessageUI.SetActive(!isActive); 
        
        if (!isActive) {
            messageInputField.text = "";
            messageInputField.ActivateInputField(); // 自动聚焦输入框
            //LockCursor(false); // 解锁鼠标
        } else {
            //LockCursor(true);
        }
    }

    public void OpenMessage()
    {
        openMessageUI.SetActive(true);
    }

    public void CloseMessage()
    {
        openMessageUI.SetActive(false);
    }

    // 供 RuneInteractable 靠近按 E 时调用
    public void OpenReadUI(string content, int likes)
    {
        if (readMessageUI == null) return;
        
        readContentText.text = content;
        readLikesText.text = $"Liked {likes} times"; // 【修改】纯英文显示
        
        readMessageUI.SetActive(true);
        //LockCursor(false);
    }

    // 供阅读面板的“关闭”按钮调用
    public void CloseReadUI()
    {
        if (readMessageUI == null) return;
        
        readMessageUI.SetActive(false);
        //LockCursor(true); 
    }

    // 统一管理鼠标指针状态
    private void LockCursor(bool isLocked)
    {
        if (isLocked) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // 绑定给“Submit”按钮的点击事件 (或回车触发)
    public void OnSubmitMessageButtonClicked()
    {
        string customText = messageInputField.text;
        
        if (string.IsNullOrWhiteSpace(customText)) {
            Debug.LogWarning("Cannot engrave an empty rune!");
            return; 
        }

        if (playerTransform == null) {
            Debug.LogError("Player Transform is missing! Please assign it in the Inspector.");
            return;
        }

        // 【核心修改】：以真正的玩家朝向和位置生成，防止原地叠加
        Vector3 spawnPos = playerTransform.position + playerTransform.forward * 2.0f;
        spawnPos.y += 0.5f;

        PlaceRuneStone(customText, spawnPos);

        writeMessageUI.SetActive(false);
        //LockCursor(true);
    }

    // --- 核心数据模块 ---
    public string GetChunkId(Vector3 pos)
    {
        int chunkX = Mathf.FloorToInt(pos.x / CHUNK_SIZE);
        int chunkZ = Mathf.FloorToInt(pos.z / CHUNK_SIZE);
        return $"Chunk_{chunkX}_{chunkZ}";
    }

    // 【核心修改】：客户端预生成（零延迟显示）
    public void PlaceRuneStone(string text, Vector3 pos)
    {
        // 1. 瞬间生成：让客户端直接生成一个合法的空文档引用
        DocumentReference newRuneRef = db.Collection("runes").Document();
        string newId = newRuneRef.Id;

        // 2. 准备数据
        RuneStoneData data = new RuneStoneData {
            content = text,
            chunk_id = GetChunkId(pos),
            x = pos.x, y = pos.y, z = pos.z,
            likes = 0,
            timestamp = Timestamp.GetCurrentTimestamp()
        };

        // 3. 零延迟：在发送网络请求前，立刻在本地世界生成
        SpawnRuneStone(newId, data);

        // 4. 后台同步：静默上传到云端
        newRuneRef.SetAsync(data).ContinueWithOnMainThread(task => {
            if (task.IsCompleted) {
                Debug.Log($"[System] Rune [{newId}] successfully synced to cloud!");
            } else {
                Debug.LogError("Failed to sync rune: " + task.Exception);
            }
        });
    }

    public void LoadRuneStonesInChunk(string chunkId)
    {
        Debug.Log($"[System] Searching cloud for runes in: {chunkId}...");

        db.Collection("runes").WhereEqualTo("chunk_id", chunkId)
          .GetSnapshotAsync().ContinueWithOnMainThread((Task<QuerySnapshot> task) => {
              if (task.IsFaulted) {
                  Debug.LogError("Query failed: " + task.Exception);
                  return;
              }
              
              QuerySnapshot snapshot = task.Result;
              Debug.Log($"[System] Found {snapshot.Count} runes in cloud!"); 

              foreach (DocumentSnapshot doc in snapshot.Documents) {
                  if (spawnedStones.ContainsKey(doc.Id)) continue;

                  RuneStoneData data = doc.ConvertTo<RuneStoneData>();
                  SpawnRuneStone(doc.Id, data);
              }
          });
    }

    public void LikeRuneStone(string docId)
    {
        DocumentReference docRef = db.Collection("runes").Document(docId);
        docRef.UpdateAsync("likes", FieldValue.Increment(1)); 
    }

    private void SpawnRuneStone(string docId, RuneStoneData data)
    {
        Vector3 spawnPos = new Vector3(data.x, data.y, data.z);
        GameObject stone = Instantiate(runeStonePrefab, spawnPos, Quaternion.identity);
        
        stone.GetComponent<RuneInteractable>().Initialize(docId, data);
        spawnedStones.Add(docId, stone);
    }
}