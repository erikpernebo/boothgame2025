using UnityEngine;
using TMPro;

public class DisplayWinner : MonoBehaviour
{
    public TextMeshProUGUI winnerText; // Assign this in Inspector

    void Start()
    {
        // Retrieve message from PlayerPrefs
        string winnerMessage = PlayerPrefs.GetString("WinnerMessage", "No winner! The temple claims its prize!");
        winnerText.text = winnerMessage;
    }
}
