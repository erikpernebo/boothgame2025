using UnityEngine;
using System.Collections;

public class TorchFlicker : MonoBehaviour {
    private Light torchLight;

    // Set these values relative to your base intensity of 55
    public float minIntensity = 50f;
    public float maxIntensity = 60f;
    
    // Duration range for each flicker transition (in seconds)
    public float minFlickerDuration = 0.05f;
    public float maxFlickerDuration = 0.15f;

    void Start() {
        torchLight = GetComponent<Light>();
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker() {
        while (true) {
            // Random target intensity between min and max
            float targetIntensity = Random.Range(minIntensity, maxIntensity);
            // Random duration for this transition
            float duration = Random.Range(minFlickerDuration, maxFlickerDuration);
            float elapsed = 0f;
            float startingIntensity = torchLight.intensity;

            while (elapsed < duration) {
                // Smoothly interpolate to the target intensity over the duration
                torchLight.intensity = Mathf.Lerp(startingIntensity, targetIntensity, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure we hit the target intensity
            torchLight.intensity = targetIntensity;
        }
    }
}
