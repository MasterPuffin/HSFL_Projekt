using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * This script controls the player movement and view / camera 
 */

public class PlayerController : NetworkBehaviour {
    public Vector3 moveVec;
    private CharacterController controller;
    private Vector3 playerVelocity;

    private bool groundedPlayer;

    public float playerSpeed = 4.0f;
    public float jumpHeight = 1.0f;
    public float maxPickupDistance = 10.0f;

    private float gravityValue = -9.81f;
    private bool jumping = false;

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

            Vector3 rot = transform.localRotation.eulerAngles;
            rotY = -rot.y;
            rotX = -rot.x;
            controller = GetComponent<CharacterController>();

            //DEBUG
            // Cursor.lockState = CursorLockMode.Locked;

            //Enable camera attached to player
            transform.Find("PlayerCamera").gameObject.SetActive(true);

            //Connect to Vivox instance
            vivox = GameObject.Find("Vivox").GetComponent<VivoxInstanceManager>();
            vivox.StartVivox(IsHost ? "Host" : "Client", ngm.vivoxChannel.Value);
        }
    }

    private void OnDestroy() {
        if (IsLocalPlayer) {
            Debug.Log("Logging out of Vivox");
            vivox.EndVivox();
        }
    }

    public void OnMove(InputValue input) {
        Vector2 inputVec = input.Get<Vector2>();
        moveVec = new Vector3(inputVec.x, 0, inputVec.y);
    }

    public void OnJump() {
        jumping = true;
    }

    public void OnPickUp() {
        if (!IsLocalPlayer) return;

        RaycastHit hit;
        //Only detects a hit when the item is on the "Pickupable Items" layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit,
            maxPickupDistance, LayerMask.GetMask("Pickupable Items"))) {
            PickupableItem pi = hit.transform.GetComponent<PickupableItem>();
            //Execute onPickup logic if defined
            if (pi.onPickup != null) {
                Debug.Log("Executing " + pi.onPickup.GetClass());

                //TODO: Is this possible without creating a new gameObject?
                GameObject tempGameObject = new GameObject();
                tempGameObject.AddComponent(pi.onPickup.GetClass());
                Destroy(tempGameObject);
                
                inventory.Add(pi);
            }
            //Delete Object on pickup
            Destroy(pi);
        }
    }

    public void OnCamera(InputValue input) {
        mouse = input.Get<Vector2>();
    }

    void Update() {
        if (IsLocalPlayer) {
            Look();
            MovePlayer();
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
        if (moveVec != Vector3.zero) {
            controller.Move(transform.rotation * moveVec * (Time.deltaTime * playerSpeed));
        }

        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        // Changes the height position of the player..
        if (jumping && groundedPlayer) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            jumping = false;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}