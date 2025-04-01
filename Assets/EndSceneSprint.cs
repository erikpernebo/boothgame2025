using UnityEngine;

public class EndSceneSprint : MonoBehaviour
{
    // Speed at which the player sprints forward
    public float sprintSpeed = 10f;

    void Update()
    {
        // Move the player forward continuously
        transform.Translate(Vector3.forward * sprintSpeed * Time.deltaTime);
    }
}
