using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed at which the player moves forward
    public float sideSpeed = 3f;  // Speed for sideways movement
    public float boostSpeed = 10f; // Speed during boost
    public float boostDuration = 2f; // Duration of the speed boost
    public float slowDownFactor = 0.5f; // Factor to slow down the player
    private bool isSliding = false; // Flag to check if the player is sliding
    private bool isStopped = false; // Flag to check if the player is stopped
    private float originalMoveSpeed; // Store the original move speed
    private float boostEndTime; // Time when the boost ends

    void Start()
    {
        originalMoveSpeed = moveSpeed; // Store the original move speed
    }

    void Update()
    {
        // Handle toggling slide with the C key
        if (Input.GetKeyDown(KeyCode.C))
        {
            isSliding = !isSliding;
            UpdatePlayerRotation();
        }

        // If the player is stopped, don't move
        if (isStopped)
        {
            return;
        }

        // Always move forward at a constant speed
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // Move sideways with A and D keys without rotation
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(-1 * Vector3.right * horizontalInput * sideSpeed * Time.deltaTime);

        // Speed boost with W key
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveSpeed = boostSpeed;
            boostEndTime = Time.time + boostDuration;
        }

        // Check if boost duration has ended
        if (Time.time > boostEndTime)
        {
            moveSpeed = originalMoveSpeed; // Reset to original speed
        }

        // Slow down with S key
        if (Input.GetKey(KeyCode.S))
        {
            moveSpeed = originalMoveSpeed * slowDownFactor;
        }
        else if (Time.time > boostEndTime) // Only reset if not boosting
        {
            moveSpeed = originalMoveSpeed; // Reset to original speed if S is not pressed
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player collides with an arrow
        if (collision.gameObject.CompareTag("Arrow"))
        {
            LayDown();
        }
    }

    public void LayDown()
    {
        // Stop the player from moving
        isStopped = true;
        UpdatePlayerRotation();
        Debug.Log("Player hit by an arrow and laid down.");
    }

    private void UpdatePlayerRotation()
    {
        // Update rotation based on current state
        float zRotation = isSliding || isStopped ? 90f : 0f;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, zRotation);
    }
}
