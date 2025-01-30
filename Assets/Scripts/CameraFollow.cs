using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // The player capsule
    public Vector3 offset = new Vector3(0, 5, -10); // Camera's position offset
    public Vector3 lookOffset = new Vector3(0, 0, 0);
    void LateUpdate()
    {
        // Follow the target
        transform.position = target.position + offset;
        
        transform.LookAt(target.position + lookOffset); // Keep the camera focused on the player
    }
}
