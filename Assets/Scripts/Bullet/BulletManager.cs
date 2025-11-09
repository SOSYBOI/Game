// Assets/Scripts/Bullet/BulletManager.cs
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance { get; private set; }
    
    [SerializeField] private GameObject bulletPrefab;  // 子彈預製物
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        if (bulletPrefab == null)
        {
            bulletPrefab = CreateDefaultBulletPrefab();
        }
    }
    
    /// <summary>
    /// 創建子彈並附加行為（可變參數）。
    /// </summary>
    public Bullet SpawnBullet(Vector3 position, params IBulletBehavior[] behaviors)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        
        if (bullet == null)
        {
            bullet = bulletObj.AddComponent<Bullet>();
        }
        
        // 為子彈添加所有行為
        foreach (var behavior in behaviors)
        {
            bullet.AddBehavior(behavior);
        }
        
        return bullet;
    }
    
    /// <summary>
    /// 創建預設子彈預製物（紅色球體）。
    /// </summary>
    private GameObject CreateDefaultBulletPrefab()
    {
        GameObject bullet = new GameObject("DefaultBullet");
        
        // 添加球體碰撞器
        SphereCollider collider = bullet.AddComponent<SphereCollider>();
        collider.radius = 0.3f;
        
        // 添加剛體
        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        // 添加球體網格
        MeshFilter meshFilter = bullet.AddComponent<MeshFilter>();
        meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        
        // 添加紅色材質
        MeshRenderer meshRenderer = bullet.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.red;
        meshRenderer.material = mat;
        
        return bullet;
    }
}
