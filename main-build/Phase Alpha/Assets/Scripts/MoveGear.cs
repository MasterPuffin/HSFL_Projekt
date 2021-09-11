using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGear : MonoBehaviour
{
    public float speed = 4.0f;

    private bool watching;
    private bool upwards;
    private bool downwards;
    public float botpoint = 0;
    public float toppoint = 50;
    public GameObject triggerObject;
    private PlayerTrigger playerTrigger;
    private GameObject player;
    private bool playerFound;
    public GameObject audioManager;
    private bool soundNotStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        upwards = true;
        downwards = false;
        playerTrigger = triggerObject.GetComponent<PlayerTrigger>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playerFound)
        {
            //Debug.Log("not finding player");
            if (playerTrigger.GetPlayer() != null)
            {
                player = playerTrigger.GetPlayer();
                playerFound = true;
                Debug.Log("player found");
            }
        }
        else
        {
            CheckIfPlayerIsWatching();
        }



        if (upwards && !watching)
        {
            transform.Translate((Vector3.up * Time.deltaTime) * speed);
        }
        else if (downwards && !watching)
        {
            transform.Translate((Vector3.down * Time.deltaTime) * speed);
        }

        if (transform.position.y >= toppoint)
        {
            upwards = false;
            downwards = true;
        }
        if (transform.position.y <= botpoint)
        {
            upwards = true;
            downwards = false;
        }

        

        
    }


    private void CheckIfPlayerIsWatching()
    {
        if (player.transform.eulerAngles.y < 315 && player.transform.eulerAngles.y > 225)
        {
            
            watching = false;
            //Debug.Log("playerWatching");
            if (soundNotStarted == false)
            {
                audioManager.GetComponent<AudioManager>().GearSoundStart();
                soundNotStarted = true;
            }
        }
        else
        {

            watching = true;
            //Debug.Log(player.transform.eulerAngles.y);
            if (soundNotStarted == true)
            {
                audioManager.GetComponent<AudioManager>().GearSoundStop();
                soundNotStarted = false;
            }
        }
    }
}