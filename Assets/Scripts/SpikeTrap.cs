using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public Transform spikes;  // Assign the "spikes" parent object in the Inspector
    public float delay = 1f;  // Delay before spikes rise
    public float spikeHeight = 1f;  // Height the spikes will rise
    public float speed = 3f;  // Speed of movement
    public float retractDelay = 1f;  // Time before spikes go back down

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isActivated = false;

    void Start()
    {
        if (spikes == null)
        {
            Debug.LogError("Spikes Transform is not assigned!");
            return;
        }

        // Store original and target positions
        initialPosition = spikes.position;
        targetPosition = initialPosition + new Vector3(0, spikeHeight, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.CompareTag("Player"))
        {
            isActivated = true;
            StartCoroutine(ActivateTrap());
        }
    }

    IEnumerator ActivateTrap()
    {
        // Wait before spikes rise
        yield return new WaitForSeconds(delay);

        // Move spikes up
        yield return StartCoroutine(MoveSpikes(targetPosition));

        // Wait before spikes retract
        yield return new WaitForSeconds(retractDelay);

        // Move spikes back down
        yield return StartCoroutine(MoveSpikes(initialPosition));

        isActivated = false;  // Reset so it can be triggered again
    }

    IEnumerator MoveSpikes(Vector3 target)
    {
        float elapsedTime = 0;
        Vector3 startPosition = spikes.position;

        while (elapsedTime < 1)
        {
            spikes.position = Vector3.Lerp(startPosition, target, elapsedTime * speed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spikes.position = target;
    }
}
