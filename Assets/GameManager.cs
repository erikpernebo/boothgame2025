using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // References to your player controllers.
    // These should be present in the scene but inactive by default.
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
            // Optionally persist across scenes:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Read join info from PlayerPrefs.
        int playerOneJoined = PlayerPrefs.GetInt("PlayerOneJoined", 0);
        int playerTwoJoined = PlayerPrefs.GetInt("PlayerTwoJoined", 0);

        // Activate the players if they joined.
        if (player1 != null)
        {
            player1.gameObject.SetActive(playerOneJoined == 1);
            Debug.Log("Player 1 active: " + (playerOneJoined == 1));
        }
        if (player2 != null)
        {
            player2.gameObject.SetActive(playerTwoJoined == 1);
            Debug.Log("Player 2 active: " + (playerTwoJoined == 1));
        }
    }

    void Update()
    {
        // Determine "dead" state for each player.
        // Treat an inactive player as dead.
        bool p1Dead = (player1 == null || !player1.gameObject.activeSelf || player1.IsDead());
        bool p2Dead = (player2 == null || !player2.gameObject.activeSelf || player2.IsDead());

        // If both players (or the only active player) are dead, trigger the end.
        if (!endTriggered && p1Dead && p2Dead)
        {
            endTriggered = true;
            StartCoroutine(LoadEndAfterDelay(1f));
        }
    }

    IEnumerator LoadEndAfterDelay(float delay)
    {
        // Wait for the specified delay.
        yield return new WaitForSeconds(delay);

        // Optionally, set a fallback winner message.
        PlayerPrefs.SetString("WinnerMessage", "No winner! The temple claims its prize!");

        // Load the Winner scene.
        SceneManager.LoadScene("WinnerScene");
    }
}
