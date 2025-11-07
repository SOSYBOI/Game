//PlayerCombat.cs
using UnityEngine;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    [Header("Component References")]
    // Reference to the CharacterController component (needed for dash movement)
    private CharacterController controller;
    // Reference to PlayerMovement to reset vertical velocity after dash
    private PlayerMovement playerMovement;

    [Header("Lock-On Settings")]
    [Tooltip("Prefab for the lock-on indicator sphere.")]
    public GameObject lockOnIndicatorPrefab;
    [Tooltip("Maximum distance to find enemies for lock-on.")]
    public float lockOnRange = 25f;
    [Tooltip("Layer containing enemy GameObjects for lock-on detection.")]
    public LayerMask enemyLayer; // Used for OverlapSphere (lock-on)

    private GameObject lockedEnemy;
    private GameObject currentLockOnIndicator; // Instance of the indicator prefab

    [Header("Dash Settings")]
    [Tooltip("Distance the player will dash towards the locked enemy.")]
    public float dashDistance = 5f; // Note: Actual distance will be dashSpeed * dashDuration
    [Tooltip("Speed of the dash.")]
    public float dashSpeed = 20f;
    [Tooltip("Duration of the dash in seconds.")]
    public float dashDuration = 0.2f;
    [Tooltip("Minimum distance to the enemy to stop the dash early (prevents overshooting).")]
    public float dashStopDistance = 1f;

    private bool isDashing = false;
    private Vector3 dashDirection;
    private float dashTimer;

    // Public property to let other scripts know if the player is dashing
    public bool IsDashing => isDashing;
    
    // NEW: Public property to let other scripts know if the player is locked on
    public bool IsLockedOn => lockedEnemy != null;

    [Header("VFX Settings")] // New header for VFX
    [Tooltip("Prefab for the slash VFX that spawns after a dash.")]
    public GameObject slashVFXPrefab;


    void Start()
    {
        // Get the CharacterController component attached to this GameObject
        controller = GetComponent<CharacterController>();
        // Get the PlayerMovement component attached to this GameObject
        playerMovement = GetComponent<PlayerMovement>();

        // Ensure components exist
        if (controller == null)
        {
            Debug.LogError("PlayerCombat requires a CharacterController component attached to the same GameObject.");
            enabled = false;
            return;
        }
        if (playerMovement == null)
        {
            Debug.LogError("PlayerCombat requires a PlayerMovement component attached to the same GameObject.");
            enabled = false;
            return;
        }

        // Initialize lock-on indicator
        if (lockOnIndicatorPrefab != null)
        {
            currentLockOnIndicator = Instantiate(lockOnIndicatorPrefab);
            currentLockOnIndicator.SetActive(false); // Start disabled
        }
        else
        {
            Debug.LogWarning("Lock-On Indicator Prefab is not assigned. Lock-on visual will not work.");
        }

        // Ensure LayerMasks are set up in the Inspector
        if (enemyLayer.value == 0)
        {
            Debug.LogWarning("Enemy LayerMask for Lock-On is not set. Lock-on might not find enemies correctly. Please set the 'Enemy Layer' in the Inspector.");
        }
    }

    void Update()
    {
        // --- Handle Lock-On Input ---
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            FindAndLockEnemy();
        }

        // --- Update Lock-On Indicator & Rotation ---
        if (lockedEnemy != null)
        {
            // Check if locked enemy is still valid (active, not destroyed) and on screen
            if (lockedEnemy.activeInHierarchy && IsTargetOnScreen(lockedEnemy.transform))
            {
                if (currentLockOnIndicator != null)
                {
                    currentLockOnIndicator.SetActive(true);
                    // Position the indicator slightly above the enemy or at its center
                    currentLockOnIndicator.transform.position = lockedEnemy.transform.position + Vector3.up * 0.5f; 
                }
                
                // Player Rotation Logic (Facing the Locked Enemy)
                FaceLockedEnemy();
            }
            else
            {
                ClearLock(); // Enemy no longer valid or off-screen
            }
        }
        else
        {
            // No enemy locked, ensure indicator is off
            if (currentLockOnIndicator != null)
            {
                currentLockOnIndicator.SetActive(false);
            }
        }

        // --- Handle Dash Input ---
        // Only allow dash if Z is pressed, an enemy is locked, and player is not already dashing
        if (Input.GetKeyDown(KeyCode.Z) && lockedEnemy != null && !isDashing)
        {
            DashToLockedEnemy();
        }

        // --- Handle Dashing State ---
        if (isDashing)
        {
            // Apply dash movement
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;

            // Check if dash is complete or player is very close to target
            if (dashTimer <= 0f || (lockedEnemy != null && Vector3.Distance(transform.position, lockedEnemy.transform.position) < dashStopDistance))
            {
                isDashing = false;
                // Re-ground the player after dash to prevent floating
                playerMovement.ResetVerticalVelocity();

                // --- Spawn Slash VFX after dash ---
                if (slashVFXPrefab != null)
                {
                    GameObject spawnedVFX = Instantiate(slashVFXPrefab, transform.position, transform.rotation);
                }
                else
                {
                    Debug.LogWarning("Slash VFX Prefab is not assigned. No slash effect will play after dash.");
                }
            }
        }
    }

    /// <summary>
    /// Rotates the player to face the currently locked enemy horizontally.
    /// </summary>
    void FaceLockedEnemy()
    {
        if (lockedEnemy == null) return;

        Vector3 directionToTarget = lockedEnemy.transform.position - transform.position;
        
        // Ignore vertical component (Y-axis)
        directionToTarget.y = 0;

        if (directionToTarget.sqrMagnitude > 0.01f) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            // Use Slerp for smoother rotation if desired, or instant rotation for tight lock-on feel
            transform.rotation = targetRotation; 
        }
    }

    /// <summary>
    /// Finds the closest enemy within range and on screen, and locks onto it.
    /// </summary>
    void FindAndLockEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, lockOnRange, enemyLayer);
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            GameObject enemy = hitCollider.gameObject;

            // Check if the enemy is active and currently visible on the screen
            if (enemy.activeInHierarchy && IsTargetOnScreen(enemy.transform))
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        if (closestEnemy != null)
        {
            if (lockedEnemy == closestEnemy)
            {
                // If player presses Shift again on the same locked enemy, unlock it (toggle)
                ClearLock();
            }
            else
            {
                // Lock onto the new closest enemy
                lockedEnemy = closestEnemy;
                Debug.Log("Locked onto: " + lockedEnemy.name);
            }
        }
        else
        {
            // No enemy found or visible, clear any existing lock
            ClearLock();
            Debug.Log("No enemy found or visible to lock onto.");
        }
    }

    /// <summary>
    /// Clears the current lock-on target and disables the indicator.
    /// </summary>
    void ClearLock()
    {
        if (lockedEnemy != null)
        {
            Debug.Log("Lock cleared from: " + lockedEnemy.name);
            lockedEnemy = null;
        }
        if (currentLockOnIndicator != null)
        {
            currentLockOnIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Initiates a dash towards the currently locked enemy.
    /// </summary>
    void DashToLockedEnemy()
    {
        if (lockedEnemy == null)
        {
            Debug.LogWarning("Cannot dash: No enemy is locked.");
            return;
        }

        // Calculate dash direction towards the enemy
        dashDirection = (lockedEnemy.transform.position - transform.position).normalized;

        // Ensure player is facing the enemy at the start of the dash
        Vector3 lookAtTarget = lockedEnemy.transform.position;
        lookAtTarget.y = transform.position.y; 
        transform.LookAt(lookAtTarget);

        isDashing = true;
        dashTimer = dashDuration;
        Debug.Log("Dashing towards: " + lockedEnemy.name);
    }

    /// <summary>
    /// Checks if a target transform is currently visible on the main camera's screen.
    /// </summary>
    bool IsTargetOnScreen(Transform target)
    {
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera not found. Please ensure your camera is tagged 'MainCamera'.");
            return false;
        }

        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(target.position);

        // Check if the target is within the viewport (0 to 1 for x and y) and in front of the camera (z > 0)
        return viewportPoint.x > 0 && viewportPoint.x < 1 &&
               viewportPoint.y > 0 && viewportPoint.y < 1 &&
               viewportPoint.z > 0;
    }

    // Optional: Draw lock-on range in editor for visualization
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lockOnRange);
    }
}
