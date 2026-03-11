using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData : ScriptableObject
{
    [SerializeField]
    private string skillName;

    [SerializeField]
    private SkillData[] children;

    [SerializeField]
    private bool unlocked = false;

    public string Skillname => skillName;
    public SkillData[] Children => children;
    public bool Unlocked => unlocked;

    public void SetUnlocked(bool unlocked)
    {
        this.unlocked = unlocked;
    }
}
