using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTabletUse : MonoBehaviour
{
    private PlayerInventory inventory;
    private GameObject tablet;
    private  GameObject cardSlot1;
    private GameObject cardSlot2;
    public void TabletUse()
    {
            Debug.Log("Executed Use tablet Script");
            
            inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
            tablet = GameObject.Find("demo_tablet (1)");
            cardSlot1 = tablet.transform.Find("keycardslot1").gameObject;
            cardSlot2 = tablet.transform.Find("keycardslot2").gameObject;
            Debug.Log(cardSlot2);

        if (inventory.Has(inventory.GetById("card1")))
            {
                Debug.Log("card1 added");
            inventory.Remove(inventory.GetById("card1"));
                cardSlot1.SetActive(true);
            }

        if (inventory.Has(inventory.GetById("card2")))
            {
                Debug.Log("card2 added");
                inventory.Remove(inventory.GetById("card2"));
                cardSlot2.SetActive(true);
            }
    }
}
