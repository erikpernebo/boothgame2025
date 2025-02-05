using UnityEngine;
using System.Collections.Generic;

public class ArrowTrap : MonoBehaviour
{
    public GameObject arrowPrefab; // Assign the arrow prefab in the Inspector
    public Transform[] firePoints; // Assign multiple positions where the arrows will spawn
    public float fireInterval = 2f; // Time between each shot
    public float arrowSpeed = 10f; // Speed of the arrow

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

        foreach (Transform firePoint in firePoints)
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
}
