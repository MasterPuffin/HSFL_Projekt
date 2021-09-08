using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBlocksOnPlate : MonoBehaviour
{
    private PlayerInventory inventory;
    private bool isPlaced;
    public void Pressed()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        if (inventory.Has(inventory.GetById("block1")) && !isPlaced)
        {
            Debug.Log("block added");
            inventory.Remove(inventory.GetById("block1"));
            transform.GetChild(0).gameObject.SetActive(true);
            isPlaced = true;
        }
        if (inventory.Has(inventory.GetById("block2")) && !isPlaced)
        {
            Debug.Log("block added");
            inventory.Remove(inventory.GetById("block2"));
            transform.GetChild(0).gameObject.SetActive(true);
            isPlaced = true;
        }
    }
}
