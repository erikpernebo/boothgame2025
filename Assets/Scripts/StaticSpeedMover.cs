using UnityEngine;

public class StaticMovement : MonoBehaviour
{
    public float moveSpeed = 8f; // Constant speed for forward movement

    void Update()
    {
        // Move the object along the Z-axis at the specified speed
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
