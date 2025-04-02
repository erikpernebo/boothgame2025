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
    public PlayerMovement player3;
    public PlayerMovement player4;

    // Flag to prevent multiple triggers.
    private bool endTriggered = false;

    void Awake()
    {
        // Singleton pattern for easy access.
        if (Instance == null)
        {
            Instance = this;
            // Optionally, persist across scenes:
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
        int playerThreeJoined = PlayerPrefs.GetInt("PlayerThreeJoined", 0);
        int playerFourJoined = PlayerPrefs.GetInt("PlayerFourJoined", 0);

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
        if (player3 != null)
        {
            player3.gameObject.SetActive(playerThreeJoined == 1);
            Debug.Log("Player 3 active: " + (playerThreeJoined == 1));
        }
        if (player4 != null)
        {
            player4.gameObject.SetActive(playerFourJoined == 1);
            Debug.Log("Player 4 active: " + (playerFourJoined == 1));
        }
    }

    void Update()
    {
        // Determine "dead" state for each player.
        // An inactive player is treated as dead.
        bool p1Dead = (player1 == null || !player1.gameObject.activeSelf || player1.IsDead());
        bool p2Dead = (player2 == null || !player2.gameObject.activeSelf || player2.IsDead());
        bool p3Dead = (player3 == null || !player3.gameObject.activeSelf || player3.IsDead());
        bool p4Dead = (player4 == null || !player4.gameObject.activeSelf || player4.IsDead());

        // If all players (or all active players) are dead, trigger the end.
        if (!endTriggered && p1Dead && p2Dead && p3Dead && p4Dead)
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
