using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f; // Speed of the projectile
    public float lifeTime = 5f; // How long the projectile exists before self-destructing
    [HideInInspector] public GameObject explosionPrefab; // Set by EnemyAI
    [HideInInspector] public LayerMask hitLayers; // Set by EnemyAI (Player and Wall layers)

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Projectile: Rigidbody component not found! Please add one to the Projectile Prefab.");
            enabled = false;
        }
    }

    // Called by the EnemyAI to set the initial direction
    public void SetDirection(Vector3 direction)
    {
        // Ensure the projectile's transform also aligns with the direction for visual consistency
        transform.forward = direction;
        // Set the Rigidbody's velocity to move the projectile
        rb.velocity = direction * speed;
    }

    void Start()
    {
        // Destroy the projectile after its lifetime, even if it doesn't hit anything
        Destroy(gameObject, lifeTime);
    }

    // OnTriggerEnter is used because the collider is set to 'Is Trigger'
    void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is on one of the layers we care about (Player or Wall)
        // The bitwise operation (1 << other.gameObject.layer) & hitLayers checks if the layer is included.
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            // If it's the player
            if (other.CompareTag("Player")) // Make sure your player has the "Player" tag
            {
                // TODO: Add player damage logic here (e.g., other.GetComponent<PlayerHealth>().TakeDamage(damageAmount);)
            }
            else // Assume it's a wall or other obstruction
            {
                Debug.Log("Projectile hit " + other.name);
            }

            // Create explosion effect
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            // Destroy the projectile itself
            Destroy(gameObject);
        }
    }
}
