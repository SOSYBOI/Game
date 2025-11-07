//PlayerTeleport.cs
using UnityEngine;
using System.Collections.Generic;

public class PlayerTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("Key to activate teleportation and show indicator.")]
    public KeyCode teleportKey = KeyCode.X;
    
    [Tooltip("Maximum horizontal distance to search for teleport locations.")]
    public float maxTeleportDistance = 10f;
    
    [Tooltip("Maximum height difference (up or down) for valid teleport spots.")]
    public float maxHeightDifference = 5f;
    
    [Tooltip("Width of the detection cone in front of the player.")]
    public float detectionWidth = 3f;
    
    [Tooltip("Layer mask for platforms/ground (use 'Wall' layer).")]
    public LayerMask groundLayer;
    
    [Tooltip("Minimum distance from ledge edge to place player.")]
    public float ledgeOffset = 0.5f;
    
    [Tooltip("Number of rays to cast for detection.")]
    public int rayCount = 15;
    
    [Tooltip("Height to check above player for clearance.")]
    public float clearanceHeight = 2f;

    [Header("Visual Feedback")]
    [Tooltip("Prefab to show teleport destination (optional).")]
    public GameObject teleportIndicatorPrefab;
    
    [Tooltip("VFX for teleport start (optional).")]
    public GameObject teleportStartVFX;
    
    [Tooltip("VFX for teleport end (optional).")]
    public GameObject teleportEndVFX;

    private CharacterController controller;
    private PlayerMovement playerMovement;
    private GameObject currentIndicator;
    private Vector3? validTeleportPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();

        if (controller == null)
        {
            Debug.LogError("PlayerTeleport requires a CharacterController component.");
            enabled = false;
            return;
        }

        if (groundLayer.value == 0)
        {
            Debug.LogWarning("Ground LayerMask is not set. Please assign it in the Inspector.");
        }

        // Create indicator if prefab is assigned
        if (teleportIndicatorPrefab != null)
        {
            currentIndicator = Instantiate(teleportIndicatorPrefab);
            currentIndicator.SetActive(false);
        }
    }

    void Update()
    {
        // Execute teleport on key release FIRST (before clearing the position)
        if (Input.GetKeyUp(teleportKey) && validTeleportPosition.HasValue)
        {
            ExecuteTeleport(validTeleportPosition.Value);
            validTeleportPosition = null; // Clear after teleporting
        }

        // Only find teleport position when holding X
        if (Input.GetKey(teleportKey))
        {
            validTeleportPosition = FindTeleportPosition();

            // Update indicator - only show when holding key
            if (currentIndicator != null)
            {
                if (validTeleportPosition.HasValue)
                {
                    currentIndicator.SetActive(true);
                    currentIndicator.transform.position = validTeleportPosition.Value;
                }
                else
                {
                    currentIndicator.SetActive(false);
                }
            }
        }
        else
        {
            // Hide indicator when key is not held (but don't clear position until after GetKeyUp check)
            if (currentIndicator != null)
            {
                currentIndicator.SetActive(false);
            }
            
            // Only clear position if we didn't just teleport
            if (!Input.GetKeyUp(teleportKey))
            {
                validTeleportPosition = null;
            }
        }
    }

    /// <summary>
    /// Finds the furthest valid teleport position in front of the player.
    /// </summary>
    Vector3? FindTeleportPosition()
    {
        Vector3 playerPosition = transform.position;
        Vector3 forwardDirection = transform.forward;
        
        List<TeleportCandidate> candidates = new List<TeleportCandidate>();

        // Cast multiple rays in a cone pattern
        for (int i = 0; i < rayCount; i++)
        {
            float progress = (float)i / (rayCount - 1);
            float horizontalDistance = progress * maxTeleportDistance;
            
            // Check center and sides
            Vector3[] offsets = new Vector3[]
            {
                Vector3.zero, // Center
                transform.right * detectionWidth * 0.5f, // Right
                -transform.right * detectionWidth * 0.5f // Left
            };

            foreach (Vector3 offset in offsets)
            {
                Vector3 checkPosition = playerPosition + forwardDirection * horizontalDistance + offset;
                
                // Cast ray downward from various heights
                for (float heightOffset = maxHeightDifference; heightOffset >= -maxHeightDifference; heightOffset -= 0.5f)
                {
                    Vector3 rayStart = checkPosition + Vector3.up * (controller.height + heightOffset);
                    RaycastHit hit;

                    if (Physics.Raycast(rayStart, Vector3.down, out hit, maxHeightDifference * 2 + controller.height, groundLayer))
                    {
                        Vector3 landingPosition = hit.point + Vector3.up * (controller.height / 2);
                        
                        // Validate the position (includes path check to prevent wall clipping)
                        if (IsValidTeleportPosition(landingPosition, hit))
                        {
                            float distance = Vector3.Distance(new Vector3(playerPosition.x, 0, playerPosition.z), 
                                                             new Vector3(landingPosition.x, 0, landingPosition.z));
                            
                            candidates.Add(new TeleportCandidate
                            {
                                position = landingPosition,
                                distance = distance,
                                hitInfo = hit
                            });
                        }
                        
                        break; // Found ground at this position, no need to check lower
                    }
                }
            }
        }

        // Return the furthest valid position
        if (candidates.Count > 0)
        {
            candidates.Sort((a, b) => b.distance.CompareTo(a.distance));
            Vector3 furthestPosition = candidates[0].position;
            
            // Adjust position if near ledge
            furthestPosition = AdjustForLedge(furthestPosition, candidates[0].hitInfo);
            
            return furthestPosition;
        }

        return null;
    }

    /// <summary>
    /// Validates if a position is suitable for teleportation.
    /// Includes line-of-sight check to prevent teleporting through walls.
    /// </summary>
    bool IsValidTeleportPosition(Vector3 position, RaycastHit groundHit)
    {
        // Check if position is too close to current position
        float horizontalDistance = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(position.x, 0, position.z)
        );
        
        if (horizontalDistance < 1f)
        {
            return false;
        }

        // --- Check for walls blocking the path ---
        // Use SphereCast to check if there's a wall between player and target position
        Vector3 startPos = transform.position + Vector3.up * (controller.height / 2);
        Vector3 endPos = position;
        Vector3 direction = (endPos - startPos).normalized;
        float pathDistance = Vector3.Distance(startPos, endPos);
        
        RaycastHit pathHit;
        // Cast a sphere along the path to detect walls
        if (Physics.SphereCast(startPos, controller.radius * 0.8f, direction, out pathHit, pathDistance, groundLayer))
        {
            // Check if we hit something other than the ground we're landing on
            if (pathHit.collider != groundHit.collider)
            {
                // Check if the hit is actually blocking (vertical wall vs walkable surface)
                if (Vector3.Dot(pathHit.normal, Vector3.up) < 0.7f)
                {
                    // This is a wall blocking the path
                    return false;
                }
            }
        }

        // --- Additional validation with multiple raycasts for better wall detection ---
        // Cast rays at different heights to ensure no walls are in the way
        for (float heightCheck = 0.2f; heightCheck <= controller.height; heightCheck += controller.height / 3f)
        {
            Vector3 rayStart = transform.position + Vector3.up * heightCheck;
            Vector3 rayEnd = position + Vector3.up * heightCheck;
            Vector3 rayDir = (rayEnd - rayStart).normalized;
            float rayDist = Vector3.Distance(rayStart, rayEnd);
            
            RaycastHit wallHit;
            if (Physics.Raycast(rayStart, rayDir, out wallHit, rayDist, groundLayer))
            {
                // If we hit something and it's not the landing surface
                if (wallHit.collider != groundHit.collider)
                {
                    // Check if it's a vertical surface (wall)
                    if (Vector3.Dot(wallHit.normal, Vector3.up) < 0.7f)
                    {
                        return false; // Wall is blocking
                    }
                }
            }
        }

        // Check if there's enough clearance above (no low ceiling)
        if (Physics.CheckSphere(position, controller.radius * 0.9f, groundLayer))
        {
            return false;
        }

        // Check overhead clearance
        RaycastHit ceilingHit;
        if (Physics.Raycast(position, Vector3.up, out ceilingHit, clearanceHeight, groundLayer))
        {
            if (ceilingHit.distance < controller.height)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Adjusts position if the player would land near a ledge (implements image 4 behavior).
    /// </summary>
    Vector3 AdjustForLedge(Vector3 position, RaycastHit groundHit)
    {
        Vector3 forwardDir = transform.forward;
        
        // Check if there's a ledge ahead
        RaycastHit forwardHit;
        Vector3 forwardCheckStart = position + Vector3.up * 0.1f + forwardDir * ledgeOffset;
        
        if (!Physics.Raycast(forwardCheckStart, Vector3.down, out forwardHit, 1f, groundLayer))
        {
            // We're near a ledge, pull back slightly
            position -= forwardDir * ledgeOffset;
        }

        return position;
    }

    /// <summary>
    /// Executes the teleportation to the target position.
    /// </summary>
    void ExecuteTeleport(Vector3 targetPosition)
    {
        // Spawn start VFX
        if (teleportStartVFX != null)
        {
            Instantiate(teleportStartVFX, transform.position, Quaternion.identity);
        }

        // Disable character controller temporarily to set position
        controller.enabled = false;
        transform.position = targetPosition;
        controller.enabled = true;

        // Reset vertical velocity
        if (playerMovement != null)
        {
            playerMovement.ResetVerticalVelocity();
        }

        // Spawn end VFX
        if (teleportEndVFX != null)
        {
            Instantiate(teleportEndVFX, targetPosition, Quaternion.identity);
        }

        // Hide indicator after teleport
        if (currentIndicator != null)
        {
            currentIndicator.SetActive(false);
        }

        Debug.Log("Teleported to: " + targetPosition);
    }

    // Helper class to store teleport candidates
    private class TeleportCandidate
    {
        public Vector3 position;
        public float distance;
        public RaycastHit hitInfo;
    }

    // Visualize detection area in editor
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || !Input.GetKey(teleportKey))
            return;

        Gizmos.color = Color.cyan;
        Vector3 playerPos = transform.position;
        Vector3 forward = transform.forward;

        // Draw detection cone
        Vector3 leftBound = playerPos + forward * maxTeleportDistance - transform.right * detectionWidth * 0.5f;
        Vector3 rightBound = playerPos + forward * maxTeleportDistance + transform.right * detectionWidth * 0.5f;
        
        Gizmos.DrawLine(playerPos, leftBound);
        Gizmos.DrawLine(playerPos, rightBound);
        Gizmos.DrawLine(leftBound, rightBound);

        // Draw path to valid teleport position
        if (validTeleportPosition.HasValue)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(validTeleportPosition.Value, 0.5f);
            Gizmos.DrawLine(playerPos, validTeleportPosition.Value);
        }
    }
}
