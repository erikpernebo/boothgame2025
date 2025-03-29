using UnityEngine;

public class Idol : MonoBehaviour
{
    public static Idol Instance { get; private set; }

    public float carryHeight = 2f;  // How high the idol floats above the player
    public Transform boulder;       // Reference to the boulder
    public float zRespawnOffset = 20f;  // Offset in front of boulder when respawning

    public Transform idolHolder = null;  // The current player holding the idol

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        RespawnIdol();  // Ensure the idol starts in the correct position
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (idolHolder == null && other.CompareTag("Player"))
        {
            PickUpIdol(other.transform);
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
}
