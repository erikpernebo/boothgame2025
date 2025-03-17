using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float sideSpeed = 3f;

    [SerializeField]
    private float forwardAdd = 10f; // Extra speed when boosting
    [SerializeField]
    private float slowAdd = 15f; // Speed reduction when slowing
    [SerializeField]
    private float boostDuration = 1f;
    [SerializeField]
    private float boostCooldown = 1f;
    [SerializeField]
    private float boostMultiplier = 2f;

    private float forwardSpeed;
    private float slowSpeed;
    private bool isBoosting = false;
    private float originalMoveSpeed;
    private float timeSinceBoost;

    private Vector2 movementInput;

    void Start()
    {
        originalMoveSpeed = moveSpeed;
        forwardSpeed = moveSpeed + forwardAdd;
        slowSpeed = originalMoveSpeed - slowAdd;
        timeSinceBoost = boostDuration + boostCooldown;
    }

    void Update()
    {
        timeSinceBoost += Time.deltaTime;   

        // Reset speed unless actively boosting
        if (!(movementInput.y > 0 && movementInput.y < 0))
        {
            moveSpeed = originalMoveSpeed;
        }

        // Move forward when pressing "S"
        if (movementInput.y < 0)
        {
            moveSpeed = forwardSpeed;
        }

        // Slow down when pressing "W"
        if (movementInput.y > 0)
        {
            moveSpeed = slowSpeed;
        }

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

        Vector3 move = new Vector3(-(movementInput.x * sideSpeed), 0, moveSpeed) * Time.deltaTime * boostFactor;
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
