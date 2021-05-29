using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockTablet : MonoBehaviour
{
    public GameObject cardslot1;
    public GameObject cardslot2;
    public GameObject tabletText;
    public bool isPickable = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cardslot1.transform.childCount == 1 && cardslot2.transform.childCount == 1)
        {
            //highlight Tablet and make Tablet pickable
            isPickable = true;
        }
    }

    public void ShowText()
    {
        tabletText.SetActive(true);
    }
    public void HideText()
    {
        tabletText.SetActive(false);
    }
}
