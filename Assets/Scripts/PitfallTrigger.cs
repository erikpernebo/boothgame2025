using UnityEngine;

public class PitfallTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false); // Hide the trap cover
        }
    }
}
