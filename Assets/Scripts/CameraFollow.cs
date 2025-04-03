using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static float cameraSpeed = 15f;
    public float maxZPosition = 100f; // The maximum z position the camera can reach
    public bool gameStarted = false;
    [SerializeField] private Transform cam;

    void LateUpdate()
    {
        if (!gameStarted) {
            return;
        }
        // Calculate the new z position, but don't exceed maxZPosition.
        float newZ = Mathf.Min(transform.position.z + cameraSpeed * Time.deltaTime, maxZPosition);
        transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
    }

    public void startGame() {
        gameStarted = true;
    }

    public void stopGame()
    {
        gameStarted = false;
    }

    public bool gameState()
    {
        return gameStarted;
    }
}
