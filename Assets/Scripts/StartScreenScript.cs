using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;              // Already set in the scene; not modified here.
    public TextMeshProUGUI instructionsText;       // Already set in the scene; not modified here.
    public Button startButton;                     // Start Game button

    [Header("Player GameObjects")]
    public GameObject playerOneGameObject;         // Pre-placed Player 1 3D object (should be unchecked by default)
    public GameObject playerTwoGameObject;         // Pre-placed Player 2 3D object (should be unchecked by default)

    [Header("Player Labels")]
    public TextMeshProUGUI playerOneLabel;         // Label that says "Player One"
    public TextMeshProUGUI playerTwoLabel;         // Label that says "Player Two"

    // Track join status
    private bool playerOneJoined = false;
    private bool playerTwoJoined = false;

    void Start()
    {
        // Ensure both player game objects are disabled at start.
        if (playerOneGameObject) playerOneGameObject.SetActive(false);
        if (playerTwoGameObject) playerTwoGameObject.SetActive(false);

        // Hide the join labels initially.
        if (playerOneLabel) playerOneLabel.gameObject.SetActive(false);
        if (playerTwoLabel) playerTwoLabel.gameObject.SetActive(false);

        // Disable start button until at least one player has joined.
        startButton.interactable = false;
    }

    void Update()
    {
        // Toggle Player One join with the Space key.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePlayerOneJoin();
        }

        // Toggle Player Two join with the Right Shift key.
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            TogglePlayerTwoJoin();
        }
    }

    void TogglePlayerOneJoin()
    {
        playerOneJoined = !playerOneJoined;
        if (playerOneGameObject != null)
        {
            playerOneGameObject.SetActive(playerOneJoined);
        }
        if (playerOneLabel != null)
        {
            playerOneLabel.gameObject.SetActive(playerOneJoined);
        }
        UpdateStartButton();
    }

    void TogglePlayerTwoJoin()
    {
        playerTwoJoined = !playerTwoJoined;
        if (playerTwoGameObject != null)
        {
            playerTwoGameObject.SetActive(playerTwoJoined);
        }
        if (playerTwoLabel != null)
        {
            playerTwoLabel.gameObject.SetActive(playerTwoJoined);
        }
        UpdateStartButton();
    }

    // Enable the start button if at least one player has joined.
    void UpdateStartButton()
    {
        startButton.interactable = playerOneJoined || playerTwoJoined;
    }

    // This method is called when the Start button is clicked.
    public void OnStartButtonClicked()
    {
        // Store the join info (using PlayerPrefs) so the game scene knows which players to load.
        PlayerPrefs.SetInt("PlayerOneJoined", playerOneJoined ? 1 : 0);
        PlayerPrefs.SetInt("PlayerTwoJoined", playerTwoJoined ? 1 : 0);

        // Load the game scene (ensure your scene name matches)
        SceneManager.LoadScene("GameScene");
    }
}
