using UnityEngine;
using System.Collections;

public class ArrowTrap : MonoBehaviour
{
    public GameObject arrowPrefab;    // Assign the arrow prefab in the Inspector
    public Transform[] firePoints;    // Assign multiple positions where the arrows will spawn
    public float fireInterval = 2f;   // Time between each complete firing cycle
    public float arrowSpeed = 10f;    // Speed of the arrow

    public float burstDelay = 0.5f;   // Delay between bursts
    public int burstSize = 4;         // Number of firePoints to fire in each burst

    private float fireTimer;

    void Start()
    {
        fireTimer = fireInterval; // Initialize the timer
    }

    void Update()
    {
        // Countdown to the next complete firing cycle
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            // Start the sequential burst firing coroutine
            StartCoroutine(FireArrowsBurst());
            fireTimer = fireInterval; // Reset the timer for the next cycle
        }
    }

    private IEnumerator FireArrowsBurst()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            FireArrow(firePoints[i]);

            // Every time we've fired a full burst (burstSize arrows) and there are still firepoints left,
            // wait for burstDelay before firing the next burst.
            if ((i + 1) % burstSize == 0 && i + 1 < firePoints.Length)
            {
                yield return new WaitForSeconds(burstDelay);
            }
        }
        yield break;
    }

    private void FireArrow(Transform firePoint)
    {
        // Calculate the rotation with a 180-degree offset on the Y-axis (adjust if needed)
        Quaternion rotationOffset = Quaternion.Euler(0, 180, 0);
        Quaternion adjustedRotation = firePoint.rotation * rotationOffset;

        // Instantiate the arrow with the adjusted rotation
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, adjustedRotation);

        // Apply velocity to the arrow. Make sure the arrow prefab has a Rigidbody component.
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * arrowSpeed;
        }
        else
        {
            Debug.LogError("Arrow prefab does not have a Rigidbody!");
        }
    }
}
