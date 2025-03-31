using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    int isStunnedHash;
    int isDyingHash;
    private Vector2 movementInput;
    private float baseSpeed = CameraFollow.cameraSpeed;
    float xVal;
    float yVal;
    private float trapHitTimer = 0f;
    public bool invincible = false;
    private bool dying = false;
    
    private Renderer[] characterRenderers;
    private Material[] originalMaterials;
    [SerializeField] private Material flashMaterial; // Assign a white material in the inspector
    [SerializeField] private float flashInterval = 0.1f;

    void Start()
    {
        // Allows boosting on start
        timeSinceBoost = boostDuration + boostCooldown;

        animator = GetComponent<Animator>();

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
       
        bool stunned = animator.GetBool(isStunnedHash);
        if (invincible || dying)
        {                 
            trapHitTimer += Time.deltaTime;
            if (invincible && dying){
               if (trapHitTimer < 2){
                Vector3 move = new Vector3(0, 0, baseSpeed*.7f + moveSpeed*.7f) * Time.deltaTime;
                transform.Translate(move); 
                }
                if (trapHitTimer >= 3f){
                    gameObject.SetActive(false);
                } 
            }
            else if (invincible){
                if (trapHitTimer >= 1.0f && stunned)
                {
                    animator.SetBool(isStunnedHash, false);
                }
                if (trapHitTimer >= 2.0f)
                {
                    invincible = false;
                    trapHitTimer = 0f; 
                }
            } else {
                if (trapHitTimer < 2){
                Vector3 move = new Vector3(0, 0, baseSpeed*.7f + moveSpeed*.7f) * Time.deltaTime;
                transform.Translate(move); 
                }
                if (trapHitTimer >= 3f){
                    gameObject.SetActive(false);
                }
            }
        } 
        if (!stunned && !dying){
            float boostFactor;
            if (isBoosting)
            {
                boostFactor = boostMultiplier;
            } else {
                boostFactor = 1;
            }
            Vector3 move = new Vector3(-boostFactor * (movementInput.x * sideSpeed), 0, baseSpeed + boostFactor * moveSpeed) * Time.deltaTime;
            transform.Translate(move);
        }

        if (!dying) handleMovement();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();

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
        } else if (((xVal == -1 && yVal != 1) || (xVal < -0.70 && xVal > -0.71 && yVal < -0.70 && yVal > -.71))){
            animator.SetBool(isRightHash, true);
        } else if (((xVal == 1 && yVal != 1) || (xVal < 0.71 && xVal > 0.70 && yVal < -0.70 && yVal > -.71))){
            animator.SetBool(isLeftStrafeHash, true);
        }
    }
    

    public void OnBoost(InputAction.CallbackContext context)
    {
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
            if(flashMaterial != null)
                StartCoroutine(FlashWhiteWhileStunned());
        } else if (collision.gameObject.CompareTag("Spike") && !invincible){
            Debug.Log("PLAYER HIT BY SPIKE");
            animator.SetBool(isStunnedHash, true);
            trapHitTimer = 0f;  
            invincible = true;
            if(flashMaterial != null)
                StartCoroutine(FlashWhiteWhileStunned());
        } else if (collision.gameObject.CompareTag("Axe") && !invincible){
            Debug.Log("PLAYER BIT BY AXE");
            animator.SetBool(isStunnedHash, true);
            trapHitTimer = 0f;  
            invincible = true;
            if(flashMaterial != null){
                StartCoroutine(FlashWhiteWhileStunned());
            }
        } else if (collision.gameObject.CompareTag("Boulder")){
            Debug.Log("Crushed By Boulder");
            dying = true;
            animator.SetBool(isStunnedHash, false); 
            animator.SetTrigger("Dying");
            trapHitTimer = 0;
        }

        if (collision.gameObject.CompareTag("Player") && timeSinceBoost < boostDuration)
        {
            if (Idol.Instance.idolHolder == collision.transform)
            {
                // Transfer the idol
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
}
