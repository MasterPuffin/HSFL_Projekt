using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    private List<PickupableItem> inventory = new List<PickupableItem>();

    public void Add(PickupableItem item) {
        inventory.Add(item);
    }

    public void Remove(PickupableItem item) {
        inventory.Remove(item);
    }

    public bool Has(PickupableItem item) {
        return inventory.Contains(item);
    }
}