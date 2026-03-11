using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillNode
{
    [SerializeField]private SkillUpgradeData _skillData;
    [SerializeField]private SkillNodeUI parentNode;
    [SerializeField]private List<SkillNodeUI> children;


    public SkillUpgradeData SkillData => _skillData;
    public SkillNodeUI ParentNode => parentNode;
    public List<SkillNodeUI> Children => children;
    public void ApplyUpgrade()
    {
        if (!_skillData.IsLocked)
        {
            Debug.Log("Skill has already been upgraded");
            return;
        }

        _skillData.SetLocked(false);
        if (_skillData.Unlockable && !_skillData.IsLocked)
        {
            foreach (SkillNodeUI nodeUI in children)
            {
                nodeUI.SkillNode.SetUnlockable(true);
                nodeUI.UpdateUpgrade();
            }
        }
    }

    private void SetUnlockable(bool unlockable)
    {
        //this.unlockable = unlockable;
    }

}

public enum SkillType { 
    FIREBALL,
    SKILL2,
    SKILL3,
    SKILL4
}
