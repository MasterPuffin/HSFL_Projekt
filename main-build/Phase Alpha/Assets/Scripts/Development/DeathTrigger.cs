using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour {
    
    //Debug only
    //This trigger calls the KillPlayer function 
    private void OnTriggerEnter(Collider other) {
        var pc = other.gameObject.GetComponent<PlayerController>();
        pc.KillPlayer();
    }
}