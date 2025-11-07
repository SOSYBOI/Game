using UnityEngine;

public class SlashVFXHandler : MonoBehaviour
{
    [Tooltip("Layer containing enemy GameObjects for collision detection.")]
    public LayerMask enemyLayer;

    [Header("Impact VFX Settings")] // New header
    [Tooltip("Prefab for the impact effect to spawn when an enemy is hit.")]
    public GameObject impactVFXPrefab;

    private ParticleSystem mainSlashParticleSystem; // Renamed for clarity

    void Awake()
    {
        // Get the main ParticleSystem component on this GameObject or its children
        mainSlashParticleSystem = GetComponentInChildren<ParticleSystem>();

        if (mainSlashParticleSystem == null)
        {
            Debug.LogWarning("SlashVFXHandler: No ParticleSystem found on this GameObject or its children. Main VFX will not play.", this);
            enabled = false; // Disable script if no particle system is found
            return;
        }
    }

    void Start()
    {
        // Ensure the main slash particle system plays
        if (mainSlashParticleSystem != null)
        {
            mainSlashParticleSystem.Play();

            // Automatically destroy the main slash GameObject after its particle system finishes
            float totalDuration = mainSlashParticleSystem.main.duration + mainSlashParticleSystem.main.startLifetime.constantMax;
            Destroy(gameObject, totalDuration);
        }
        else
        {
            // If for some reason it was null in Start (shouldn't happen if Awake worked), destroy it.
            Destroy(gameObject);
        }

        // Ensure enemyLayer is set up in the Inspector
        if (enemyLayer.value == 0)
        {
            Debug.LogWarning("Enemy LayerMask for SlashVFXHandler is not set. Slash VFX might not destroy enemies correctly. Please set the 'Enemy Layer' in the Inspector of the Slash VFX prefab.");
        }
    }

    // This method is called when the collider marked as a trigger enters another collider.
    void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is on the enemyLayer
        if (((1 << other.gameObject.layer) & enemyLayer.value) != 0)
        {
            Debug.Log("Slash VFX hit " + other.gameObject.name + ". Destroying enemy.");
            Destroy(other.gameObject);

            // --- NEW: Spawn Impact VFX at the enemy's position ---
            if (impactVFXPrefab != null)
            {
                // Instantiate the impact VFX at the enemy's position.
                // Quaternion.identity means no specific rotation, which is often fine for impact effects.
                // You might want to adjust the Y position slightly (e.g., + Vector3.up * 0.5f)
                // if the enemy's pivot is at its base and the impact should be higher.
                Instantiate(impactVFXPrefab, other.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Impact VFX Prefab is not assigned in SlashVFXHandler. No impact effect will play.");
            }

            // Optional: If you want the slash to only hit one enemy, you could disable its collider here:
            // GetComponent<Collider>().enabled = false;
        }
    }
}
