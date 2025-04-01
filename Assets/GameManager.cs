using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // References to your player controllers.
    // These scripts should have an IsDead() method or similar property.
    public PlayerMovement player1;
    public PlayerMovement player2;

    // Flag to prevent multiple triggers.
    private bool endTriggered = false;

    void Awake()
    {
        // Singleton pattern for easy access.
        if (Instance == null)
        {
            Instance = this;
            // Uncomment if you want the manager to persist across scenes.
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Check if both players are dead and the end hasn't been triggered yet.
        if (!endTriggered && player1.IsDead() && player2.IsDead())
        {
            endTriggered = true;
            StartCoroutine(LoadEndAfterDelay(1f));
        }
    }

    IEnumerator LoadEndAfterDelay(float delay)
    {
        // Wait for the specified delay (1 second).
        yield return new WaitForSeconds(delay);

        // Optionally, set a fallback winner message.
        PlayerPrefs.SetString("WinnerMessage", "No winner! The temple claims its prize!");
        
        // Load the end/winner scene.
        SceneManager.LoadScene("WinnerScene");
    }
}
