using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public class DisplayWinner : MonoBehaviour
{
    public TextMeshProUGUI winnerText; // Assign this in Inspector
    private float timeSinceSceneLoad = 0f; // Timer to track how long the scene has been open
    public float inputDelay = 5f;          // Delay before input is accepted
    private Keyboard keyboard;

    void Start()
    {
        // Retrieve message from PlayerPrefs.
        string winnerMessage = PlayerPrefs.GetString("WinnerMessage", "No winner! The temple claims its prize!");
        winnerText.text = winnerMessage;
        keyboard = InputSystem.GetDevice<Keyboard>();
    }

    void Update()
    {
        timeSinceSceneLoad += Time.deltaTime;
        // Only process input after the specified delay.
        if (timeSinceSceneLoad < inputDelay)
            return;

        if (keyboard.spaceKey.wasPressedThisFrame ||
            keyboard.rightShiftKey.wasPressedThisFrame ||
            keyboard.zKey.wasPressedThisFrame ||
            keyboard.xKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
