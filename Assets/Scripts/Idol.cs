using UnityEngine;
using System.Collections;

public class Idol : MonoBehaviour
{
    public static Idol Instance { get; private set; }

    public float carryHeight = 2f;  // How high the idol floats above the player
    public Transform boulder;       // Reference to the boulder
    public float zRespawnOffset = 25f;  // Offset in front of boulder when respawning
    public float contactTimeRequired = 3f;  // Time required for consistent contact to start the game

    public Transform idolHolder = null;  // The current player holding the idol

    private Transform playerInContact = null;  // Reference to player currently in contact
    private float contactTimer = 0f;  // Timer to track contact duration
    public Transform cam;
    CameraFollow script;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        script = cam.GetComponent<CameraFollow>();
    }

    private void Update()
    {
        // Check if the boulder has passed the idol's Z position
        if (boulder != null && transform.position.z < boulder.position.z)
        {
            RespawnIdol();
        }

        // If being held, update its position above the holder
        if (idolHolder != null)
        {
            transform.position = idolHolder.position + Vector3.up * carryHeight;
        }

        // If player is in contact with the idol, increment the timer
        if (!script.gameState() && playerInContact != null)
        {
            contactTimer += Time.deltaTime;

            // If the contact time exceeds the required time, start the game
            if (contactTimer >= contactTimeRequired)
            {
                StartGame();
            }
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
            transform.parent = null; // Remove parent
            idolHolder = null;
        }
    }

    private void RespawnIdol()
    {
        DropIdol(); // If held, force-drop it

        // Set new position slightly in front of the boulder
        transform.position = new Vector3(0, 2, boulder.position.z + zRespawnOffset);
        transform.parent = null;
    }

    private void StartGame()
    {
        RespawnIdol();
        script.startGame();
    }
}
