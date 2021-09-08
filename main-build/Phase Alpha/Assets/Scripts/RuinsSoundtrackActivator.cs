using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuinsSoundtrackActivator : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject audioManager;
    public bool soundtrackNotPlaying;

    // Update is called once per frame
    void Start()
    {
        soundtrackNotPlaying = true;
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (soundtrackNotPlaying == true)
        {
            audioManager.GetComponent<AudioManager>().RuinsSoundtrack();
            soundtrackNotPlaying = false;
        }

    }
}
