using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillTreeManager : MonoBehaviour 
{
    public static UnityAction<SkillNodeUI> OnSkillUpgrade; 

    private void Awake()
    {
        OnSkillUpgrade += UpdateSkill;
    }


    private void UpdateSkill(SkillNodeUI nodeUI)
    {
        int i = (int)nodeUI.SkillNode.SkillData.SkillType;
        BaseSkill skill = CooldownSystem.Instance.GetSkill(i);
        SkillUpgradeManager.Instance.ApplyUpgrade(skill,nodeUI.SkillNode.SkillData.UpgradeType,nodeUI.SkillNode.SkillData.Value);
        SkillUpgradeManager.Instance.RecordUpgrade(i, nodeUI.SkillNode.SkillData.UpgradeType);
    }

}
