using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Animator animator;
    [SerializeField] public float sideSpeed = 3f;
    [SerializeField] private float forwardAdd = 10f; // Extra speed when boosting
    [SerializeField] private float slowAdd = 15f; // Speed reduction when slowing
    [SerializeField] private float boostDuration = 1f;
    [SerializeField] private float boostCooldown = 1f;
    [SerializeField] private float boostMultiplier = 2f;

    private bool isBoosting = false;
    private float timeSinceBoost;

    int isSprintingHash; 
    int isSlowJoggingHash;
    int isRightHash;
    int isLeftStrafeHash;
    int isBoostHash;
    private Vector2 movementInput;
    private float baseSpeed = CameraFollow.cameraSpeed;
    float xVal;
    float yVal;
    void Start()
    {
        // Allows boosting on start
        timeSinceBoost = boostDuration + boostCooldown;

        animator = GetComponent<Animator>();

        isSprintingHash = Animator.StringToHash("isSprinting");
        isSlowJoggingHash = Animator.StringToHash("isSlowJogging");
        isRightHash = Animator.StringToHash("right");
        isLeftStrafeHash = Animator.StringToHash("strafeLeft");
        isBoostHash = Animator.StringToHash("isDash");
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

        float dashAnimationTime = 0.2f;

        if (timeSinceBoost >= dashAnimationTime){
            animator.SetBool(isBoostHash, false);
        }
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

        Vector3 move = new Vector3(-boostFactor * (movementInput.x * sideSpeed), 0, baseSpeed + boostFactor * moveSpeed) * Time.deltaTime;
        transform.Translate(move);

        handleMovement();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        Debug.Log(movementInput);

        xVal = movementInput.x;
        yVal = movementInput.y;

        if (xVal == 0 && yVal == 0) {
            animator.SetBool(isSprintingHash, false);
            animator.SetBool(isSlowJoggingHash, false);
            animator.SetBool(isRightHash, false);
            animator.SetBool(isLeftStrafeHash, false);
        }

        
    }

    void handleMovement(){
        animator.SetBool(isSprintingHash, false);
        animator.SetBool(isSlowJoggingHash, false);
        animator.SetBool(isRightHash, false);
        animator.SetBool(isLeftStrafeHash, false);

        if (xVal == 0 && yVal == -1){
            animator.SetBool(isSprintingHash, true);
        } else if (xVal == 0 && yVal == 1){
            animator.SetBool(isSlowJoggingHash, true);
        } else if (((xVal == -1 && yVal != 1) || (xVal == -0.71 && yVal == -0.71))){
            animator.SetBool(isRightHash, true);
        } else if (((xVal == 1 && yVal != 1) || (xVal == 0.71 && yVal == -0.71))){
            animator.SetBool(isLeftStrafeHash, true);
        }
    }
    

    public void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed && timeSinceBoost >= boostDuration + boostCooldown)
        {
            isBoosting = true;
            timeSinceBoost = 0;
            animator.SetBool(isBoostHash, true);
        }
    }
}
