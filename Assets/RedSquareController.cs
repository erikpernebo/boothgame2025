using UnityEngine;

public class RedSquareController : MonoBehaviour {
    public GameObject fireEffectPrefab; // Assign your fire particle system prefab in the Inspector
    private bool ignited = false;

    public void Ignite() {
        if (!ignited) {
            ignited = true;
            // Instantiate the fire effect at the red square's position (optionally as a child)
            Instantiate(fireEffectPrefab, transform.position, Quaternion.identity, transform);
            // Optionally, add further behavior such as playing a sound or changing material properties
        }
    }
}
