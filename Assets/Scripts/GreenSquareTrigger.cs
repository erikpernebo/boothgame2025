using UnityEngine;
public class GreenSquareTrigger : MonoBehaviour {
    public float requiredTime = 1.5f;
    private float timer = 0f;
    // Reference to the FlameSpawner instead of RedSquareController
    public FlameSpawner flameSpawner;
    public AudioSource audiosource;
    public AudioClip clip;
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            timer = 0f;
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            timer += Time.deltaTime;
            if (timer >= requiredTime) {
                if (audiosource != null && clip != null)
                {
                audiosource.clip = clip;
                audiosource.Play();
                }
                flameSpawner.TriggerFire();
                // Optionally disable further triggering
                enabled = false;
                
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            timer = 0f;
        }
    }
}
