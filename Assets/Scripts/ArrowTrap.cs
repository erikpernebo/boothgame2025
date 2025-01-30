using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    public GameObject arrowPrefab; // Assign the arrow prefab in the Inspector
    public Transform firePoint;    // Assign the position where the arrow will spawn
    public float fireInterval = 2f; // Time between each shot
    public float arrowSpeed = .2f; // Speed of the arrow

    private float fireTimer;

    void Update()
    {
        // Countdown to the next fire
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            FireArrow();
            fireTimer = fireInterval; // Reset the timer
        }
    }

private void FireArrow()
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
