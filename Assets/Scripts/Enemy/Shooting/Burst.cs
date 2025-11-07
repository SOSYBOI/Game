using UnityEngine;
using System.Collections; // Required for Coroutines

public class BurstShooterAI : MonoBehaviour
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
    [Range(1, 30)] // Added a range for better control in Inspector
    public int projectilesPerBurst = 5; // NEW: Public variable for number of projectiles per burst
    public float burstCooldown = 5f; // Time between the end of one burst and the start of the next
    public float timeBetweenProjectilesInBurst = 1f; // Cooldown between each projectile in the burst
    [Range(0, 30)]
    public float angleSpread = 5f; // Max angle deviation in degrees for shooting randomness (Y-axis only)

    // The speeds for the projectiles. This array will be dynamically handled based on projectilesPerBurst.
    public float[] projectileSpeeds = { 20f, 18f, 16f, 14f, 12f };
    private const float DEFAULT_PROJECTILE_SPEED = 15f; // Fallback speed if projectileSpeeds array is insufficient

    private Transform playerTransform;
    private bool playerInSight;
    private float nextBurstTime;
    private bool isBursting = false; // Flag to prevent starting a new burst while one is active

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("BurstShooterAI: Player not found! Make sure your player has the 'Player' tag.");
            enabled = false;
            return; // Disable script if player isn't found
        }

        nextBurstTime = Time.time; // Can burst immediately

        // --- Handle projectileSpeeds array initialization and resizing ---
        if (projectileSpeeds == null || projectileSpeeds.Length == 0)
        {
            Debug.LogWarning($"BurstShooterAI: projectileSpeeds array is null or empty. Initializing with {projectilesPerBurst} elements at {DEFAULT_PROJECTILE_SPEED}f.");
            projectileSpeeds = new float[projectilesPerBurst];
            for (int i = 0; i < projectilesPerBurst; i++)
            {
                projectileSpeeds[i] = DEFAULT_PROJECTILE_SPEED;
            }
        }
        else if (projectileSpeeds.Length < projectilesPerBurst)
        {
            Debug.LogWarning($"BurstShooterAI: projectileSpeeds array has {projectileSpeeds.Length} elements, but projectilesPerBurst is {projectilesPerBurst}. Resizing and filling with the last available speed or default.");
            float[] newSpeeds = new float[projectilesPerBurst];
            System.Array.Copy(projectileSpeeds, newSpeeds, projectileSpeeds.Length);
            // Fill the remaining elements with the last available speed, or a default if the original array was empty (handled above)
            float fillSpeed = projectileSpeeds[projectileSpeeds.Length - 1];
            for (int i = projectileSpeeds.Length; i < projectilesPerBurst; i++)
            {
                newSpeeds[i] = fillSpeed;
            }
            projectileSpeeds = newSpeeds;
        }
        // If projectileSpeeds.Length > projectilesPerBurst, we simply use the first 'projectilesPerBurst' elements, no action needed here.
    }

    void Update()
    {
        CheckForPlayer();

        if (playerInSight && !isBursting)
        {
            // Make the enemy look at the player (only yaw)
            Vector3 lookAtPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.LookAt(lookAtPosition);

            if (Time.time >= nextBurstTime)
            {
                StartCoroutine(ShootBurst());
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
                Vector3 rayOrigin = transform.position + Vector3.up * 0.5f; // Offset origin slightly to avoid self-collision
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

    IEnumerator ShootBurst()
    {
        isBursting = true; // Set flag to true
        Debug.Log("Starting burst!");

        for (int i = 0; i < projectilesPerBurst; i++) // Loop uses the new 'projectilesPerBurst' variable
        {
            if (projectilePrefab == null || firePoint == null)
            {
                Debug.LogError("BurstShooterAI: Projectile Prefab or Fire Point not assigned!");
                isBursting = false; // Reset flag
                yield break; // Exit coroutine
            }

            // Calculate the base direction towards the player
            Vector3 baseDirection = (playerTransform.position - firePoint.position).normalized;

            // Apply random angle spread ONLY on the Y-axis (yaw)
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
                // Set the specific speed for this projectile in the burst
                // Use the speed from the array if available, otherwise use the last speed or a default.
                float currentProjectileSpeed = (i < projectileSpeeds.Length && projectileSpeeds.Length > 0)
                                                ? projectileSpeeds[i]
                                                : (projectileSpeeds.Length > 0 ? projectileSpeeds[projectileSpeeds.Length - 1] : DEFAULT_PROJECTILE_SPEED);

                projectileScript.speed = currentProjectileSpeed;
                projectileScript.SetDirection(deviatedDirection);
                projectileScript.explosionPrefab = explosionPrefab;
                projectileScript.hitLayers = playerLayer | obstructionLayer;
            }
            else
            {
                Debug.LogError("BurstShooterAI: Projectile Prefab is missing the Projectile script!");
            }

            // Wait for the cooldown between individual projectiles
            yield return new WaitForSeconds(timeBetweenProjectilesInBurst);
        }

        Debug.Log("Burst finished!");
        isBursting = false; // Reset flag after burst
        nextBurstTime = Time.time + burstCooldown; // Set cooldown for the next burst
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
