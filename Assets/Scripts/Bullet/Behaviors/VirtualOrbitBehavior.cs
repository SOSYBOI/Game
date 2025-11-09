// Assets/Scripts/Bullet/Behaviors/VirtualOrbitBehavior.cs
using UnityEngine;

/// <summary>
/// 圍繞虛擬中心點旋轉的行為（中心點會隨時間移動）。
/// 適合製作螺旋彈幕、環繞攻擊等效果。
/// </summary>
public class VirtualOrbitBehavior : IBulletBehavior
{
    private Vector3 virtualCenterPosition;   // 虛擬中心點位置
    private Vector3 centerVelocity;          // 中心點移動速度
    private float currentRadius;             // 當前軌道半徑
    private float radiusGrowthRate;          // 半徑增長速率
    private float rotationSpeed;             // 旋轉速度（度/秒）
    private float currentAngle;              // 當前角度
    
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
        
        return true;  // 持續執行
    }
    
    /// <summary>
    /// 根據當前角度與半徑計算子彈位置。
    /// </summary>
    private void UpdateBulletPosition(Bullet bullet)
    {
        Vector3 offset = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * currentRadius;
        bullet.transform.position = virtualCenterPosition + offset;
        
        // 讓 Rigidbody 的速度跟上位置變化（避免物理衝突）
        bullet.velocity = centerVelocity;
    }
    
    public void OnBehaviorEnd(Bullet bullet) { }
}
