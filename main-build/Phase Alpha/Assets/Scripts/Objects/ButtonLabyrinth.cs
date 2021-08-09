using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLabyrinth : MonoBehaviour
{
    public GameObject door1;
    public GameObject door2;
    public GameObject door3;
    // Start is called before the first frame update
    void Start()
    {
        if (door1.transform.position.y < -334)
        {
            StartCoroutine(Moveup(3f, door1));
        }
        else
        {
            StartCoroutine(Movedown(3f, door1));
        }
        if (door2.transform.position.y < -334)
        {
            StartCoroutine(Moveup(3f, door2));
        }
        else
        {
            StartCoroutine(Movedown(3f, door2));
        }
        if (door3.transform.position.y < -334)
        {
            StartCoroutine(Moveup(3f, door3));
        }
        else
        {
            StartCoroutine(Movedown(3f, door3));
        }


    }

    private IEnumerator Moveup(float time, GameObject door)
    {
        Vector3 startingPos = door.transform.position;
        Vector3 finalPos = door.transform.position + (transform.up * 8);
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
        Vector3 finalPos = door.transform.position + (transform.up * 8);
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            door.transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}


