using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpController : MonoBehaviour
{

    private Rigidbody rb;
    private BoxCollider collid;
    public GameObject player;
    private Transform playerTransform;
    public Transform itemContainer;
    public Transform cardDropContainer;
    public Transform cam;
    public float pickUpRange;
    private bool inHand;
    public static bool slotFull;  
    // so nicht für mehrspieler umsetzbar?!
    private InputAction pickUp;
    private InputAction use;
    private InputAction drop;
    private InputActionMap actionMap;
    public InputActionAsset playerActions;
    private Vector3 distToPlayer;
    private Vector3 distToDrop;
    public GameObject AudioManager;

    // Start is called before the first frame update

    void Awake()
    {
        actionMap = playerActions.FindActionMap("Player");
        pickUp = actionMap.FindAction("PickUp");
        pickUp.performed += context => PickUp();
        use = actionMap.FindAction("Use");
        use.performed += context => UseCard();
        drop = actionMap.FindAction("Drop");
        drop.performed += context => Drop();
    }
    void Start()
    {
        playerTransform = player.GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        collid = GetComponent<BoxCollider>();
        rb.isKinematic = false;
        collid.isTrigger = false;
    }

    // Update is called once per frame

    void Update()
    {
        distToPlayer = playerTransform.position - transform.position;
        distToDrop = cardDropContainer.position - transform.position;
    }

    /*
    if (!inHand && !slotFull && Input.GetKeyDown(KeyCode.F) && distToPlayer.magnitude <= pickUpRange)
    {
        PickUp();
    }
    if (inHand && Input.GetKeyDown(KeyCode.G))
    {
        Drop();
    }

    if (inHand && distToDrop.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.R))
    {
        UseCard();
    }
}
*/
        private void UseCard()
    {
        if (inHand && distToDrop.magnitude <= pickUpRange)
        {
            inHand = false;
            slotFull = false; //when item is used you can pick up another item

            rb.isKinematic = false;
            collid.isTrigger = false;
            transform.SetParent(cardDropContainer);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.localScale = Vector3.one;

            rb.isKinematic = true;
            collid.isTrigger = true;
        }
    }

    private void Drop()
    {
        if (inHand)
        {
            
            inHand = false;
        slotFull = false;        //when item is thrown away you can pick up another item

        rb.isKinematic = false;
        collid.isTrigger = false;

        // make item no longer child of camera 
        transform.SetParent(null);

        //momentum of player to gun + fore of throw
        rb.velocity = player.GetComponent<CharacterController>().velocity/4;
        rb.AddForce(cam.forward * 5,ForceMode.Impulse);
        rb.AddForce(cam.up * 2, ForceMode.Impulse);
        }
    }

    private void PickUp()
    {
        if (!inHand && !slotFull && distToPlayer.magnitude <= pickUpRange)
        {
            inHand = true;
            slotFull = true; //when item is picked up u cant pick up another item
            AudioManager.GetComponent<AudioManager>().PickSomethingUpSound();

            // make item child of camera and move it to hand
            transform.SetParent(itemContainer);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.localScale = Vector3.one;

            rb.isKinematic = true;
            collid.isTrigger = true;
        }
    }
}
