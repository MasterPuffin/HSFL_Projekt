using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HelloWorld {
    public class HelloWorldPlayer : NetworkBehaviour {
        public Vector3 moveVec;
        private CharacterController controller;
        private Vector3 playerVelocity;
        
        private bool groundedPlayer;
        
        public float playerSpeed = 2.0f;
        public float jumpHeight = 1.0f;
        
        private float gravityValue = -9.81f;
        private bool jumping = false;

        public float mouseSensitivity = 50.0f;
        public float clampAngle = 80.0f;

        private float rotY = 0.0f; // rotation around the up/y axis
        private float rotX = 0.0f; // rotation around the right/x axis

        private Vector2 mouse;

        void Start() {
            // Camera.main.enabled = false;
            Vector3 rot = transform.localRotation.eulerAngles;
            rotY = rot.y;
            rotX = rot.x;
            controller = GetComponent<CharacterController>();
            
            // Cursor.lockState = CursorLockMode.Locked;
        }

        public void OnMove(InputValue input) {
            Vector2 inputVec = input.Get<Vector2>();
            moveVec = new Vector3(inputVec.x, 0, inputVec.y);
        }

        public void OnJump() {
            jumping = true;
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
}