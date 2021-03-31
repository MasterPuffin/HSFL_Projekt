using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HelloWorld {
    public class HelloWorldPlayer : NetworkBehaviour {
        public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.ServerOnly,
            //WritePermission = NetworkVariablePermission.Everyone,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public Vector3 moveVec;
        private CharacterController controller;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
        private float playerSpeed = 2.0f;
        private float jumpHeight = 1.0f;
        private float gravityValue = -9.81f;


        public override void NetworkStart() {
            var cubeRenderer = GetComponent<Renderer>();
            if (IsLocalPlayer) {
                cubeRenderer.material.SetColor("_Color", Color.green);
            } else {
                cubeRenderer.material.SetColor("_Color", Color.red);
            }


            if (IsLocalPlayer) {
                controller = gameObject.AddComponent<CharacterController>();
            }


            Move();
        }

        public void Move() {
            if (NetworkManager.Singleton.IsServer) {
                //var randomPosition = GetRandomPositionOnPlane(Position.Value);
                //transform.position = randomPosition;


                Position.Value = transform.position;
            } else {
                SubmitPositionRequestServerRpc();
            }
        }


        public void OnMove(InputValue input) {
            Vector2 inputVec = input.Get<Vector2>();
            moveVec = new Vector3(inputVec.x, 0, inputVec.y);
            Debug.Log(moveVec);
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default) {
            //Position.Value = GetRandomPositionOnPlane(Position.Value);
            Position.Value = transform.position;
        }

        static Vector3 GetRandomPositionOnPlane(Vector3 position = new Vector3()) {
            //return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
            return position + new Vector3(1, 0, 0);
        }

        void Update() {
            if (IsLocalPlayer) {
                Debug.Log("IsLocalPlayer");
                groundedPlayer = controller.isGrounded;
                if (groundedPlayer && playerVelocity.y < 0) {
                    playerVelocity.y = 0f;
                }

                controller.Move(moveVec * Time.deltaTime * playerSpeed);

                if (moveVec != Vector3.zero) {
                    gameObject.transform.forward = moveVec;
                }

                /*
                // Changes the height position of the player..
                if (Input.GetButtonDown("Jump") && groundedPlayer) {
                    playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                }
                */


                playerVelocity.y += gravityValue * Time.deltaTime;
                controller.Move(playerVelocity * Time.deltaTime);
            }


            Move();
            transform.position = Position.Value;
        }
    }
}