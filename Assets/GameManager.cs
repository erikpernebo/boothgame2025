using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // References to your player controllers.
    // These should be present in the scene but inactive by default.
    public PlayerMovement player1;
    public PlayerMovement player2;
    public PlayerMovement player3;
    public PlayerMovement player4;
    [SerializeField] private Transform cam;
    private Keyboard keyboard;
    CameraFollow script;

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
        keyboard = InputSystem.GetDevice<Keyboard>();
        script = cam.GetComponent<CameraFollow>();
    }

    void Start()
    {
        player1.gameObject.SetActive(false);
        player2.gameObject.SetActive(false);
        player3.gameObject.SetActive(false);
        player4.gameObject.SetActive(false);
    }

    void Update()
    {
        // Player One: Space key.
        if (!player1.gameObject.activeSelf && keyboard.spaceKey.wasPressedThisFrame)
        {
            player1.gameObject.SetActive(true);
        }
        // Player Two: Right Shift key.
        if (!player2.gameObject.activeSelf && keyboard.rightShiftKey.wasPressedThisFrame)
        {
            player2.gameObject.SetActive(true);
        }
        // Player Three: Z key.
        if (!player3.gameObject.activeSelf && keyboard.zKey.wasPressedThisFrame)
        {
            player3.gameObject.SetActive(true);
        }
        // Player Four: X key.
        if (!player4.gameObject.activeSelf && keyboard.xKey.wasPressedThisFrame)
        {
            player4.gameObject.SetActive(true);
        }

        // Determine "dead" state for each player.
        // An inactive player is treated as dead.
        bool p1Dead = (player1 == null || !player1.gameObject.activeSelf || player1.IsDead());
        bool p2Dead = (player2 == null || !player2.gameObject.activeSelf || player2.IsDead());
        bool p3Dead = (player3 == null || !player3.gameObject.activeSelf || player3.IsDead());
        bool p4Dead = (player4 == null || !player4.gameObject.activeSelf || player4.IsDead());

        // If all players (or all active players) are dead, trigger the end.
        if (script.gameState() && !endTriggered && p1Dead && p2Dead && p3Dead && p4Dead)
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
