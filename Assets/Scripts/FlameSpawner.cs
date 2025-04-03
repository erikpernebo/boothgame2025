using UnityEngine;
using System.Collections;

public class FlameSpawner : MonoBehaviour {
    // The fire prefab to spawn.
    public GameObject firePrefab;
    
    // An array of spawn points where the fire should appear.
    public Transform[] spawnPoints;
    
    // How far upward the fire should rise.
    public float riseDistance = 2.0f;
    
    // Duration over which the fire rises.
    public float riseDuration = 1.0f;
    
    // Total lifetime of the fire (including rising time).
    public float flameLifetime = 2.0f;

    // Call this method when the trap is triggered.
    public void TriggerFire() {
        foreach (Transform spawnPoint in spawnPoints) {
            // Instantiate the fire prefab at the spawn point.
            GameObject fireInstance = Instantiate(firePrefab, spawnPoint.position, spawnPoint.rotation);
            // Start the coroutine that raises and then destroys the fire.
            StartCoroutine(RiseAndDestroy(fireInstance));
        }
    }

    IEnumerator RiseAndDestroy(GameObject fireInstance) {
        float elapsed = 0f;
        Vector3 startPos = fireInstance.transform.position;
        Vector3 targetPos = startPos + Vector3.up * riseDistance;

        // Raise the fire over the specified duration.
        while (elapsed < riseDuration) {
            if (fireInstance != null) {
                fireInstance.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / riseDuration);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Ensure the fire reaches the target position.
        if (fireInstance != null) {
            fireInstance.transform.position = targetPos;
        }
        
        // Wait the remaining time (flameLifetime - riseDuration).
        yield return new WaitForSeconds(flameLifetime - riseDuration);
        
        // Destroy the fire instance.
        if (fireInstance != null) {
            Destroy(fireInstance);
        }
    }
}