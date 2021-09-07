using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public GameObject interactiblePushableRock;
    public GameObject ruinsSoundtrackActivator;

    public AudioSource pushing_Edited_SomethingMetallicOrRocky = new AudioSource();
    public AudioSource pressurePlate_StonePlate_Edited = new AudioSource();
    public AudioSource pickSomethingUp_Edited = new AudioSource();
    public AudioSource deviceStartUp = new AudioSource();
    public AudioSource windyPlanetSurface = new AudioSource();
    public AudioSource ruinsSoundtrack = new AudioSource();
    public float soundFadeOut = 0;
    public bool soundStopped = false;
    public bool soundGestartet = false;


    // Start is called before the first frame update
    void Start()
    {
        pushing_Edited_SomethingMetallicOrRocky.Stop();
        windyPlanetSurface.Play();
    }

    // Update is called once per frame
    void Update()
    {

        WindyPlanetSurface();
     //   PushingSound();


    }


  /*  public void PushingSound()
    {
        
            
        if (soundStopped == false)
        {
            if (interactiblePushableRock.GetComponent<PullController>().pushingRock == true && soundGestartet == false)
            {
                soundGestartet = true;
               
            
                soundStopped = false;
                pushing_Edited_SomethingMetallicOrRocky.Play();
                pushing_Edited_SomethingMetallicOrRocky.volume = 1;

            }

            if (Input.GetKeyUp("w") || Input.GetKeyUp("e")|| Input.GetKeyUp("a") || Input.GetKeyUp("d"))
            {
                soundStopped = true;
                soundGestartet = false;
            }

        }
        else if (soundStopped == true && soundGestartet == false)
        {
            soundFadeOut += Time.deltaTime;
            pushing_Edited_SomethingMetallicOrRocky.volume -= Time.deltaTime*2;
            if (soundFadeOut > 0.5)
            {
                pushing_Edited_SomethingMetallicOrRocky.Stop();
                soundFadeOut = 0;
                soundStopped = false;
            }
        }
        
    }*/

    public void PressurePlateSound()
    {
        pressurePlate_StonePlate_Edited.Play();
    }


    public void PickSomethingUpSound()
    {
        pickSomethingUp_Edited.Play();
    }

    public void DeviceStartUp()
    {
        deviceStartUp.Play();
    }

    public void WindyPlanetSurface()
    {
        if (ruinsSoundtrackActivator.GetComponent<RuinsSoundtrackActivator>().soundtrackNotPlaying == true)
        {
            windyPlanetSurface.volume = 0.250f;
        }
       
        
    else if(ruinsSoundtrackActivator.GetComponent<RuinsSoundtrackActivator>().soundtrackNotPlaying == false)
        {
            windyPlanetSurface.volume = 0.150f;
        }
    }

    public void RuinsSoundtrack()
    {
        ruinsSoundtrack.Play();
    }
}
