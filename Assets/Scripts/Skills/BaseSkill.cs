// BaseSkill.cs
using UnityEngine;

[System.Serializable]
public abstract class BaseSkill
{
    [SerializeField] protected int skillId;           // 技能編號
    [SerializeField] protected string skillName;      // 技能名稱
    [SerializeField] protected string description;    // 技能描述
    
    // 基於「技能冷卻池」的冷卻系統
    [SerializeField] protected float maxCooldownPool = 40f;      // 冷卻池最大容量
    [SerializeField] protected float cooldownCostPerCast = 10f;  // 每次施放消耗的冷卻量
    [SerializeField] protected float cooldownRegenRate = 1f;     // 每秒恢復的冷卻值
    
    [SerializeField] protected float castTime = 1f;   // 技能施放時間
    [SerializeField] protected float baseDamage = 10f; // 技能基礎傷害

    protected float currentCooldownPool;  // 當前冷卻池剩餘容量
    protected bool isCasting = false;      // 是否正在施放
    protected float castTimer = 0f;        // 施放計時器

    public int SkillId => skillId;
    public string SkillName => skillName;
    public float CurrentCooldownPool => currentCooldownPool;
    public float MaxCooldownPool => maxCooldownPool;
    public float CooldownCostPerCast => cooldownCostPerCast;
    public float CastTime => castTime;
    public bool IsCasting => isCasting;
    public bool IsReady => currentCooldownPool >= cooldownCostPerCast && !isCasting; // 是否可施放
    public int AvailableCasts => Mathf.FloorToInt(currentCooldownPool / cooldownCostPerCast); // 可施放次數

    // 建構函式：初始化時將冷卻池填滿
    public BaseSkill()
    {
        currentCooldownPool = maxCooldownPool;
    }

    public void SetIndex(int index)
    {
        skillId = index; // 設定技能索引（便於事件識別）
    }

    /// <summary>
    /// 當技能被施放時呼叫，可於子類別覆寫以添加特效或行為。
    /// </summary>
    public virtual void Cast(Transform casterTransform)
    {
        if (!IsReady)
        {
            Debug.LogWarning($"Skill {skillName} is not ready. Pool: {currentCooldownPool}/{maxCooldownPool} (needs {cooldownCostPerCast})");
            return;
        }

        // 消耗冷卻池的能量
        currentCooldownPool -= cooldownCostPerCast;
        currentCooldownPool = Mathf.Max(0f, currentCooldownPool);

        isCasting = true;         // 標記為施放中
        castTimer = castTime;     // 設定施放時間

        // 觸發事件（例如更新 UI 或其他系統）
        EventManager.TriggerEvent($"OnSkill{skillId}Cast");
        Debug.Log($"Casting {skillName}. Pool: {currentCooldownPool}/{maxCooldownPool}");
    }

    /// <summary>
    /// 每幀更新技能狀態，包含施放倒數與冷卻恢復。
    /// </summary>
    public virtual void UpdateSkill()
    {
        // 更新施放計時器
        if (isCasting)
        {
            castTimer -= Time.deltaTime;
            if (castTimer <= 0f)
            {
                OnCastComplete(); // 施放完成時呼叫
            }
        }

        // 冷卻池自動回復
        if (currentCooldownPool < maxCooldownPool)
        {
            currentCooldownPool += cooldownRegenRate * Time.deltaTime;
            if (currentCooldownPool > maxCooldownPool)
            {
                currentCooldownPool = maxCooldownPool;
            }
        }
    }

    /// <summary>
    /// 施放完成時觸發，可由子類別覆寫以產生技能效果（傷害、特效等）。
    /// </summary>
    protected virtual void OnCastComplete()
    {
        isCasting = false;
        EventManager.TriggerEvent($"OnSkill{skillId}CastComplete");
        Debug.Log($"{skillName} cast complete");
    }

    /// <summary>
    /// 重設冷卻池（如：休息點恢復技能）。
    /// </summary>
    public virtual void ResetCooldown()
    {
        currentCooldownPool = maxCooldownPool;
        isCasting = false;
        castTimer = 0f;
    }

    /// <summary>
    /// 根據升級類型調整技能屬性（由技能樹呼叫）。
    /// </summary>
    public virtual void ApplyUpgrade(string upgradeType, float value)
    {
        switch (upgradeType)
        {
            case "IncreaseMaxPool":
                maxCooldownPool += value;
                currentCooldownPool += value; // 同步增加當前值
                break;
            case "ReduceCost":
                cooldownCostPerCast -= value;
                cooldownCostPerCast = Mathf.Max(1f, cooldownCostPerCast); // 最低1
                break;
            case "IncreaseRegenRate":
                cooldownRegenRate += value;
                break;
            case "IncreaseDamage":
                baseDamage += value;
                break;
            case "ReduceCastTime":
                castTime -= value;
                castTime = Mathf.Max(0.1f, castTime);
                break;
            default:
                Debug.LogWarning($"Unknown upgrade type: {upgradeType}");
                break;
        }
    }

    /// <summary>
    /// 取得當前冷卻池百分比，用於顯示 UI。
    /// </summary>
    public float GetPoolPercentage()
    {
        return currentCooldownPool / maxCooldownPool;
    }
}
