// Assets/Scripts/Level/InteractiveObject.cs
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    [SerializeField] protected string stateKey;  // 唯一狀態鍵
    [SerializeField] protected Collider objectCollider;
    protected PersistentStateManager persistentState;

    protected virtual void Start()
    {
        persistentState = PersistentStateManager.Instance;
        if (objectCollider == null)
            objectCollider = GetComponent<Collider>();

        // 初始化時恢復狀態
        RestoreState();
    }

    /// <summary>
    /// 啟用此物件（改變永久狀態）
    /// </summary>
    public virtual void Activate()
    {
        persistentState.SetBoolState(stateKey, true);
        OnStateChanged(true);
        EventManager.TriggerEvent($"OnObject{gameObject.name}Activated");
        Debug.Log($"{gameObject.name} 已啟用");
    }

    /// <summary>
    /// 停用此物件
    /// </summary>
    public virtual void Deactivate()
    {
        persistentState.SetBoolState(stateKey, false);
        OnStateChanged(false);
        EventManager.TriggerEvent($"OnObject{gameObject.name}Deactivated");
    }

    /// <summary>
    /// 恢復此物件的永久狀態
    /// </summary>
    public virtual void RestoreState()
    {
        bool isActive = persistentState.GetBoolState(stateKey, false);
        OnStateChanged(isActive);
    }

    /// <summary>
    /// 狀態改變時調用。派生類實作具體行為。
    /// </summary>
    protected abstract void OnStateChanged(bool isActive);
}
