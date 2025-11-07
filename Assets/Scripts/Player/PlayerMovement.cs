//PlayerMovement.cs
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The speed at which the player moves horizontally.")]
    public float moveSpeed = 30f;
    [Tooltip("The rate at which gravity pulls the player down.")]
    public float gravity = 160f;
    [Tooltip("The speed at which the player rotates to face the movement direction.")]
    public float rotationSpeed = 10f; // NEW: Rotation speed setting

    [Header("Ledge Detection")]
    public float ledgeDetectHeight = 10f;
    public float ledgeDetectDistance = 0.05f;


    [Header("Component References")]
    // Reference to the CharacterController component
    private CharacterController controller;
    // Reference to the PlayerCombat script to check if dashing or locked on
    private PlayerCombat playerCombat;
    // Reference to the main camera's transform
    private Transform cameraTransform; 


    // Internal variable to store the current movement direction and speed
    private Vector3 moveDirection;


    void Start()
    {
        // Get the CharacterController component attached to this GameObject
        controller = GetComponent<CharacterController>();
        // Get the PlayerCombat component attached to this GameObject
        playerCombat = GetComponent<PlayerCombat>();
        // Get the main camera's transform. Ensure your camera has the "MainCamera" tag.
        cameraTransform = Camera.main.transform;


        // Ensure the CharacterController exists
        if (controller == null)
        {
            Debug.LogError("PlayerMovement requires a CharacterController component attached to the same GameObject.");
            enabled = false; 
            return;
        }
        // Ensure PlayerCombat exists
        if (playerCombat == null)
        {
            Debug.LogError("PlayerMovement requires a PlayerCombat component attached to the same GameObject.");
            enabled = false; 
            return;
        }
        // Ensure the main camera exists
        if (cameraTransform == null)
        {
            Debug.LogError("PlayerMovement requires a main camera in the scene with the 'MainCamera' tag.");
            enabled = false; 
            return;
        }
    }


    void Update()
    {
        // If PlayerCombat is currently dashing, skip all movement and rotation.
        if (playerCombat.IsDashing)
        {
            return; 
        }


        // --- 1. Handle Horizontal Input ---
        float horizontalInput = Input.GetAxisRaw("Horizontal"); 
        float verticalInput = Input.GetAxisRaw("Vertical");     


        // --- 2. Calculate Camera-Relative Movement Vectors ---
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;


        cameraForward.y = 0;
        cameraRight.y = 0;


        cameraForward.Normalize();
        cameraRight.Normalize();


        // --- 3. Calculate Desired Movement Direction ---
        Vector3 desiredHorizontalMovement = (cameraForward * verticalInput) + (cameraRight * horizontalInput);


        // --- 4. Handle Rotation (Only if NOT locked on) ---
        if (!playerCombat.IsLockedOn)
        {
            if (desiredHorizontalMovement.magnitude > 0.1f)
            {
                // Calculate the target rotation based on the horizontal movement vector
                Quaternion targetRotation = Quaternion.LookRotation(desiredHorizontalMovement);


                // Smoothly rotate the player towards the target rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        // Note: If playerCombat.IsLockedOn is true, PlayerCombat handles the rotation.


        // --- 5. Apply Speed and Prepare for CharacterController.Move ---
        if (desiredHorizontalMovement.magnitude > 0.1f)
        {
            desiredHorizontalMovement.Normalize();
            desiredHorizontalMovement *= moveSpeed;
        }
        else
        {
            desiredHorizontalMovement = Vector3.zero; // No input, no horizontal movement
        }


        // --- 6. Edge protection ---
        bool canMove = true;
        
        if (desiredHorizontalMovement.magnitude > 0.1f) 
        {
            Vector3 raycastOrigin = transform.position + (desiredHorizontalMovement * ledgeDetectDistance) + (Vector3.up * 0.5f);

            // Visualize the ray in the Scene view for easy debugging
            Debug.DrawRay(raycastOrigin, Vector3.down * ledgeDetectHeight, Color.red);

            // Perform the raycast downwards. If it doesn't hit anything, there's a ledge.
            if (!Physics.Raycast(raycastOrigin, Vector3.down, ledgeDetectHeight))
            {
                canMove = false; // There is no ground ahead, so we can't move.
            }
        }

        // --- 7. Reapply Vertical Velocity (Gravity) ---
        float verticalVelocity = moveDirection.y; 
        moveDirection = new Vector3(desiredHorizontalMovement.x, verticalVelocity, desiredHorizontalMovement.z);


        // --- 8. Apply Gravity ---
        if (controller.isGrounded == false)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = -0.5f;
        }


        // --- 9. Move the CharacterController ---
        // If canMove is false, nullify the horizontal movement but keep the vertical (gravity).
        if (!canMove)
        {
            moveDirection.x = 0f;
            moveDirection.z = 0f;
        }
        
        controller.Move(moveDirection * Time.deltaTime);
    }


    /// <summary>
    /// Resets the player's vertical velocity to a grounded state.
    /// </summary>
    public void ResetVerticalVelocity()
    {
        moveDirection.y = -0.5f;
    }
}