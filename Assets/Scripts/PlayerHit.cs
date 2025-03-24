using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            Debug.Log("PLAYER SHOT BY ARROW");
        } else if (collision.gameObject.CompareTag("Spike")){
            Debug.Log("PLAYER HIT BY SPIKE");
        } else if (collision.gameObject.CompareTag("Axe")){
            Debug.Log("PLAYER BIT BY AXE");
        }
    }
}
