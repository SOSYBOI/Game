using UnityEngine;

public class TopDownFollowCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The object the camera will follow (usually the player).")]
    public Transform target;

    [Header("Camera Position")]
    [Tooltip("The offset from the target's position.")]
    public Vector3 offset = new Vector3(0f, 10f, -5f); // Example: 10 units up, 5 units back
    [Tooltip("How quickly the camera moves to the target position.")]
    public float smoothSpeed = 5f;

    [Header("Camera Rotation")]
    [Tooltip("The rotation the camera should maintain (e.g., looking down at a 45-degree angle).")]
    public Quaternion fixedRotation = Quaternion.Euler(45f, 0f, 0f); // 45 degrees pitch

    // --- Private Variables ---
    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;

    void Start()
    {
    	// Optional: Find the player automatically if the target isn't set in the Inspector
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
            else
            {
                Debug.LogError("Camera target (Player) not found! Please assign it or tag the player correctly.");
                enabled = false; // Disable the script if no target is found
            }
        }
        
        // Set the initial rotation
        transform.rotation = fixedRotation;
    }

    // LateUpdate is called after all Update functions have been called.
    void LateUpdate()
    {
        if (target == null) return;

        // 1. Calculate the desired position
        desiredPosition = target.position + offset;

        // 2. Smoothly move the camera towards the desired position
        smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 3. Ensure the camera maintains its fixed rotation (optional, but good for top-down)
        transform.rotation = fixedRotation;
    }
}
