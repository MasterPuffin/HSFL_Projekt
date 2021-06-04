using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockTablet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickable() && gameObject.layer==11)
        {
            // make tablet pickable
            gameObject.layer = 10;
        }
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
}
