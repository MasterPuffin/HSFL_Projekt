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

            Vector3 rot = transform.localRotation.eulerAngles;
            rotY = -rot.y;
            rotX = -rot.x;
            controller = GetComponent<CharacterController>();

            //DEBUG
            // Cursor.lockState = CursorLockMode.Locked;

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
            //Execute onPickup logic if defined
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
            Debug.Log("player used at useable layer");
            UseableItem pi = hit.transform.GetComponent<UseableItem>();
            //Execute onUse logic if defined
            pi.onUse();
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