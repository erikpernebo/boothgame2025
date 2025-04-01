using UnityEngine;
using System.Collections;

public class SphereRolling : MonoBehaviour
{
    public float maxZPosition = 715f;                     // The z threshold to trigger the transition.
    public Vector3 targetPosition = new Vector3(0f, 34.2f, 715f);  // Final target position.
    public Vector3 targetRotationEuler = new Vector3(-164.986f, 0f, 0f); // Final target rotation in Euler angles.
    public float transitionDuration = 1f;                 // Duration of the transition.

    private bool isTransitioning = false; // Flag to ensure the transition only happens once.
    
    void Update()
    {
        if (!isTransitioning)
        {
            // Calculate how far the sphere will move this frame.
            float movement = CameraFollow.cameraSpeed * Time.deltaTime;
            
            // Check if the next movement would reach or exceed the maxZPosition.
            if (transform.position.z + movement >= maxZPosition)
            {
                // Start the smooth transition coroutine.
                StartCoroutine(MoveToTarget());
                isTransitioning = true;
            }
            else
            {
                // Normal movement: move forward and rotate to simulate rolling.
                transform.Translate(Vector3.forward * movement, Space.World);
                float rotationAmount = movement * (360f / (2 * Mathf.PI * transform.localScale.x));
                transform.Rotate(rotationAmount, 0, 0, Space.Self);
            }
        }
    }
    
    private IEnumerator MoveToTarget()
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetRotationEuler);
        float elapsedTime = 0f;
        
        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure final values are set.
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }
}
