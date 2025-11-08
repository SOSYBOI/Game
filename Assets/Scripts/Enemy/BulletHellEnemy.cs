// Assets/Scripts/Enemy/BulletHellEnemy.cs
using UnityEngine;

public class BulletHellEnemy : BaseEnemy
{
    private float nextBulletTime = 0f;
    private int bulletsFired = 0;
    
    protected override void ExecuteSpellCard()
    {
        if (Time.time >= nextBulletTime)
        {
            FireCircleFormationWithVirtualCenter();
            bulletsFired++;
            nextBulletTime = Time.time + 0.6f;  // 每 2 秒發射一次圓形陣型
        }
    }
    
    protected override bool IsSpellCardFinished()
    {
        return bulletsFired >= 5;  // 發射 5 次後結束
    }
    
    protected override void OnAttackEnd()
    {
        base.OnAttackEnd();
        bulletsFired = 0;
        nextBulletTime = Time.time;
    }
    
    /// <summary>
    /// 發射圓形旋轉陣型。
    /// </summary>
    private void FireCircleFormationWithVirtualCenter()
    {
        if (playerTransform == null)
            return;
        
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 centerVelocity = directionToPlayer * 25f;
        Vector3 centerStartPos = transform.position + Vector3.up * 0.5f;
        
        int bulletCount = 6;
        float angleStep = 360f / bulletCount;
        
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            
            BulletManager.Instance.SpawnBullet(
                centerStartPos,  // 所有彈幕從中心點創建
                new VirtualOrbitBehavior(
                    startCenterPos: centerStartPos,
                    centerMoveVelocity: centerVelocity,
                    initialRadius: 1f,
                    radiusGrowth: 12f,
                    rotSpeed: 100f,
                    startAngle: angle
                )
            );
        }
    }
}
