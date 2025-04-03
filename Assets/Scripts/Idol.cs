using UnityEngine;
using System.Collections;

public class Idol : MonoBehaviour
{
    public static Idol Instance { get; private set; }

    public float carryHeight = 2f;  // How high the idol floats above the player
    public Transform boulder;       // Reference to the boulder
    public float zRespawnOffset = 25f;  // Offset in front of boulder when respawning
    public float contactTimeRequired = 3f;  // Time required for consistent contact to start the game
    public float throwForce = 15f;   // Force applied when dropping the idol

    public Transform idolHolder = null;  // The current player holding the idol
    public GameObject myUIElement; // Assign this in the Inspector

    private Transform playerInContact = null;  // Reference to player currently in contact
    private float contactTimer = 0f;  // Timer to track contact duration
    public Transform cam;
    CameraFollow script;
    private Rigidbody rb;

    private CameraShake cameraShake;
    private bool isShaking = false;
    public float shakeThreshold = 2.5f; // Start shaking when timer is below this value
    public float maxShakeIntensity = 0.75f; // Maximum shake intensity when timer is near zero

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        script = cam.GetComponent<CameraFollow>();
        rb = GetComponent<Rigidbody>();

        // If there's no Rigidbody, add one
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
        if (cameraShake == null)
        {
            cameraShake = Camera.main.gameObject.AddComponent<CameraShake>();
        }
    }

    private void Update()
    {
        // Check if the boulder has passed the idol's Z position
        if (boulder != null && transform.position.z < boulder.position.z)
        {
            RespawnIdol();
        }

        // If being held, update its position above the holder and keep it upright
        if (idolHolder != null)
        {
            transform.position = idolHolder.position + Vector3.up * carryHeight;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            // Disable physics while being held
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }

        // If player is in contact with the idol, increment the timer
        if (!script.gameState() && playerInContact != null)
        {
            contactTimer += Time.deltaTime;

            // Start shaking when timer is close to completion
            if (contactTimer >= (contactTimeRequired - shakeThreshold) && contactTimer < contactTimeRequired)
            {
                // Calculate intensity based on how close we are to completion
                float progress = (contactTimer - (contactTimeRequired - shakeThreshold)) / shakeThreshold;
                float intensity = progress * maxShakeIntensity;

                // Apply camera shake with increasing intensity
                if (cameraShake != null && !isShaking)
                {
                    cameraShake.ShakeCamera(intensity, 0.2f);
                    isShaking = true;
                    StartCoroutine(ResetShakeFlag(0.25f)); // Allow shake to happen again after a short delay
                }
            }

            // If the contact time exceeds the required time, start the game
            if (contactTimer >= contactTimeRequired)
            {
                myUIElement.SetActive(false);
                StartGame();
            }
        }
        else if (isShaking && cameraShake != null)
        {
            // Stop shaking if player breaks contact
            cameraShake.StopShaking();
            isShaking = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (script.gameState() && idolHolder == null && other.CompareTag("Player"))
        {
            PickUpIdol(other.transform);
        }

        // Track when player enters trigger
        if (other.CompareTag("Player"))
        {
            playerInContact = other.transform;
            contactTimer = 0f;  // Reset timer when contact begins
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Ensure we're tracking the correct player if multiple players exist
        if (other.CompareTag("Player") && playerInContact == null)
        {
            playerInContact = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset contact tracking when player exits trigger
        if (other.CompareTag("Player") && other.transform == playerInContact)
        {
            playerInContact = null;
            contactTimer = 0f;  // Reset timer when contact is broken
        }
    }

    public void PickUpIdol(Transform newHolder)
    {
        idolHolder = newHolder;
        transform.parent = idolHolder;
        transform.position = idolHolder.position + Vector3.up * carryHeight;
    }

    public void DropIdol()
    {
        if (idolHolder != null)
        {
            // Get the player's forward direction
            Vector3 playerForward = idolHolder.forward;

            // Remove parent
            transform.parent = null;

            // Enable physics
            if (rb != null)
            {
                rb.isKinematic = false;

                // Generate a random angle between -135 and 135 degrees (avoiding the 90 degrees behind)
                float randomAngle = Random.Range(-100f, 100f);

                // Calculate the throw direction (rotating around the Y axis)
                Vector3 throwDirection = Quaternion.Euler(0, randomAngle, 0) * playerForward;

                // Add upward component for 45-degree angle
                throwDirection = 2 * throwDirection + Vector3.up;
                throwDirection.Normalize();

                // Apply force in the calculated direction
                rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }

            idolHolder = null;
        }
    }

    private void RespawnIdol()
    {
        if (idolHolder != null)
        {
            transform.parent = null; // Remove parent
            idolHolder = null;
        }

        // Set new position slightly in front of the boulder
        transform.position = new Vector3(0, 2, boulder.position.z + zRespawnOffset);
        transform.parent = null;

        // Reset physics
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }
    }

    private void StartGame()
    {
        RespawnIdol();
        script.startGame();
    }

    private IEnumerator ResetShakeFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        isShaking = false;
    }
}
