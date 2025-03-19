using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static float cameraSpeed = 15f;

    void LateUpdate()
    {
        Vector3 cameraVector = new Vector3(0, 0, cameraSpeed * Time.deltaTime);
        transform.position += cameraVector;
    }
}

