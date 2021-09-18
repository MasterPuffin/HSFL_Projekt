using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTrigger : MonoBehaviour {
    
    //This trigger creates a save manager and saves the players current stats
    private void OnTriggerEnter(Collider other) {
        SaveManager sm = gameObject.AddComponent<SaveManager>();
        sm.Save(other);
        Destroy(sm);
    }
}