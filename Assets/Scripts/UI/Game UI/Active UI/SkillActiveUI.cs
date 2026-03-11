using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillActiveUI : MonoBehaviour
{
    [SerializeField]
    private Image _skillHolder;
    [SerializeField]
    private Image _skillTimer;

    /// <summary>
    /// _skillSprites[0]: inactive sprite (not ready to use)
    /// _skillSprites[1]: active sprite (ready to use)
    /// </summary>
    [SerializeField]
    private Sprite[] _skillSprites;

    private BaseSkill _skill;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if(_skill == null)
        {
            Debug.Log("Skill is NULL");
            return;
        }
        float time = _skill.CurrentCooldownPool;
        float totalDuration = _skill.MaxCooldownPool;
        _skillTimer.fillAmount = time/totalDuration;
        if (!_skill.IsReady) _skillHolder.sprite = _skillSprites[0];
        else _skillHolder.sprite = _skillSprites[1];
    }

    public void AssignSkill(BaseSkill skill)
    {
        _skill = skill;
    }
}
