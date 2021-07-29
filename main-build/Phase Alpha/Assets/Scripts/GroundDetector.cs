using System.Collections.Generic;
using UnityEngine;

/*
 * Ground detector
 * Author: Johannes Bluhm
 * Checks if the object is in contact with the ground. Used for the player to enable / disable jumping and the
 * cubes so that the dragging sound isn't played when the cube isn't mid-air.
 */

public class GroundDetector : MonoBehaviour {
    //List of ground objects
    private List<GameObject> currentCollisions = new List<GameObject>();
    //If the object is grounded. Used by other classes to check the grounded state
    public bool grounded = false;

    private void OnCollisionEnter(Collision hit) {
        //Check if collision is a ground object and player contacts the top of it
        //This value has to be rounded as is can be slightly below 1 on some occurrences
        Debug.Log("enter collision");
        if (Mathf.Round(hit.contacts[0].normal.y) == 1) {
            //Add to collision list
            currentCollisions.Add(hit.gameObject);
            grounded = true;
        }
    }

    // Detect collision exit with floor
    private void OnCollisionExit(Collision hit) {
        //if (hit.gameObject.CompareTag("Ground")) {
            //Remove collision
            currentCollisions.Remove(hit.gameObject);
            //Check if player has any ground collisions
            if (currentCollisions.Count == 0) {
                grounded = false;
            }
        //}
    }
}