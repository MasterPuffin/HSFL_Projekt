using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindActivator : MonoBehaviour
{
    public GameObject audioManager;
    
    private void OnTriggerEnter(Collider other)
    {
        
            audioManager.GetComponent<AudioManager>().WindVolumeAdjustment();
         

    }
}
