using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerRuinEntrance : MonoBehaviour
{
    [SerializeField] private Animator plateDoor = null;
    [SerializeField] private Animator entranceDoor = null;
    public bool pressed = false;
    private bool open;
    private bool soundPlayed = false;
    public GameObject audioManager;
    public GameObject otherPlate;

    void Update()
    {
        
        if (otherPlate.GetComponent<DoorTriggerRuinEntrance>().pressed && pressed && !open)
        {
            Debug.Log("plates pressed ");
            open = true;
            plateDoor.Play("open gate", 0, 0.0f);
            entranceDoor.Play("close gate", 0, 0.0f);
            entranceDoor.SetBool("close",true);
            entranceDoor.SetBool("open", false);
            plateDoor.SetBool("close", false);
            plateDoor.SetBool("open", true);
            audioManager.GetComponent<AudioManager>().PressurePlateSound();
            audioManager.GetComponent<AudioManager>().OpenDoorSound();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("on pressureplate: " + other.tag);
        pressed = true;
        if (soundPlayed == false)
        {
            audioManager.GetComponent<AudioManager>().PressurePlateSound();
            soundPlayed = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (!open)
        {
            pressed = false;
            soundPlayed = false;
        }
    }
}
