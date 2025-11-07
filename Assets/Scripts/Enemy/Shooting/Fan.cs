using UnityEngine;

public class FanShotAI : MonoBehaviour
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
    public GameObject explosionPrefab; // Drag your ExplosionPrefab here (passed to projectile)
    public float projectileSpeed = 20f; // Speed for all projectiles in the fan

    [Header("Fan Shot Specifics")]
    public int numProjectilesInFan = 5; // How many projectiles in one fan burst
    [Range(0, 180)] // Max 180 degrees for a reasonable fan
    public float totalFanAngle = 60f; // The total angle across which projectiles are spread
    public float fanCooldown = 2f; // Time between each fan burst

    private Transform playerTransform;
    private bool playerInSight;
    private float nextFanTime;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("FanShotAI: Player not found! Make sure your player has the 'Player' tag.");
            enabled = false;
        }

        nextFanTime = Time.time; // Can fire immediately
    }

    void Update()
    {
        CheckForPlayer();

        if (playerInSight)
        {
            // Make the enemy look at the player (only yaw)
            Vector3 lookAtPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.LookAt(lookAtPosition);

            if (Time.time >= nextFanTime)
            {
                ShootFanBurst();
                nextFanTime = Time.time + fanCooldown;
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

    void ShootFanBurst()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("FanShotAI: Projectile Prefab or Fire Point not assigned!");
            return;
        }

        Debug.Log("Firing fan burst!");

        // Calculate the base direction towards the player
        Vector3 baseDirection = (playerTransform.position - firePoint.position).normalized;
        Quaternion baseRotation = Quaternion.LookRotation(baseDirection);

        // Calculate the starting angle for the fan (e.g., if totalFanAngle is 60, start at -30)
        float startAngle = -totalFanAngle / 2f;
        float angleStep = 0f;

        if (numProjectilesInFan > 1)
        {
            angleStep = totalFanAngle / (numProjectilesInFan - 1);
        }
        // If numProjectilesInFan is 1, angleStep remains 0, and it fires straight.

        for (int i = 0; i < numProjectilesInFan; i++)
        {
            float currentAngle = startAngle + i * angleStep;

            // Apply the fan angle relative to the base rotation
            Quaternion fanRotation = Quaternion.Euler(0, currentAngle, 0); // Only yaw for horizontal fan
            Quaternion finalProjectileRotation = baseRotation * fanRotation;

            Vector3 deviatedDirection = finalProjectileRotation * Vector3.forward;

            GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, finalProjectileRotation);
            Projectile projectileScript = projectileGO.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                projectileScript.speed = projectileSpeed; // Use the single speed
                projectileScript.SetDirection(deviatedDirection);
                projectileScript.explosionPrefab = explosionPrefab;
                projectileScript.hitLayers = playerLayer | obstructionLayer;
            }
            else
            {
                Debug.LogError("FanShotAI: Projectile Prefab is missing the Projectile script!");
            }
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

        // Draw fan lines
        if (firePoint != null && playerTransform != null)
        {
            Gizmos.color = Color.magenta;
            Vector3 baseDirection = (playerTransform.position - firePoint.position).normalized;
            Quaternion baseRotation = Quaternion.LookRotation(baseDirection);

            float startAngle = -totalFanAngle / 2f;
            float angleStep = 0f;
            if (numProjectilesInFan > 1)
            {
                angleStep = totalFanAngle / (numProjectilesInFan - 1);
            }

            for (int i = 0; i < numProjectilesInFan; i++)
            {
                float currentAngle = startAngle + i * angleStep;
                Quaternion fanRotation = Quaternion.Euler(0, currentAngle, 0);
                Quaternion finalProjectileRotation = baseRotation * fanRotation;
                Vector3 deviatedDirection = finalProjectileRotation * Vector3.forward;
                Gizmos.DrawRay(firePoint.position, deviatedDirection * sightRange);
            }
        }
    }
}
