using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;

    private bool open = false;
    private bool rock = false;
    private bool soundPlayed = false;
    public GameObject audioManager;



    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("on pressureplate: " + other.tag);
        if (other.CompareTag("Player") || other.CompareTag("Rock"))
        {
            if (other.CompareTag("Rock"))
            {
                rock = true;
            }
                if (!open)
            {
                //myDoor.Play("open gate", 0, 0.0f);
                //this.GetComponent<Animator>().Play("push down", 0, 0.0f);   
                myDoor.SetBool("close", false);
                myDoor.SetBool("open", true);
                open = true;
                if (soundPlayed == false)
                {
                    audioManager.GetComponent<AudioManager>().PressurePlateSound();
                    audioManager.GetComponent<AudioManager>().OpenDoorSound();
                    soundPlayed = true;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Rock"))
        {
            if (other.CompareTag("Rock"))
            {
                rock = false;
            }
            Debug.Log("exit: " + other.tag);
            
            if (open)
            {
                //myDoor.Play("close gate", 0, 0.0f);
                //this.GetComponent<Animator>().Play("push up", 0, 0.0f);
                myDoor.SetBool("close", true);
                myDoor.SetBool("open", false);
                open = false;
                if (!rock)
                {
                    soundPlayed = false;
                }
                
            }
        }
    }
}
