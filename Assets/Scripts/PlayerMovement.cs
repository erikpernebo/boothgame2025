using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    Animator animator;
    private float sideSpeed = 16f;
    [SerializeField] private float forwardAdd = 10f; // Extra speed when boosting
    [SerializeField] private float slowAdd = 15f;      // Speed reduction when slowing
    private float lobbySpeed = 15f;
    [SerializeField] private float boostDuration = 1f;
    [SerializeField] private float boostCooldown = 1f;
    [SerializeField] private float boostMultiplier = 2f;
    
    // Side boundaries.
    [SerializeField] private float leftBoundary = -10f;
    [SerializeField] private float rightBoundary = 10f;
    
    // Use empty GameObjects to define your diagonal barriers.
    [SerializeField] private Transform barrier1PointA; // First barrier, first point
    [SerializeField] private Transform barrier1PointB; // First barrier, second point
    [SerializeField] private float barrier1Threshold = 0f; // Allowed distance for barrier 1

    [SerializeField] private Transform barrier2PointA; // Second barrier, first point
    [SerializeField] private Transform barrier2PointB; // Second barrier, second point
    [SerializeField] private float barrier2Threshold = 0f; // Allowed distance for barrier 2

    [SerializeField] private RuntimeAnimatorController alternateAnimatorController;


    // New field for the forced sprint speed when control is lost.
    [SerializeField] private float forcedSprintSpeed = 15f;
    
    private bool isBoosting = false;
    private float timeSinceBoost;

    int isSprintingHash; 
    int isSlowJoggingHash;
    int isRightHash;
    int isLeftStrafeHash;
    int isBoostHash;
    int isStunnedHash;
    int isDyingHash;
    int isLeftWalkingHash;
    int isLeftBackHash;
    int isRightWalkingHash;
    int isRightBackHash;

    private Vector2 movementInput;
    // baseSpeed is initially set from CameraFollow.
    private float baseSpeed = CameraFollow.cameraSpeed;
    // This variable will store the forced speed when control is lost.
    private float forcedSpeed = 0f;
    
    float xVal;
    float yVal;
    private float trapHitTimer = 0f;
    public bool invincible = false;
    private bool dying = false;
    private bool dead = false;

    private bool swappedAnimator = false;
    
    private Renderer[] characterRenderers;
    private Material[] originalMaterials;
    [SerializeField] private Material flashMaterial; // Assign a white material in the inspector
    [SerializeField] private float flashInterval = 0.1f;
    private float cameraOffsetMax = -15f;
    private float cameraOffsetMin = -70f;
    [SerializeField] private Transform cam;
    CameraFollow camScript;

    void Start()
    {
        // Allows boosting on start.
        timeSinceBoost = boostDuration + boostCooldown;

        animator = GetComponent<Animator>();
        camScript = cam.GetComponent<CameraFollow>();

        characterRenderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[characterRenderers.Length];
        for (int i = 0; i < characterRenderers.Length; i++)
        {
            originalMaterials[i] = characterRenderers[i].material;
        }

        isSprintingHash = Animator.StringToHash("isSprinting");
        isSlowJoggingHash = Animator.StringToHash("isSlowJogging");
        isRightHash = Animator.StringToHash("right");
        isLeftStrafeHash = Animator.StringToHash("strafeLeft");
        isBoostHash = Animator.StringToHash("isDash");
        isStunnedHash = Animator.StringToHash("isStunned");
        isDyingHash = Animator.StringToHash("isDying");
        isLeftWalkingHash = Animator.StringToHash("isLeftWalking");
        isLeftBackHash = Animator.StringToHash("isLeftBack");
        isRightWalkingHash = Animator.StringToHash("isRightWalking");
        isRightBackHash = Animator.StringToHash("isRightBack");

    }
    
    void Update()
    {
        // Once control is lost, force the character to sprint forward.
        if (transform.position.z >= 744f)
        {
            // If forcedSpeed hasn't been captured yet, capture it from forcedSprintSpeed.
            if (forcedSpeed <= 0f)
            {
                forcedSpeed = forcedSprintSpeed;
            }
            
            // Force the sprint animation.
            animator.SetBool(isSprintingHash, true);

            // Move forward at the captured forcedSpeed regardless of camera movement.
            Vector3 forcedMovement = new Vector3(0, 0, forcedSpeed) * Time.deltaTime;
            transform.Translate(forcedMovement, Space.World);
            
            // Clamp the player's x position so they don't exceed the side boundaries.
            Vector3 clampedPos = transform.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, leftBoundary, rightBoundary);
            transform.position = clampedPos;
            
            // Skip processing further inputs.
            return;
        }
        
        float moveSpeed = 0;

        // Move forward when pressing "S"
        if (movementInput.y < 0)
        {
            moveSpeed = forwardAdd;
            if (!camScript.gameState())
            {
                moveSpeed = lobbySpeed;
            }
        }
        // Slow down when pressing "W"
        if (movementInput.y > 0)
        {
            moveSpeed = -slowAdd;
            if (!camScript.gameState())
            {
                moveSpeed = -lobbySpeed;
            }
        }

        timeSinceBoost += Time.deltaTime;

        // Reset boost animation.
        float dashAnimationTime = 0.2f;
        if (timeSinceBoost >= dashAnimationTime)
        {
            animator.SetBool(isBoostHash, false);
        }
        if (timeSinceBoost >= boostDuration)
        {
            isBoosting = false;
        }
       
        bool stunned = animator.GetBool(isStunnedHash);
        if (invincible || dying)
        {                 
            trapHitTimer += Time.deltaTime;
            if (invincible && dying)
            {
                if (trapHitTimer < 2)
                {
                    Vector3 move = new Vector3(0, 0, baseSpeed * 0.7f + moveSpeed * 0.7f) * Time.deltaTime;
                    transform.Translate(move);
                }
                if (trapHitTimer >= 3f)
                {
                    gameObject.SetActive(false);
                } 
            }
            else if (invincible)
            {
                if (trapHitTimer >= 1.0f && stunned)
                {
                    animator.SetBool(isStunnedHash, false);
                }
                if (trapHitTimer >= 2.0f)
                {
                    invincible = false;
                    trapHitTimer = 0f; 
                }
            }
            else
            {
                if (trapHitTimer < 2)
                {
                    Vector3 move = new Vector3(0, 0, baseSpeed * 0.7f + moveSpeed * 0.7f) * Time.deltaTime;
                    transform.Translate(move); 
                }
                if (trapHitTimer >= 3f)
                {
                    gameObject.SetActive(false);
                    dead = true;
                }
            }
        } 
        if (!stunned && !dying)
        {
            float boostFactor = isBoosting ? boostMultiplier : 1;
            Vector3 move;
            if (camScript.gameState())
            {
                move = new Vector3(-boostFactor * (movementInput.x * sideSpeed), 0, baseSpeed + boostFactor * moveSpeed) * Time.deltaTime;
            }
            else
            {
                move = new Vector3(-boostFactor * (movementInput.x * sideSpeed), 0, boostFactor * moveSpeed) * Time.deltaTime;
            }
            transform.Translate(move);

            float camZ = cam.transform.position.z;
            if (transform.position.z > camZ + cameraOffsetMax)
            {
                Vector3 adjust = new Vector3(0, 0, -(transform.position.z - (camZ + cameraOffsetMax)));
                transform.Translate(adjust);    
            }

            if (!camScript.gameState() && transform.position.z < camZ + cameraOffsetMin)
            {
                Vector3 adjust = new Vector3(0, 0, -(transform.position.z - (camZ + cameraOffsetMin)));
                transform.Translate(adjust);
            }
        }

        if (!dying)
        {
            if (camScript.gameState())
            {   
                if (!swappedAnimator){
                    animator.runtimeAnimatorController = alternateAnimatorController;
                    swappedAnimator = true;
                }
                handleMovement();
            }
            else
            {
                handleLobbyMovement();
            }
        }
        
        // Clamp the player's x position so they don't go beyond the side borders.
        Vector3 clampedPos2 = transform.position;
        clampedPos2.x = Mathf.Clamp(clampedPos2.x, leftBoundary, rightBoundary);
        transform.position = clampedPos2;
        
        // Only apply barrier clamping if the player is near the end of the level.
        if (transform.position.z >= 684f)
        {
            // Clamp against both diagonal barriers.
            ClampToBarrier(barrier1PointA.position, barrier1PointB.position, barrier1Threshold);
            ClampToBarrier(barrier2PointA.position, barrier2PointB.position, barrier2Threshold);
        }
    }

    public bool IsDead() { return dead; }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Disable input if control is lost.
        if (transform.position.z >= 744f) return;
        movementInput = context.ReadValue<Vector2>();

        xVal = movementInput.x;
        yVal = movementInput.y;

        if (xVal == 0 && yVal == 0)
        {
            animator.SetBool(isSprintingHash, false);
            animator.SetBool(isSlowJoggingHash, false);
            animator.SetBool(isRightHash, false);
            animator.SetBool(isLeftStrafeHash, false);
        }
    }

    void handleMovement()
    {
        animator.SetBool(isSprintingHash, false);
        animator.SetBool(isSlowJoggingHash, false);
        animator.SetBool(isRightHash, false);
        animator.SetBool(isLeftStrafeHash, false);

        if (xVal == 0 && yVal == -1)
        {
            animator.SetBool(isSprintingHash, true);
        }
        else if (xVal == 0 && yVal == 1)
        {
            animator.SetBool(isSlowJoggingHash, true);
        }
        else if (((xVal == -1 && yVal != 1) || (xVal < -0.70f && xVal > -0.71f && yVal < -0.70f && yVal > -0.71f)))
        {
            animator.SetBool(isRightHash, true);
        }
        else if (((xVal == 1 && yVal != 1) || (xVal < 0.71f && xVal > 0.70f && yVal < -0.70f && yVal > -0.71f)))
        {
            animator.SetBool(isLeftStrafeHash, true);
        }
    }

    void handleLobbyMovement()
    {
        // Reset all animator booleans
        animator.SetBool(isSprintingHash, false);
        animator.SetBool(isSlowJoggingHash, false);
        animator.SetBool(isRightHash, false);
        animator.SetBool(isLeftStrafeHash, false);
        animator.SetBool(isLeftWalkingHash, false);
        animator.SetBool(isLeftBackHash, false);
        animator.SetBool(isRightWalkingHash, false);
        animator.SetBool(isRightBackHash, false);

        // Forward and backward logic (already present)
        if (xVal == 0 && yVal == -1)
        {
            animator.SetBool(isSprintingHash, true);
        }
        else if (xVal == 0 && yVal == 1)
        {
            animator.SetBool(isSlowJoggingHash, true);
        }

        // Left / Right Forward movement
        else if (xVal == 1 && yVal != 1) 
        {
            animator.SetBool(isLeftWalkingHash, true);
        }
        else if (xVal == -1 && yVal != 1)
        {
            animator.SetBool(isRightWalkingHash, true);
        }

        // Left / Right Backward movement
        else if (xVal < -0.70f && xVal > -0.71f && yVal > 0.70f && yVal < 0.71f)
        {
            animator.SetBool(isLeftBackHash, true);
        }
        else if (xVal > 0.70f && xVal < 0.71f && yVal > 0.70f && yVal < 0.71f)
        {
            animator.SetBool(isRightBackHash, true);
        }
        else if ((xVal < -0.70f && xVal > -0.71f && yVal < -0.70f && yVal > -0.71f))
        {
            animator.SetBool(isRightHash, true);
        }
        else if ((xVal < 0.71f && xVal > 0.70f && yVal < -0.70f && yVal > -0.71f))
        {
            animator.SetBool(isLeftStrafeHash, true);
        }
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        if (transform.position.z >= 744f) return;
        if (context.performed && timeSinceBoost >= boostDuration + boostCooldown && !animator.GetBool(isStunnedHash) && !dying)
        {
            isBoosting = true;
            timeSinceBoost = 0;
            animator.SetBool(isBoostHash, true);
        }
    }
    
    private void OnTriggerEnter(Collider collision)
    {   
        if (dying) return; 
        if (collision.gameObject.CompareTag("Arrow") && !invincible)
        {
            Debug.Log("PLAYER SHOT BY ARROW");
            animator.SetBool(isStunnedHash, true);
            trapHitTimer = 0f;  
            invincible = true;
            if (flashMaterial != null)
                StartCoroutine(FlashWhiteWhileStunned());
            if (Idol.Instance.idolHolder == transform) {
                Idol.Instance.DropIdol();
            }
        }
        else if (collision.gameObject.CompareTag("Spike") && !invincible)
        {
            Debug.Log("PLAYER HIT BY SPIKE");
            animator.SetBool(isStunnedHash, true);
            trapHitTimer = 0f;  
            invincible = true;
            if (flashMaterial != null)
                StartCoroutine(FlashWhiteWhileStunned());
            if (Idol.Instance.idolHolder == transform) {
                Idol.Instance.DropIdol();
            }
        }
        else if (collision.gameObject.CompareTag("Axe") && !invincible)
        {
            Debug.Log("PLAYER BIT BY AXE");
            animator.SetBool(isStunnedHash, true);
            trapHitTimer = 0f;  
            invincible = true;
            if (flashMaterial != null)
            {
                StartCoroutine(FlashWhiteWhileStunned());
            }
            if (Idol.Instance.idolHolder == transform) {
                Idol.Instance.DropIdol();
            }
        }
        else if (collision.gameObject.CompareTag("FireTrap") && !invincible)
        {
            Debug.Log("PLAYER HIT BY FIRE TRAP");
            animator.SetBool(isStunnedHash, true);
            trapHitTimer = 0f;
            invincible = true;
            if (flashMaterial != null)
                StartCoroutine(FlashWhiteWhileStunned());
            if (Idol.Instance.idolHolder == transform) {
                Idol.Instance.DropIdol();
            }
        }
        else if (collision.gameObject.CompareTag("Boulder") && camScript.gameState())
        {
            Debug.Log("Crushed By Boulder");
            dying = true;
            animator.SetBool(isStunnedHash, false); 
            animator.SetTrigger("Dying");
            trapHitTimer = 0;
            if (Idol.Instance.idolHolder == transform) {
                Idol.Instance.DropIdol();
            }
        }
        else if (collision.gameObject.CompareTag("Door") && !camScript.gameState())
        {
            gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Player") && timeSinceBoost < boostDuration)
        {
            if (Idol.Instance.idolHolder == collision.transform)
            {
                // Transfer the idol.
                Idol.Instance.PickUpIdol(transform);
            }
        }
    }
    
    IEnumerator FlashWhiteWhileStunned()
    {
        float flashDuration = 1.8f; // Total flash time (match invincibility duration)
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            // Set flash material on all renderers.
            for (int i = 0; i < characterRenderers.Length; i++)
            {
                characterRenderers[i].material = flashMaterial;
            }
            yield return new WaitForSeconds(flashInterval);
            elapsedTime += flashInterval;
            // Reset each renderer's material back to original.
            for (int i = 0; i < characterRenderers.Length; i++)
            {
                characterRenderers[i].material = originalMaterials[i];
            }
            yield return new WaitForSeconds(flashInterval);
            elapsedTime += flashInterval;
        }
        // Final reset after flashing stops.
        for (int i = 0; i < characterRenderers.Length; i++)
        {
            characterRenderers[i].material = originalMaterials[i];
        }
    }

    public bool ableToBoost()
    {
        return timeSinceBoost >= boostDuration + boostCooldown;
    }
    
    private void ClampToBarrier(Vector3 pointA, Vector3 pointB, float threshold)
    {
        // Calculate the barrier's direction.
        Vector3 barrierDirection = pointB - pointA;
        // Compute the barrier normal by crossing the barrier direction with Vector3.up.
        Vector3 barrierNormal = Vector3.Cross(barrierDirection, Vector3.up).normalized;
        
        // Debug: Draw the barrier line and its normal for visualization.
        Debug.DrawLine(pointA, pointB, Color.green);
        Debug.DrawRay(pointA, barrierNormal * 5f, Color.red);
        
        // Compute the player's signed distance from the barrier.
        float distance = Vector3.Dot(transform.position - pointA, barrierNormal);
        
        // If the distance is less than the threshold, push the player back.
        if (distance < threshold)
        {
            float pushDistance = threshold - distance;
            transform.position += pushDistance * barrierNormal;
            Debug.Log("Clamped to barrier: pushing " + pushDistance + " units along " + barrierNormal);
        }
    }
}
