using UnityEngine;
using TMPro;

public class ShomAI : MonoBehaviour
{
    int playersInContact = 0;
    public Material shomik;
    public GameObject boulder;
    public GameObject floor;
    public GameObject tooltip;
    public GameObject idol;
    public GameObject vape;

    private void Start()
    { 
    
    }

    private void OnTriggerEnter(Collider other)
    {
        // Track when player enters trigger
        if (other.CompareTag("Player"))
        {
            playersInContact += 1;
            if (playersInContact >= 4) {
                ActivateShomikMode();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset contact tracking when player exits trigger
        if (other.CompareTag("Player"))
        {
            playersInContact -= 1;
        }
    }

    private void ActivateShomikMode()
    {
        boulder.GetComponent<MeshRenderer>().material = shomik;
        floor.GetComponent<MeshRenderer>().material = shomik;
        tooltip.GetComponent<TextMeshProUGUI>().text = "Holy Glaze";
        idol.SetActive(false);
        vape.SetActive(true);
        Debug.Log("Shomik");
    }
}