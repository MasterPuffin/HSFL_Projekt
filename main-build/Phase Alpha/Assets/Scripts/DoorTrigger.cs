using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;

    private bool open = false;
    private bool soundPlayed = false;
    public GameObject audioManager;



    private void OnTriggerStay(Collider other)
    {
        Debug.Log("on pressureplate: " + other.tag);
        if (other.CompareTag("Player") || other.CompareTag("Rock"))
        {
            if (!open)
            {
                myDoor.Play("open gate", 0, 0.0f);
                this.GetComponent<Animator>().Play("push down", 0, 0.0f);   
                open = true;
                if (soundPlayed == false)
                {
                    audioManager.GetComponent<AudioManager>().PressurePlateSound();
                    soundPlayed = true;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Rock"))
        {
            Debug.Log("exit: " + other.tag);
            if (open)
            {
                myDoor.Play("close gate", 0, 0.0f);
                this.GetComponent<Animator>().Play("push up", 0, 0.0f);
                open = false;
                soundPlayed = false;
            }
        }
    }
}
