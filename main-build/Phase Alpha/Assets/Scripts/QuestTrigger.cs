using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isTriggered;
    //private GameObject[] Canvas;

    // Update is called once per frame
    void Start()
    {
    
    }

    void Update()
    {
    }

    public void ManualTrigger()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            /*
            Canvas = GameObject.FindGameObjectsWithTag("PlayerCanvas");
            foreach (var obj in Canvas)
            {
                obj.GetComponent<QuestController>().QuestAbgeschlossen();
            }
            */
            GameObject.Find("PlayerCanvas").GetComponent<QuestController>().QuestAbgeschlossen();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered)
        {
            isTriggered = true;
            GameObject.Find("PlayerCanvas").GetComponent<QuestController>().QuestAbgeschlossen();
        }

    }
}
