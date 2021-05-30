using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    public Text text;

    public String questText1 = "Auftrag: Finde das Raumschiff der Crew";
    public String questText2 = "Auftrag: Finde die 2 Keycards und entsperre das Tablet";
    private bool cr_running;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ShowQuest", 2.0f, 30.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetQuest1()
    {
        text.text = questText1;
        ShowQuest();
    }

    private void SetQuest2()
    {
        text.text = questText2;
        StartCoroutine(ShowTextCoroutine());
    }
    private void ShowQuest()
    {
        if (text.text == "")
        {
            text.text = questText1;
        }

        if (!cr_running)
        {
            StartCoroutine(ShowTextCoroutine());
        }
    }

    public void QuestAbgeschlossen()
    {
        text.color = Color.green;
        ShowQuest(); 

    }
    IEnumerator ShowTextCoroutine()
    {
        cr_running = true;
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);
        text.gameObject.SetActive(true);

        //EinblendSound

        //yield on a new YieldInstruction that waits for 10 seconds.
        yield return new WaitForSeconds(10);

        //After we have waited 5 seconds print the time again.
        text.gameObject.SetActive(false);

        //if first quest is done
        if (text.text == questText1 && text.color == Color.green)
        {
            text.color = Color.white;
            SetQuest2();
        }

        //if second quest is done
        if (text.text == questText2 && text.color == Color.green)
        {
            text.text = "Quests done";
        }

        //AusblendSound
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        cr_running = false;
    }
}