using UnityEngine;

public class StaticSpeedMover : MonoBehaviour
{
    public float moveSpeed = 3f; // Default static speed for this object

    void Update()
    {
        // Move the object forward at the specified static speed
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
    }
}
