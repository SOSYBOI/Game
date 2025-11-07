// Skill1.cs (Skill2 3 4 are similar)
using UnityEngine;

public class Skill1 : BaseSkill
{
    public Skill1()
    {
        skillName = "Fireball";
        description = "Launch a fireball at enemies";
        
        // Pool-based cooldown configuration
        maxCooldownPool = 40f;        // Total pool capacity
        cooldownCostPerCast = 10f;    // Cost per cast (can cast 4 times when full)
        cooldownRegenRate = 1f;       // Regenerates 1 point per second (10 seconds between casts)
        
        castTime = 1f;
        baseDamage = 10f;
        
        // Initialize pool to full
        currentCooldownPool = maxCooldownPool;
    }

    public override void Cast(Transform casterTransform)
    {
        base.Cast(casterTransform);
    }

    protected override void OnCastComplete()
    {
        base.OnCastComplete();
        
        Debug.Log("Fireball Effect Applied!");
        // Add your skill effect logic here
    }
}
