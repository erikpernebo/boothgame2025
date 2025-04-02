using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using TMPro;

public class StartScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;              // Already set in the scene; not modified here.
    public TextMeshProUGUI instructionsText;       // Already set in the scene; not modified here.
    public Button startButton;                     // Start Game button

    [Header("Player GameObjects")]
    public GameObject playerOneGameObject;         // Pre-placed Player 1 3D object (unchecked by default)
    public GameObject playerTwoGameObject;         // Pre-placed Player 2 3D object (unchecked by default)
    public GameObject playerThreeGameObject;       // Pre-placed Player 3 3D object (unchecked by default)
    public GameObject playerFourGameObject;        // Pre-placed Player 4 3D object (unchecked by default)

    [Header("Player Labels")]
    public TextMeshProUGUI playerOneLabel;         // Label that says "Player One"
    public TextMeshProUGUI playerTwoLabel;         // Label that says "Player Two"
    public TextMeshProUGUI playerThreeLabel;       // Label that says "Player Three"
    public TextMeshProUGUI playerFourLabel;        // Label that says "Player Four"

    // Track join status for 4 players.
    private bool playerOneJoined = false;
    private bool playerTwoJoined = false;
    private bool playerThreeJoined = false;
    private bool playerFourJoined = false;
    private Keyboard keyboard;

    void Start()
    {
        // Ensure all player game objects are disabled at start.
        if (playerOneGameObject) playerOneGameObject.SetActive(false);
        if (playerTwoGameObject) playerTwoGameObject.SetActive(false);
        if (playerThreeGameObject) playerThreeGameObject.SetActive(false);
        if (playerFourGameObject) playerFourGameObject.SetActive(false);

        // Hide all join labels initially.
        if (playerOneLabel) playerOneLabel.gameObject.SetActive(false);
        if (playerTwoLabel) playerTwoLabel.gameObject.SetActive(false);
        if (playerThreeLabel) playerThreeLabel.gameObject.SetActive(false);
        if (playerFourLabel) playerFourLabel.gameObject.SetActive(false);

        // Disable start button until at least one player has joined.
        startButton.interactable = false;
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(OnStartButtonClicked);

        keyboard = InputSystem.GetDevice<Keyboard>();

        Debug.Log("StartScreenManager active.");
    }

    void Update()
    {
        // Toggle join status:
        // Player One: Space key.
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            TogglePlayerOneJoin();
        }
        // Player Two: Right Shift key.
        if (keyboard.rightShiftKey.wasPressedThisFrame)
        {
            TogglePlayerTwoJoin();
        }
        // Player Three: Z key.
        if (keyboard.zKey.wasPressedThisFrame)
        {
            TogglePlayerThreeJoin();
        }
        // Player Four: X key.
        if (keyboard.xKey.wasPressedThisFrame)
        {
            TogglePlayerFourJoin();
        }

        // If the button is active and Enter is pressed, start the game.
        if (startButton.interactable && Input.GetKeyDown(KeyCode.Return))
        {
            OnStartButtonClicked();
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

    void TogglePlayerThreeJoin()
    {
        playerThreeJoined = !playerThreeJoined;
        if (playerThreeGameObject != null)
        {
            playerThreeGameObject.SetActive(playerThreeJoined);
        }
        if (playerThreeLabel != null)
        {
            playerThreeLabel.gameObject.SetActive(playerThreeJoined);
        }
        UpdateStartButton();
    }

    void TogglePlayerFourJoin()
    {
        playerFourJoined = !playerFourJoined;
        if (playerFourGameObject != null)
        {
            playerFourGameObject.SetActive(playerFourJoined);
        }
        if (playerFourLabel != null)
        {
            playerFourLabel.gameObject.SetActive(playerFourJoined);
        }
        UpdateStartButton();
    }

    // Enable the start button if at least one player has joined.
    void UpdateStartButton()
    {
        bool canStart = playerOneJoined || playerTwoJoined || playerThreeJoined || playerFourJoined;
        startButton.interactable = canStart;

        // Optional: Adjust the visual state (color) of the button based on join status.
        ColorBlock cb = startButton.colors;
        cb.normalColor = canStart ? Color.white : Color.gray;
        startButton.colors = cb;
    }

    // This method is called when the Start button is clicked or Enter is pressed.
    public void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked, loading GameScene");

        // Store join info (using PlayerPrefs) so the game scene knows which players to load.
        PlayerPrefs.SetInt("PlayerOneJoined", playerOneJoined ? 1 : 0);
        PlayerPrefs.SetInt("PlayerTwoJoined", playerTwoJoined ? 1 : 0);
        PlayerPrefs.SetInt("PlayerThreeJoined", playerThreeJoined ? 1 : 0);
        PlayerPrefs.SetInt("PlayerFourJoined", playerFourJoined ? 1 : 0);

        // Load the game scene (ensure the scene name matches exactly in Build Settings).
        SceneManager.LoadScene("GameScene");
    }
}
