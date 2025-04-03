using UnityEngine;
using TMPro;

public class ShomAI : MonoBehaviour
{
    int playersInContact = 0;
    public Material shomik;
    public Material boulderMat;
    public Material floorMat;
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

    public void ActivateShomikMode()
    {
        boulder.GetComponent<MeshRenderer>().material = shomik;
        floor.GetComponent<MeshRenderer>().material = shomik;
        tooltip.GetComponent<TextMeshProUGUI>().text = "Holy Glaze";
        idol.SetActive(false);
        vape.SetActive(true);
        Debug.Log("Shomik");
    }

    public void DeactivateShomikMode()
    {
        boulder.GetComponent<MeshRenderer>().material = boulderMat;
        floor.GetComponent<MeshRenderer>().material = floorMat;
        tooltip.GetComponent<TextMeshProUGUI>().text = "Take the Idol if you Dare";
        idol.SetActive(true);
        vape.SetActive(false);
    }
}