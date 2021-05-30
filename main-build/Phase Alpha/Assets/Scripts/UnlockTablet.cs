using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockTablet : MonoBehaviour
{
    public GameObject cardslot1;
    public GameObject cardslot2;
    public GameObject tabletText;
    public bool isPickable = false;
    public GameObject player;
    private PlayerController pc;
    public GameObject AudioManager;
    public GameObject gesperrtText;
    // Start is called before the first frame update
    void Start()
    {
        pc = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cardslot1.transform.childCount == 1 && cardslot2.transform.childCount == 1)
        {
            //highlight Tablet and make Tablet pickable
            isPickable = true;
            if (gesperrtText.activeSelf)
            {
                gesperrtText.SetActive(false);
            }

        }
    }

    public void ShowText()
    {
        tabletText.SetActive(true);
        AudioManager.GetComponent<AudioManager>().DeviceStartUp();
        // block movement of player
    }
    public void HideText()
    {
        tabletText.SetActive(false);
        // unblock movement of player
    }
}
