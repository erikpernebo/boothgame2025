using UnityEngine;
using System.Collections;

using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionMap gameplayActions;
    public static float moveSpeed = 10f;  // Speed at which the player moves forward
    public float sideSpeed = 3f;  // Speed for sideways movement
    
    [SerializeField]
    private float forwardAdd = 10f; // Speed during forward
    [SerializeField]
    private float slowAdd = 15f; // Speed during slow
    [SerializeField]
    private float boostDuration = 1f;
    [SerializeField]
    private float boostCooldown = 1f;
    [SerializeField]
    private float boostMultiplier = 2f;
    private float forwardSpeed;
    private float slowSpeed;
    private bool isSliding = false; // Flag to check if the player is sliding
    private bool isStopped = false; // Flag to check if the player is stopped
    private float originalMoveSpeed; // Store the original move speed
    private float timeSinceBoost;

    void Awake()
    {
        gameplayActions.Enable();

        gameplayActions["Crouch"].performed += ctx => 
            {
                isSliding = !isSliding;
                UpdatePlayerRotation();
            };
    }

    void Start()
    {
        originalMoveSpeed = moveSpeed; // Store the original move speed
        forwardSpeed = moveSpeed + forwardAdd;
        slowSpeed = originalMoveSpeed - slowAdd;
        timeSinceBoost = boostDuration + boostCooldown;
    }

    void Update()
    {
        timeSinceBoost += Time.deltaTime;

        // If the player is stopped, don't move
        if (isStopped) {
            return;
        }

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // If boost is still going, continue
        if (timeSinceBoost < boostDuration)
        {
            return;
        }

        if (!(Input.GetKey(KeyCode.S) & Input.GetKey(KeyCode.W))) {
            moveSpeed = originalMoveSpeed;
        }

        // move forward with S key
        if (Input.GetKey(KeyCode.S))
        {
            moveSpeed = forwardSpeed;
        }

        // Slow down with W key
        if (Input.GetKey(KeyCode.W))
        {
            moveSpeed = slowSpeed;
        }

        // Boost
        if (Input.GetKey(KeyCode.Space) & timeSinceBoost >= boostDuration + boostCooldown) {
            timeSinceBoost = 0;
            moveSpeed += (boostMultiplier - 1) * (moveSpeed - originalMoveSpeed);
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
