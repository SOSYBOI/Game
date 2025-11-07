using UnityEngine;
using System.Collections;

public class SpiralShotAI : MonoBehaviour
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
    public float projectileSpeed = 20f; // Speed for all projectiles in the spiral
    public GameObject explosionPrefab; // Drag your ExplosionPrefab here (passed to projectile)

    [Header("Spiral Specifics")]
    public int numProjectilesInSpiral = 8; // How many projectiles in one spiral burst
    [Range(0, 360)]
    public float spiralAngleStep = 45f; // Angle between each projectile in the spiral (e.g., 45 for 8 projectiles)
    public float timeBetweenProjectilesInSpiral = 0.1f; // Delay between each projectile in the burst
    public float spiralBurstCooldown = 3f; // Time between the end of one spiral burst and the start of the next

    private Transform playerTransform;
    private bool playerInSight;
    private float nextSpiralBurstTime;
    private bool isBursting = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("SpiralShotAI: Player not found! Make sure your player has the 'Player' tag.");
            enabled = false;
        }

        nextSpiralBurstTime = Time.time; // Can burst immediately
    }

    void Update()
    {
        CheckForPlayer();

        if (playerInSight && !isBursting)
        {
            // Make the enemy look at the player (only yaw)
            Vector3 lookAtPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.LookAt(lookAtPosition);

            if (Time.time >= nextSpiralBurstTime)
            {
                StartCoroutine(ShootSpiralBurst());
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

    IEnumerator ShootSpiralBurst()
    {
        isBursting = true;
        Debug.Log("Starting spiral burst!");

        // Calculate the base direction towards the player ONCE for the entire burst.
        Vector3 baseDirection = (playerTransform.position - firePoint.position).normalized;
        Quaternion baseRotation = Quaternion.LookRotation(baseDirection);

        // Start angle for the spiral (can be randomized or fixed)
        float currentSpiralAngle = 0f; // Or Random.Range(0f, 360f); for varied starting points

        for (int i = 0; i < numProjectilesInSpiral; i++)
        {
            if (projectilePrefab == null || firePoint == null)
            {
                Debug.LogError("SpiralShotAI: Projectile Prefab or Fire Point not assigned!");
                isBursting = false;
                yield break;
            }

            // Apply the spiral angle relative to the base rotation
            Quaternion spiralRotation = Quaternion.Euler(0, currentSpiralAngle, 0); // Only yaw for horizontal spiral
            Quaternion finalProjectileRotation = baseRotation * spiralRotation;

            Vector3 deviatedDirection = finalProjectileRotation * Vector3.forward;

            GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, finalProjectileRotation);
            Projectile projectileScript = projectileGO.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                projectileScript.speed = projectileSpeed;
                projectileScript.SetDirection(deviatedDirection);
                projectileScript.explosionPrefab = explosionPrefab;
                projectileScript.hitLayers = playerLayer | obstructionLayer;
            }
            else
            {
                Debug.LogError("SpiralShotAI: Projectile Prefab is missing the Projectile script!");
            }

            currentSpiralAngle += spiralAngleStep; // Increment angle for the next projectile
            yield return new WaitForSeconds(timeBetweenProjectilesInSpiral);
        }

        Debug.Log("Spiral burst finished!");
        isBursting = false;
        nextSpiralBurstTime = Time.time + spiralBurstCooldown;
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

            // Draw spiral lines if in burst mode (for visualization)
            if (firePoint != null)
            {
                Gizmos.color = Color.cyan;
                Vector3 baseDirection = (playerTransform.position - firePoint.position).normalized;
                Quaternion baseRotation = Quaternion.LookRotation(baseDirection);

                float currentSpiralAngle = 0f;
                for (int i = 0; i < numProjectilesInSpiral; i++)
                {
                    Quaternion spiralRotation = Quaternion.Euler(0, currentSpiralAngle, 0);
                    Quaternion finalProjectileRotation = baseRotation * spiralRotation;
                    Vector3 deviatedDirection = finalProjectileRotation * Vector3.forward;
                    Gizmos.DrawRay(firePoint.position, deviatedDirection * sightRange);
                    currentSpiralAngle += spiralAngleStep;
                }
            }
        }
    }
}
