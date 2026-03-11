using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillUpgradeData", menuName = "Skill/New SkillUpgrade")]
public class SkillUpgradeData : ScriptableObject
{
    [SerializeField]
    private string upgradeName;

    [SerializeField]
    private string upgradeType; //This needs to be changed in the future.

    [SerializeField]
    private string description;

    [SerializeField]
    private Sprite upgradeSprite;

    [SerializeField] 
    private SkillType skillType = SkillType.FIREBALL;

    [SerializeField] 
    private float value;

    [SerializeField] 
    private bool locked = true;

    [SerializeField] 
    private bool unlockable = false;

    public string UpgradeName => upgradeName;
    public string UpgradeType => upgradeType;
    public string Description => description;
    public Sprite UpgradeSprite => upgradeSprite;
    public SkillType SkillType => skillType;
    public float Value => value;
    public bool IsLocked => locked;
    public bool Unlockable => unlockable;

    public void SetLocked(bool locked)
    {
        this.locked = locked;
    }

    public void SetUnlockable(bool unlockable)
    {
        this.unlockable = unlockable;
    }
}
