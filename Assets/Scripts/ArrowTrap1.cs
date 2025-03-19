using UnityEngine;
using System.Collections.Generic;

public class ArrowTrap1 : MonoBehaviour
{
    public GameObject arrowPrefab; // Assign the arrow prefab in the Inspector
    public Transform[] firePoints; // Assign multiple positions where the arrows will spawn
    public float fireInterval = 2f; // Time between each shot
    public float arrowSpeed = 10f; // Speed of the arrow

    [SerializeField]
    public float staggerDelay = 0.5f; // Delay for the second group of arrows

    public float secondStaggerDelay = 0.7f; // Additional delay for firePoints 5-9

    private float fireTimer;

    void Start()
    {
        fireTimer = fireInterval; // Initialize the timer
    }

    void Update()
    {
        // Countdown to the next fire
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            FireArrows();
            fireTimer = fireInterval; // Reset the timer
        }
    }

    private void FireArrows()
    {
        if (arrowPrefab == null || firePoints.Length == 0)
        {
            Debug.LogError("Arrow prefab or fire points not set!");
            return;
        }

        // Fire the first group of arrows immediately (firePoints 0-4)
        for (int i = 0; i < Mathf.Min(5, firePoints.Length); i++)
        {
            FireArrow(firePoints[i]);
        }

        // Fire the second group (firePoints 5-9) with staggered delay
        if (firePoints.Length > 5)
        {
            Invoke(nameof(FireSecondGroup), staggerDelay);
        }

        // Fire the third group (firePoints 10+) with another delay
        if (firePoints.Length > 10)
        {
            Invoke(nameof(FireThirdGroup), secondStaggerDelay);
        }
    }

    private void FireSecondGroup()
    {
        for (int i = 5; i < Mathf.Min(10, firePoints.Length); i++)
        {
            FireArrow(firePoints[i]);
        }
    }

    private void FireThirdGroup()
    {
        for (int i = 10; i < firePoints.Length; i++)
        {
            FireArrow(firePoints[i]);
        }
    }

    private void FireArrow(Transform firePoint)
    {
        // Calculate the rotation with a 90-degree offset on the Y-axis
        Quaternion rotationOffset = Quaternion.Euler(0, 270, 0);
        Quaternion adjustedRotation = firePoint.rotation * rotationOffset;

        // Instantiate the arrow with the adjusted rotation
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, adjustedRotation);

        // Apply velocity to the arrow
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
