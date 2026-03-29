using UnityEngine;
using UnityEngine.InputSystem;

public class RuneInteractable : MonoBehaviour
{
    private string documentId;
    private string messageContent;
    private int currentLikes;
    private bool isPlayerNearby = false;

    // 初始化时，从 Manager 接收数据
    public void Initialize(string docId, RuneStoneData data)
    {
        documentId = docId;
        messageContent = data.content;
        currentLikes = data.likes;
    }

    // 玩家进入感应圈 (必须带 Player 标签)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            isPlayerNearby = true;
            RuneManager.Instance.OpenMessage("Interact (F)\r\n\r\nLike (G)");
            Debug.Log("[System] Rune discovered! Press E to read, F to like.");
        }
    }

    // 玩家离开感应圈
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            isPlayerNearby = false;
            RuneManager.Instance.CloseMessage();

            // 安全机制：如果玩家在阅读时直接走开，自动关闭 UI
            if (RuneManager.Instance.readMessageUI != null && RuneManager.Instance.readMessageUI.activeSelf) {
                RuneManager.Instance.CloseReadUI();
            }
        }
    }

    void Update()
    {
        if (!isPlayerNearby || Keyboard.current == null) return;

        // 玩家按 E 键：呼出阅读 UI
        if (Keyboard.current.fKey.wasPressedThisFrame) {
            RuneManager.Instance.OpenReadUI(messageContent, currentLikes);
        }

        // 玩家按 F 键：点赞逻辑
        if (Keyboard.current.gKey.wasPressedThisFrame) {
            RuneManager.Instance.LikeRuneStone(documentId);
            currentLikes++; // 客户端表现先+1
            Debug.Log("Liked successfully! The rune glows faintly.");
            
            // 如果面板正开着，立刻刷新面板上的数字
            if (RuneManager.Instance.readMessageUI.activeSelf) {
                RuneManager.Instance.OpenReadUI(messageContent, currentLikes);
            }
        }
    }
}