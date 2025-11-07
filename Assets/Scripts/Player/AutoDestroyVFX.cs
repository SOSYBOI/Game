using UnityEngine;

public class AutoDestroyVFX : MonoBehaviour
{
    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps == null)
        {
            // If the main component is not a ParticleSystem, try children
            ps = GetComponentInChildren<ParticleSystem>();
        }

        if (ps != null)
        {
            // Destroy after the particle system finishes playing
            // This accounts for the main duration and the lifetime of individual particles
            float totalDuration = ps.main.duration + ps.main.startLifetime.constantMax;
            Destroy(gameObject, totalDuration);
        }
        else
        {
            // Fallback: If no particle system is found, destroy after a default time
            Debug.LogWarning("AutoDestroyVFX: No ParticleSystem found on this GameObject or its children. Destroying after 2 seconds.", this);
            Destroy(gameObject, 2f);
        }
    }
}
