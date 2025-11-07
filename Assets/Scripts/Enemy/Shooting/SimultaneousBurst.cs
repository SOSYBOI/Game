using UnityEngine;

public class SimultaneousBurstAI : MonoBehaviour
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

    [Header("Burst Specifics")]
    // This cooldown applies to the entire burst. After 5 projectiles fire simultaneously,
    // the enemy waits this long before it can fire another burst.
    public float burstCooldown = 1f; // Time between simultaneous bursts
    [Range(0, 30)]
    public float angleSpread = 5f; // Max angle deviation in degrees for shooting randomness (Y-axis only)

    // The speeds for the 5 projectiles, corresponding to their order of instantiation
    public float[] projectileSpeeds = { 20f, 18f, 16f, 14f, 12f };
    private const int BURST_PROJECTILE_COUNT = 5; // Fixed number of projectiles in a burst

    private Transform playerTransform;
    private bool playerInSight;
    private float nextBurstTime;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("SimultaneousBurstAI: Player not found! Make sure your player has the 'Player' tag.");
            enabled = false;
        }

        nextBurstTime = Time.time; // Can burst immediately
        // Ensure the projectileSpeeds array is correctly sized
        if (projectileSpeeds.Length != BURST_PROJECTILE_COUNT)
        {
            Debug.LogWarning($"SimultaneousBurstAI: projectileSpeeds array should have {BURST_PROJECTILE_COUNT} elements. Resizing to default values.");
            projectileSpeeds = new float[] { 20f, 18f, 16f, 14f, 12f };
        }
    }

    void Update()
    {
        CheckForPlayer();

        if (playerInSight)
        {
            // Make the enemy look at the player (only yaw)
            Vector3 lookAtPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.LookAt(lookAtPosition);

            if (Time.time >= nextBurstTime)
            {
                ShootSimultaneousBurst();
                // Set cooldown for the next burst immediately after firing
                nextBurstTime = Time.time + burstCooldown;
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

    void ShootSimultaneousBurst()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("SimultaneousBurstAI: Projectile Prefab or Fire Point not assigned!");
            return;
        }

        Debug.Log("Firing simultaneous burst!");

        // Calculate the base direction towards the player ONCE for the entire burst.
        // This ensures all projectiles are aimed generally at the player from the same moment.
        Vector3 baseDirection = (playerTransform.position - firePoint.position).normalized;

        for (int i = 0; i < BURST_PROJECTILE_COUNT; i++)
        {
            // Apply random angle spread ONLY on the Y-axis (yaw) for EACH projectile.
            // Each projectile gets its own random deviation.
            float randomAngleY = Random.Range(-angleSpread, angleSpread);
            Quaternion targetRotation = Quaternion.LookRotation(baseDirection);
            Quaternion spreadRotation = Quaternion.Euler(0, randomAngleY, 0); // X-axis (pitch) is 0
            Quaternion finalProjectileRotation = targetRotation * spreadRotation;
            Vector3 deviatedDirection = finalProjectileRotation * Vector3.forward;

            // Instantiate the projectile
            GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, finalProjectileRotation);
            Projectile projectileScript = projectileGO.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                // Set the specific speed for this projectile from the array
                projectileScript.speed = projectileSpeeds[i];
                projectileScript.SetDirection(deviatedDirection);
                projectileScript.explosionPrefab = explosionPrefab;
                projectileScript.hitLayers = playerLayer | obstructionLayer;
            }
            else
            {
                Debug.LogError("SimultaneousBurstAI: Projectile Prefab is missing the Projectile script!");
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
    }
}
