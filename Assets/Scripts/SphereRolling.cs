using UnityEngine;

public class SphereRolling : MonoBehaviour
{
    public float stopZ = 100f; // The z position at which the boulder should stop

    void Update()
    {
        // Only move the boulder if it hasn't reached the stop position yet.
        if (transform.position.z < stopZ)
        {
            // Calculate the movement distance for this frame.
            float moveDistance = CameraFollow.cameraSpeed * Time.deltaTime;

            // If this move would overshoot stopZ, adjust the distance.
            if (transform.position.z + moveDistance > stopZ)
            {
                moveDistance = stopZ - transform.position.z;
            }

            // Move the sphere forward using the adjusted distance.
            transform.Translate(Vector3.forward * moveDistance, Space.World);

            // Calculate and apply the rotation based on the actual move distance.
            float rotationAmount = moveDistance * (360f / (2 * Mathf.PI * transform.localScale.x));
            transform.Rotate(rotationAmount, 0, 0, Space.Self);
        }
    }
}
