using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PullController : MonoBehaviour
{

    private Vector3 lastPos;
    private Transform myTransform;


    public bool pushingRock = false;

    void Start()
    {
        myTransform = transform;
        lastPos = myTransform.position;
    }

    void Update()
    {
        if (myTransform.position != lastPos)
        {
            pushingRock = true;
            lastPos = myTransform.position;
        }
        else
            pushingRock = false;


    }


    private void OnCollisionStay(Collision other)
    {

        if (other.gameObject.CompareTag("Player"))  
        {
            if (other.gameObject.GetComponent<PlayerController>().pulling)
            {
                Debug.Log("e");
                Rigidbody rb = this.GetComponent<Rigidbody>();
                Vector3 direction = other.transform.position - this.transform.position;
                rb.velocity = direction;
            }
        }

    }

    private void OnCollisionExit(Collision other)
    {


        if (other.gameObject.CompareTag("Player"))//|| Input.GetKeyUp("e")
        {
            pushingRock = false;
            Debug.Log("stop");
            Rigidbody rb = this.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

    }

}

