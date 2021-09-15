using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour {
    
    //Debug only
    //This trigger calls the KillPlayer function 
    private void OnTriggerEnter(Collider other) {
        var player = GameObject.FindWithTag("Player");
        var pc = player.GetComponent<PlayerController>();
        pc.KillPlayer();
    }
}