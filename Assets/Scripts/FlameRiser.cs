using UnityEngine;
using System.Collections;

public class FlameRiser : MonoBehaviour {
    // Assign your 4 flame objects in the inspector.
    public Transform[] flames; 
    // How far upward each flame should move.
    public float riseDistance = 1.0f;
    // Duration of the rising effect in seconds.
    public float riseDuration = 1.0f;

    // Call this method when the condition (e.g., player on green square for 1.5 seconds) is met.
    public void IgniteFlames() {
        foreach (Transform flame in flames) {
            StartCoroutine(RiseFlame(flame));
        }
    }

    IEnumerator RiseFlame(Transform flame) {
        // Record the starting local position of the flame.
        Vector3 startPos = flame.localPosition;
        // Calculate the target position by moving upward by riseDistance.
        Vector3 targetPos = startPos + new Vector3(0, riseDistance, 0);
        float elapsed = 0f;

        // Smoothly interpolate the flame's position upward over riseDuration.
        while (elapsed < riseDuration) {
            flame.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / riseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Ensure the flame reaches the target position.
        flame.localPosition = targetPos;
    }
}
