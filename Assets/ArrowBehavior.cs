using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    // This function is called when the arrow collides with another collider.
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the tag "Wall".
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Destroy the arrow.
            Destroy(gameObject);
        }
    }
}
