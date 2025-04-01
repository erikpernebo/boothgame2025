using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DisplayWinner : MonoBehaviour
{
    public TextMeshProUGUI winnerText; // Assign this in Inspector
    private float timeSinceSceneLoad = 0f; // Timer to track how long the scene has been open
    public float inputDelay = 5f;          // Delay before input is accepted

    void Start()
    {
        // Retrieve message from PlayerPrefs.
        string winnerMessage = PlayerPrefs.GetString("WinnerMessage", "No winner! The temple claims its prize!");
        winnerText.text = winnerMessage;
    }

    void Update()
    {
        timeSinceSceneLoad += Time.deltaTime;
        // Only process input after the specified delay.
        if (timeSinceSceneLoad < inputDelay)
            return;

        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.RightShift) ||
            Input.GetKeyDown(KeyCode.Z) ||
            Input.GetKeyDown(KeyCode.X))
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}
