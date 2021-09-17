using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockTablet : MonoBehaviour
{
    public Animator myDoor1 = null;
    public Animator myDoor2 = null;
    private GameObject tabletText;
    private GameObject playerCanvas;
    private GameObject player;
    public GameObject audioManager;
    private bool tabletOffen = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("Player(Clone)");
        }else if (tabletText == null)   //throws exeption if not checked for player first
        {
            playerCanvas = GameObject.Find("PlayerCanvas");
            tabletText = playerCanvas.transform.GetChild(0).gameObject;  //other way to get inactive gameobject?
        }
        //Debug.Log(tabletText);
        
        if (isPickable() && gameObject.layer==11)
        {
            // make tablet pickable
            gameObject.layer = 10;

            
        }

        if (tabletOffen == true)
        {
           // tabletText.SetActive(false);
            tabletOffen = false;
        }
    }

    public void OpenRuins()
    {
        Debug.Log("opened ruins");
        // open doors to ruins, normally in Onpickup script
        //myDoor.Play("open gate", 0, 0.0f);
        myDoor1.SetBool("open", true);
        myDoor1.SetBool("close", false);
        myDoor2.SetBool("open", true);
        myDoor2.SetBool("close", false);

        //Ui Stuff
        playerCanvas.GetComponent<QuestController>().QuestAbgeschlossen();
        //tabletText.SetActive(true);
        tabletOffen = true;
        

        audioManager.GetComponent<AudioManager>().DeviceStartUp();
        // block movement of player
    }


    private bool isPickable()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (!gameObject.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }


    IEnumerator ShowTextCoroutine()
    {
        
        //Print the time of when the function is first called.
         Debug.Log("Started Coroutine at timestamp : " + Time.time);
        

        //EinblendSound

        //yield on a new YieldInstruction that waits for 10 seconds.
        yield return new WaitForSeconds(5);
        Debug.Log("5 Sekunden um");
        //After we have waited 5 seconds print the time again.
        
    }

    }
