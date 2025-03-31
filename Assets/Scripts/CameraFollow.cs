using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static float cameraSpeed = 15f;
    public float maxZPosition = 100f; // The maximum z position the camera can reach

    void LateUpdate()
    {
        // Calculate the new z position, but don't exceed maxZPosition.
        float newZ = Mathf.Min(transform.position.z + cameraSpeed * Time.deltaTime, maxZPosition);
        transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
    }
}
