// Assets/Scripts/Bullet/Bullet.cs
using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    [Header("基礎屬性")]
    public Vector3 velocity = Vector3.zero;  // 子彈速度向量
    public Vector3 position;                 // 子彈當前位置
    public Rigidbody rb;                     // 剛體元件參考
    
    [Header("生存時間")]
    public float lifetime = 5f;    // 子彈最大生存時間
    private float elapsedTime = 0f;  // 已存活時間
    
    // 行為系統
    private List<IBulletBehavior> behaviors = new List<IBulletBehavior>();          // 當前套用的行為列表
    private List<IBulletBehavior> behaviorToRemove = new List<IBulletBehavior>();  // 待移除的行為列表
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            // 若無剛體則自動添加並設定屬性
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
    
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        position = transform.position;
        
        // 更新所有行為
        UpdateBehaviors(Time.deltaTime);
        
        // 應用速度到剛體
        rb.velocity = velocity;
        
        // 檢查生存時間，到期則銷毀
        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 更新所有行為，並移除已完成的行為。
    /// </summary>
    private void UpdateBehaviors(float deltaTime)
    {
        behaviorToRemove.Clear();
        
        foreach (var behavior in behaviors)
        {
            // 執行行為更新，回傳 false 表示行為完成
            bool shouldContinue = behavior.Update(this, deltaTime);
            if (!shouldContinue)
            {
                behavior.OnBehaviorEnd(this);
                behaviorToRemove.Add(behavior);
            }
        }
        
        // 移除已完成的行為
        foreach (var behavior in behaviorToRemove)
        {
            behaviors.Remove(behavior);
        }
    }
    
    /// <summary>
    /// 添加行為到子彈。
    /// </summary>
    public void AddBehavior(IBulletBehavior behavior)
    {
        behavior.Initialize(this);
        behaviors.Add(behavior);
    }
    
    /// <summary>
    /// 移除指定行為。
    /// </summary>
    public void RemoveBehavior(IBulletBehavior behavior)
    {
        behaviors.Remove(behavior);
    }
    
    /// <summary>
    /// 一次性添加多個行為（可變參數）。
    /// </summary>
    public void AddBehaviors(params IBulletBehavior[] behaviorArray)
    {
        foreach (var behavior in behaviorArray)
        {
            AddBehavior(behavior);
        }
    }
    
    /// <summary>
    /// 清空所有行為。
    /// </summary>
    public void ClearBehaviors()
    {
        behaviors.Clear();
    }
}
