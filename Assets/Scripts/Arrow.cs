using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeTime = 5f; // Time before the arrow is destroyed

    void Start()
    {
        // Automatically destroy the arrow after its lifetime expires
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the arrow hits something
        Debug.Log("Arrow hit: " + collision.gameObject.name);

        // Destroy the arrow on collision
        Destroy(gameObject);
    }
}
