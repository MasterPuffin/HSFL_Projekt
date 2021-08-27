using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLabyrinth : MonoBehaviour
{
    public GameObject door1;
    public GameObject door2;
    public GameObject door3;


    public void ButtonLab()
    {
        //Debug.Log(door1.transform.position.y >= -543);
        
        if (door1.transform.position.y >= -543)
        {
            Debug.Log(" d1 up");
            StartCoroutine(Movedown(3f, door1));
        }
        else
        {
            Debug.Log(" d1 down");
            
            StartCoroutine(Moveup(3f, door1));
        }
        if (door2.transform.position.y >= -543)
        {
            Debug.Log(" d6 up");
            
            StartCoroutine(Movedown(3f, door2));
        }
        else
        {
            Debug.Log(" d6 down");
            StartCoroutine(Moveup(3f, door2));
        }
        if (door3.transform.position.y >= -543)
        {
            Debug.Log(" d7 up");
            
            StartCoroutine(Movedown(3f, door3));
        }
        else
        {
            Debug.Log(" d7 down");
            StartCoroutine(Moveup(3f, door3));
        }


    }

    private IEnumerator Moveup(float time, GameObject door)
    {
        
        Vector3 startingPos = door.transform.position;
        Vector3 finalPos = door.transform.position + ( new Vector3(0,8,0));
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            door.transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator Movedown(float time, GameObject door)
    {
        Vector3 startingPos = door.transform.position;
        Vector3 finalPos = door.transform.position - new Vector3(0, 8, 0);
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            door.transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}


