// Assets/Scripts/Bullet/Behaviors/VirtualOrbitBehavior.cs
using UnityEngine;

/// <summary>
/// 圍繞虛擬中心點旋轉的行為（中心點會隨時間移動）。
/// </summary>
public class VirtualOrbitBehavior : IBulletBehavior
{
    private Vector3 virtualCenterPosition;
    private Vector3 centerVelocity;
    private float currentRadius;
    private float radiusGrowthRate;
    private float rotationSpeed;
    private float currentAngle;
    
    public VirtualOrbitBehavior(
        Vector3 startCenterPos,
        Vector3 centerMoveVelocity,
        float initialRadius,
        float radiusGrowth,
        float rotSpeed,
        float startAngle = 0f)
    {
        virtualCenterPosition = startCenterPos;
        centerVelocity = centerMoveVelocity;
        currentRadius = initialRadius;
        radiusGrowthRate = radiusGrowth;
        rotationSpeed = rotSpeed;
        currentAngle = startAngle;
    }
    
    public void Initialize(Bullet bullet)
    {
        UpdateBulletPosition(bullet);
    }
    
    public bool Update(Bullet bullet, float deltaTime)
    {
        // 移動虛擬中心點
        virtualCenterPosition += centerVelocity * deltaTime;
        
        // 增長半徑
        currentRadius += radiusGrowthRate * deltaTime;
        
        // 更新角度
        currentAngle += rotationSpeed * deltaTime;
        if (currentAngle >= 360f)
            currentAngle -= 360f;
        
        // 更新彈幕位置
        UpdateBulletPosition(bullet);
        
        return true;
    }
    
    private void UpdateBulletPosition(Bullet bullet)
    {
        Vector3 offset = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * currentRadius;
        bullet.transform.position = virtualCenterPosition + offset;
        
        // 讓 Rigidbody 的速度跟上位置變化（避免物理衝突）
        bullet.velocity = centerVelocity;
    }
    
    public void OnBehaviorEnd(Bullet bullet) { }
}
