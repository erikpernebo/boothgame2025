using UnityEngine;

public class SphereRolling : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the sphere moves
    public float rotationSpeed = 360f; // Rotation speed for rolling (degrees per second)

    void Update()
    {
        // Move the sphere forward at the specified speed
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);

        // Rotate the sphere to simulate rolling
        float rotationAmount = moveSpeed * Time.deltaTime * (360f / (2 * Mathf.PI * transform.localScale.x));
        transform.Rotate(rotationAmount, 0, 0, Space.Self);
    }
}
