using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Required for scene management

public class SphereRolling : MonoBehaviour
{
    public float maxZPosition = 715f;                     // The z threshold to trigger the transition.
    public Vector3 targetPosition = new Vector3(0f, 34.2f, 715f);  // Final target position.
    public Vector3 targetRotationEuler = new Vector3(-164.986f, 0f, 0f); // Final target rotation in Euler angles.
    public float transitionDuration = 1f;                 // Duration of the transition.
    [SerializeField] private Transform cam;
    private bool isTransitioning = false; // Flag to ensure the transition only happens once.
    CameraFollow script;
    
    void Start()
    {
        script = cam.GetComponent<CameraFollow>();
    }

    void Update()
    {
        if (!script.gameState())
        {
            return;
        }
        if (!isTransitioning)
        {
            
            float movement = CameraFollow.cameraSpeed * Time.deltaTime;

            if (transform.position.z + movement >= maxZPosition)
            {
                StartCoroutine(MoveToTarget());
                isTransitioning = true;
            }
            else
            {
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
        
        transform.position = targetPosition;
        transform.rotation = targetRotation;

        // Load the "Winner" scene after the transition
        LoadWinnerScene();
    }

    private void LoadWinnerScene()
    {
        string winnerMessage;

        if (Idol.Instance != null && Idol.Instance.idolHolder != null)
        {
            // Get the name of the idol holder
            string winnerName = Idol.Instance.idolHolder.name;
            winnerMessage = $"Player {winnerName} Wins!";
        }
        else
        {
            // Temple of Doom-inspired fallback message
            winnerMessage = "No winner! The temple claims its prize!";
        }

        // Store the message in PlayerPrefs for the WinnerScene to display
        PlayerPrefs.SetString("WinnerMessage", winnerMessage);

        // Load the WinnerScene
        SceneManager.LoadScene("WinnerScene");
    }

}
