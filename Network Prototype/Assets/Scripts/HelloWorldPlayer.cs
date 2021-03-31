using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HelloWorld {
    public class HelloWorldPlayer : NetworkBehaviour {
        private CharacterController controller;
        private Vector3 playerVelocity;
        private Vector3 moveVec;
        private bool groundedPlayer;
        private float playerSpeed = 2.0f;
        private float jumpHeight = 1.0f;
        private float gravityValue = -9.81f;


        public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public override void NetworkStart() {
            controller = gameObject.AddComponent<CharacterController>();
            Move();
        }

        public void Update() {
            Move();
        }

        public void Move() {
            if (NetworkManager.Singleton.IsServer) {
                /*
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                Position.Value = randomPosition;
                */
                DoMove();
            } else {
                SubmitPositionRequestServerRpc();
            }
        }

        //Movement
        public void OnJump() {
            Debug.Log("jump");
        }

        public void OnMove(InputValue input) {
            Vector2 inputVec = input.Get<Vector2>();

            moveVec = new Vector3(inputVec.x, 0, inputVec.y);
            Debug.Log(moveVec);
        }

        public void DoMove() {
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0) {
                playerVelocity.y = 0f;
            }

            controller.Move(moveVec * Time.deltaTime * playerSpeed);

            if (moveVec != Vector3.zero) {
                gameObject.transform.forward = moveVec;
            }

            // Changes the height position of the player..
            /*
            if (Input.GetButtonDown("Jump") && groundedPlayer) {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
            */

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default) {
            Position.Value = GetRandomPositionOnPlane();
        }

        static Vector3 GetRandomPositionOnPlane() {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }
    }
}