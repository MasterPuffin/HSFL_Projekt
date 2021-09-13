using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

/*
 * This script controls the player movement and view / camera 
 */

public class PlayerController : NetworkBehaviour {
    private Vector3 moveVec;
    private CharacterController controller;
    private Vector3 playerVelocity;

    private bool groundedPlayer;
    private float distToGround;

    public float playerSpeed = 4.0f;
    public float jumpHeight = 1.0f;
    public float maxPickupDistance = 10.0f;

    private float gravityValue = -9.81f;

    private bool jumping = false;
    public bool pulling = false;
    private bool running = false;
    private bool crouching = false;
    private bool moving = false;
    private CapsuleCollider collid;
    float normalHeight;
    public float reducedHeight;
    public AudioSource fastWalking;
    public AudioSource slowWalking;

    private bool walkingActive = false;

    // get player animator
    private Animator animator;

    //Vector to continue the direction while jumping
    private Vector3 jumpMoveVec;

    public float mouseSensitivity = 30.0f;
    public float clampAngle = 80.0f;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    private Vector2 mouse;

    private VivoxInstanceManager vivox;
    private NetworkedGameManager ngm;
    private PlayerInventory inventory;

    void Start() {
        if (IsLocalPlayer) {
            ngm = GameObject.Find("NetworkedGameManager").GetComponent<NetworkedGameManager>();
            inventory = GetComponent<PlayerInventory>();

            animator = transform.GetChild(0).GetComponent<Animator>();

            //TODO: When enabled the player falls through the map
             slowWalking.volume = 0.0f;
             fastWalking.volume = 0.0f;

            Vector3 rot = transform.localRotation.eulerAngles;
            rotY = -rot.y;
            rotX = -rot.x;
            controller = GetComponent<CharacterController>();

            //DEBUG
            // Cursor.lockState = CursorLockMode.Locked;

            //get collider of player
            //TODO: When enabled the camera is not working
            // collid = GetComponent<CapsuleCollider>();
            // normalHeight = collid.height;
            // reducedHeight = 0.5f;

            //Enable camera attached to player
            transform.Find("PlayerCamera").gameObject.SetActive(true);
            distToGround = GetComponent<Collider>().bounds.extents.y;

            //Connect to Vivox instance
            vivox = GameObject.Find("Vivox").GetComponent<VivoxInstanceManager>();
            vivox.StartVivox(IsHost ? "Host" : "Client", ngm.vivoxChannel.Value);

            //Teleports player to spawn position
            //Offsets the position by a small random amount to prevent players standing in each
            //other when spawning
            TeleportPlayer(
                GameObject.Find("Network/PlayerSpawnPosition").transform.position +
                new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 2.0f)
            );
        }
    }

    private void OnDestroy() {
        if (IsLocalPlayer) {
            Debug.Log("Logging out of Vivox");
            vivox.EndVivox();
        }
    }

    public void OnMove(InputValue input) {
        
        if (!moving) {
            moving = true;
            Debug.Log("start moving");
        } else {
            moving = false;
            Debug.Log("stop moving");
        }
        

        Vector2 inputVec = input.Get<Vector2>();
        moveVec = new Vector3(inputVec.x, 0, inputVec.y);
    }

    public void OnJump() {
        if (groundedPlayer) {
            jumping = true;
            jumpMoveVec = moveVec;
        }
    }

    public void OnPickUp() {
        if (!IsLocalPlayer) return;

        RaycastHit hit;
        //Only detects a hit when the item is on the "Pickupable Items" layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit,
            maxPickupDistance, LayerMask.GetMask("Pickupable Items"))) {
            PickupableItem pi = hit.transform.GetComponent<PickupableItem>();
            //Delete Object on pickup
            Destroy(hit.collider.gameObject);
            pi.onPickup();
            inventory.Add(pi);
        }
    }

    public void OnUse() {
        //Debug.Log("player tries to use");

        if (!IsLocalPlayer) return;

        RaycastHit hit;
        //Only detects a hit when the item is on the "Usableable Objects" layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit,
            maxPickupDistance, LayerMask.GetMask("Useable"))) {
            Debug.Log("player used at usable layer");
            hit.transform.GetComponent<UseableItem>().onUse();
        }
    }

    public void OnPull() {
        if (!pulling) {
            pulling = true;
        } else {
            pulling = false;
        }

        Debug.Log("pulling");
    }

    //TODO: Not working because collider is not working
    public void OnCrouch() {
        if (!crouching) {
            Debug.Log("crouching");
            //reduce height
            // collid.height = reducedHeight;
            crouching = true;
        } else {
            Debug.Log("not crouching");
            //add height
            // collid.height = normalHeight;
            crouching = false;
        }
    }

    public void OnRunning() {
        if (!running) {
            Debug.Log("running");
            playerSpeed += 5;
            running = true;
            fastWalking.volume = 1.0f;
        } else {
            Debug.Log("not running");
            playerSpeed -= 5;
            running = false;
            fastWalking.volume = 0.0f;
        }
    }

    public void OnCamera(InputValue input) {
        mouse = input.Get<Vector2>();
    }

    void Update() {
        if (IsLocalPlayer) {
            Look();
            MovePlayer();

            //TODO: This is inefficient way beyond belief. Please fix this abomination
            //TODO: Doesn't work, because the player has to animation component
            /*
            if (!moving && !crouching && !running) {
                animator.SetBool("isCrouchWalking", false);
                animator.SetBool("isCrouchIdling", false);
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isIdling", true);
            } else if (moving && !crouching && !running) {
                animator.SetBool("isIdling", false);
                animator.SetBool("isCrouchWalking", false);
                animator.SetBool("isCrouchIdling", false);
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", true);
            } else if (moving && !crouching && running) {
                animator.SetBool("isIdling", false);
                animator.SetBool("isCrouchWalking", false);
                animator.SetBool("isCrouchIdling", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true);
            } else if (!moving && crouching) {
                animator.SetBool("isIdling", false);
                animator.SetBool("isCrouchWalking", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                animator.SetBool("isCrouchIdling", true);
            } else if (moving && crouching) {
                animator.SetBool("isIdling", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                animator.SetBool("isCrouchIdling", false);
                animator.SetBool("isCrouchWalking", true);
            }
            */
        }
    }

    private void Look() {
        rotY += mouse.x * mouseSensitivity * Time.deltaTime;
        rotX += mouse.y * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY * -1, 0.0f);
        transform.rotation = localRotation;
    }

    private void MovePlayer() {
        if (!groundedPlayer) {
            controller.Move(transform.rotation * jumpMoveVec * (Time.deltaTime * playerSpeed));
        } else {
            if (moveVec != Vector3.zero) {
                controller.Move(transform.rotation * moveVec * (Time.deltaTime * playerSpeed));
            }
        }

        groundedPlayer = controller.isGrounded;
        if (!groundedPlayer) {
            //Check again via Raycast as the player Controller is buggy
            groundedPlayer = IsGrounded();
        }

        if (groundedPlayer && playerVelocity.y < 0.1f) {
            playerVelocity.y = 0f;
        }

        // Changes the height position of the player..
        if (jumping && groundedPlayer) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            jumping = false;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (jumping == false) {
            if (!walkingActive && moving) {
                slowWalking.Play();
                walkingActive = true;
                slowWalking.volume = 1.0f;
            }

            if (walkingActive && !moving) {
                slowWalking.Stop();
                slowWalking.volume = 0.0f;
                walkingActive = false;
            }
        } else {
            slowWalking.volume = 0.0f;
        }
    }

    //Teleports a player as the setting of transform.position is only possible if
    //the Character Controller is disabled
    private void TeleportPlayer(Vector3 position) {
        CharacterController cc = GetComponent<CharacterController>();

        cc.enabled = false;
        transform.position = position;
        cc.enabled = true;
    }

    private bool IsGrounded() {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity);
        // Debug.Log(hit.distance-distToGround);
        return hit.distance - distToGround < 0.35f;
    }
}