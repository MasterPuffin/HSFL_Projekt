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
        private float playerSpeed = 2.0f;
        private float jumpHeight = 1.0f;
        private float gravityValue = -9.81f;

        void Start() {
            controller = GetComponent<CharacterController>();
        }

        public void OnMove(InputValue input) {
            Vector2 inputVec = input.Get<Vector2>();
            moveVec = new Vector3(inputVec.x, 0, inputVec.y);
            Debug.Log(moveVec);
        }

        void Update() {
            if (IsLocalPlayer) {
                MovePlayer();
            }
        }

        private void MovePlayer() {
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0) {
                playerVelocity.y = 0f;
            }

            controller.Move(moveVec * Time.deltaTime * playerSpeed);
            if (moveVec != Vector3.zero) {
                gameObject.transform.forward = moveVec;
            }
        }
    }
}