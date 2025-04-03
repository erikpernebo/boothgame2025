using UnityEngine;

public class audio : MonoBehaviour
{
    
    public AudioSource audiosource; // Reference to your looping AudioSource
    public AudioClip clip;  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (audiosource != null && clip != null)
        {
        audiosource.clip = clip;
        audiosource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
