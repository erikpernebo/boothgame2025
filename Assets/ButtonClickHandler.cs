using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonClickHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Button was clicked via IPointerClickHandler!");
        // Load your game scene.
        SceneManager.LoadScene("GameScene");
    }
}
