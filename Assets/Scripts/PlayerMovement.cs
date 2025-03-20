using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float sideSpeed = 3f;
    [SerializeField] private float forwardAdd = 10f; // Extra speed when boosting
    [SerializeField] private float slowAdd = 15f; // Speed reduction when slowing
    [SerializeField] private float boostDuration = 1f;
    [SerializeField] private float boostCooldown = 1f;
    [SerializeField] private float boostMultiplier = 2f;

    private bool isBoosting = false;
    private float timeSinceBoost;

    private Vector2 movementInput;
    private float baseSpeed = CameraFollow.cameraSpeed;

    void Start()
    {
        // Allows boosting on start
        timeSinceBoost = boostDuration + boostCooldown;
    }

    void Update()
    {
        float moveSpeed = 0;

        // Move forward when pressing "S"
        if (movementInput.y < 0)
        {
            moveSpeed = forwardAdd;
        }

        // Slow down when pressing "W"
        if (movementInput.y > 0)
        {
            moveSpeed = -slowAdd;
        }

        timeSinceBoost += Time.deltaTime;

        // Reset boost
        if (timeSinceBoost >= boostDuration)
        {
            isBoosting = false;
        }

        // Move the player
        float boostFactor;
        if (isBoosting)
        {
            boostFactor = boostMultiplier;
        } else {
            boostFactor = 1;
        }
        Debug.Log(boostFactor);
        Vector3 move = new Vector3(-boostFactor * (movementInput.x * sideSpeed), 0, baseSpeed + boostFactor * moveSpeed) * Time.deltaTime;
        transform.Translate(move);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed && timeSinceBoost >= boostDuration + boostCooldown)
        {
            isBoosting = true;
            timeSinceBoost = 0;
        }
    }
}
