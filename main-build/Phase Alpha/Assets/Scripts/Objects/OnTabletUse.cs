using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTabletUse : MonoBehaviour
{
    private PlayerInventory inventory;
    private  GameObject cardSlot1;
    private GameObject cardSlot2;
    public void Start()
    {
            Debug.Log("Executed Use tablet Script");
            inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
            cardSlot1 = GameObject.Find("keycardslot1");
            cardSlot2 = GameObject.Find("keycardslot2");

            if (inventory.GetById("card1") != null)
            {
                inventory.Remove(inventory.GetById("card1"));
                cardSlot1.SetActive(true);
            }

            if (inventory.GetById("card2") != null)
            {
                inventory.Remove(inventory.GetById("card2"));
                cardSlot2.SetActive(true);
            }
    }
}
