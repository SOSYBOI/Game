using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection Settings")]
    public float sightRange = 20f; // How far the enemy can see
    [Range(0, 360)]
    public float fieldOfViewAngle = 90f; // Total angle of the enemy's vision cone
    public LayerMask playerLayer; // Assign the 'Player' layer in Inspector
    public LayerMask obstructionLayer; // Assign 'Wall' layer in Inspector (for blocking sight)

    [Header("Shooting Settings")]
    public GameObject projectilePrefab; // Drag your ProjectilePrefab here
    public Transform firePoint; // An empty GameObject child of the enemy, where projectiles spawn
    public float shootRate = 1f; // Time in seconds between shots
    [Range(0, 30)] // Max 30 degrees of spread for reasonable gameplay
    public float angleSpread = 5f; // Max angle deviation in degrees for shooting randomness
    public GameObject explosionPrefab; // Drag your ExplosionPrefab here (passed to projectile)

    private Transform playerTransform;
    private bool playerInSight;
    private float nextShootTime;

    void Start()
    {
        // Find the player by tag. Make sure your player GameObject has the "Player" tag.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("EnemyAI: Player not found! Make sure your player has the 'Player' tag.");
            enabled = false; // Disable script if no player to prevent errors
        }

        nextShootTime = Time.time; // Enemy can shoot immediately
    }

    void Update()
    {
        CheckForPlayer();

        if (playerInSight)
        {
            // Make the enemy look at the player (only yaw, so it doesn't tilt up/down)
            Vector3 lookAtPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.LookAt(lookAtPosition);

            if (Time.time >= nextShootTime)
            {
                ShootProjectile();
                nextShootTime = Time.time + shootRate;
            }
        }
    }

    void CheckForPlayer()
    {
        playerInSight = false; // Reset each frame

        if (playerTransform == null) return;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // 1. Is player within sight range?
        if (Vector3.Distance(transform.position, playerTransform.position) <= sightRange)
        {
            // 2. Is player within field of view?
            if (angleToPlayer < fieldOfViewAngle / 2f)
            {
                // 3. Is there an obstruction between enemy and player?
                // We raycast to the player, ignoring the enemy's own layer.
                // The raycast will hit either the player or an obstruction.
                RaycastHit hit;
                // Ensure the raycast starts slightly above the ground to avoid hitting self or ground immediately
                Vector3 rayOrigin = transform.position + Vector3.up * 0.5f; // Adjust 0.5f as needed
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

    void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("EnemyAI: Projectile Prefab or Fire Point not assigned!");
            return;
        }

        // Calculate the base direction towards the player
        Vector3 baseDirection = (playerTransform.position - firePoint.position).normalized;

        // Apply random angle spread
        // Generate random angles for pitch (X-axis) and yaw (Y-axis) relative to the base direction
        float randomAngleX = 0f;
        float randomAngleY = Random.Range(-angleSpread, angleSpread);

        // Create a rotation that points towards the player
        Quaternion targetRotation = Quaternion.LookRotation(baseDirection);

        // Apply the random spread relative to the target rotation
        // Quaternion.Euler(pitch, yaw, roll)
        Quaternion spreadRotation = Quaternion.Euler(randomAngleX, randomAngleY, 0);
        Quaternion finalProjectileRotation = targetRotation * spreadRotation;

        // The projectile's initial forward direction will be this final rotation's forward vector
        Vector3 deviatedDirection = finalProjectileRotation * Vector3.forward;

        // Instantiate the projectile at the fire point's position and the calculated rotation
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, finalProjectileRotation);
        Projectile projectileScript = projectileGO.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            // Pass the deviated direction and explosion prefab to the projectile
            projectileScript.SetDirection(deviatedDirection); // Now passing a direction
            projectileScript.explosionPrefab = explosionPrefab;
            projectileScript.hitLayers = playerLayer | obstructionLayer; // Tell projectile what to hit
        }
        else
        {
            Debug.LogError("EnemyAI: Projectile Prefab is missing the Projectile script!");
        }
    }

    // --- Gizmos for Debugging in Editor ---
    void OnDrawGizmosSelected()
    {
        // Sight Range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Field of View
        Vector3 fovLine1 = Quaternion.Euler(0, fieldOfViewAngle / 2, 0) * transform.forward * sightRange;
        Vector3 fovLine2 = Quaternion.Euler(0, -fieldOfViewAngle / 2, 0) * transform.forward * sightRange;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        // Line to player if in sight
        if (playerInSight && playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
}
