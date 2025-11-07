using UnityEngine;

public class HomingShotAI : MonoBehaviour
{
    [Header("Detection Settings")]
    public float sightRange = 20f;
    [Range(0, 360)]
    public float fieldOfViewAngle = 90f;
    public LayerMask playerLayer;
    public LayerMask obstructionLayer;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab; // Drag your ProjectilePrefab here
    public Transform firePoint; // An empty GameObject child of the enemy, where projectiles spawn
    public float shootRate = 3f; // Time in seconds between homing shots
    public float projectileSpeed = 15f; // Initial speed of the homing projectile
    public float homingTurnSpeed = 5f; // How quickly the projectile can turn to home in
    public float homingDuration = 3f; // How long the projectile will home before flying straight
    public GameObject explosionPrefab; // Drag your ExplosionPrefab here (passed to projectile)

    private Transform playerTransform;
    private bool playerInSight;
    private float nextShootTime;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("HomingShotAI: Player not found! Make sure your player has the 'Player' tag.");
            enabled = false;
        }

        nextShootTime = Time.time; // Can shoot immediately
    }

    void Update()
    {
        CheckForPlayer();

        if (playerInSight)
        {
            // Make the enemy look at the player (only yaw)
            Vector3 lookAtPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.LookAt(lookAtPosition);

            if (Time.time >= nextShootTime)
            {
                ShootHomingProjectile();
                nextShootTime = Time.time + shootRate;
            }
        }
    }

    void CheckForPlayer()
    {
        playerInSight = false;

        if (playerTransform == null) return;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (Vector3.Distance(transform.position, playerTransform.position) <= sightRange)
        {
            if (angleToPlayer < fieldOfViewAngle / 2f)
            {
                RaycastHit hit;
                Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
                if (Physics.Raycast(rayOrigin, directionToPlayer, out hit, sightRange, playerLayer | obstructionLayer))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        playerInSight = true;
                    }
                }
            }
        }
    }

    void ShootHomingProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("HomingShotAI: Projectile Prefab or Fire Point not assigned!");
            return;
        }

        Debug.Log("Firing homing projectile!");

        // Instantiate the projectile
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        HomingProjectile homingProjectileScript = projectileGO.GetComponent<HomingProjectile>();

        if (homingProjectileScript != null)
        {
            homingProjectileScript.speed = projectileSpeed;
            homingProjectileScript.turnSpeed = homingTurnSpeed;
            homingProjectileScript.homingDuration = homingDuration;
            homingProjectileScript.SetTarget(playerTransform); // Pass the player's transform for homing
            homingProjectileScript.explosionPrefab = explosionPrefab;
            homingProjectileScript.hitLayers = playerLayer | obstructionLayer;
        }
        else
        {
            Debug.LogError("HomingShotAI: Projectile Prefab is missing the HomingProjectile script!");
        }
    }

    // --- Gizmos for Debugging in Editor ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 fovLine1 = Quaternion.Euler(0, fieldOfViewAngle / 2, 0) * transform.forward * sightRange;
        Vector3 fovLine2 = Quaternion.Euler(0, -fieldOfViewAngle / 2, 0) * transform.forward * sightRange;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (playerInSight && playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
}

// --- REQUIRED NEW PROJECTILE SCRIPT FOR HOMING ---
// You will need to create a new C# script named HomingProjectile.cs
// and attach it to your projectilePrefab instead of the basic Projectile.cs
public class HomingProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float turnSpeed = 5f; // How quickly it turns towards the target
    public float homingDuration = 3f; // How long it will home before flying straight
    public float lifetime = 5f;
    public GameObject explosionPrefab;
    public LayerMask hitLayers;

    private Transform target;
    private float homingTimer;
    private bool isHoming = true;

    void Start()
    {
        Destroy(gameObject, lifetime);
        homingTimer = homingDuration;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (isHoming && target != null)
        {
            homingTimer -= Time.deltaTime;
            if (homingTimer <= 0)
            {
                isHoming = false; // Stop homing after duration
            }
            else
            {
                // Calculate direction to target
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                // Smoothly rotate towards the target
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }

        // Move forward based on current rotation
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other) // Or OnCollisionEnter
    {
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            // Add damage logic here
            Destroy(gameObject);
        }
    }
}
