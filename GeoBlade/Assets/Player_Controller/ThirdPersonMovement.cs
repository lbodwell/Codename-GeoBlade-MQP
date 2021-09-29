using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Player_Controller {
    public class ThirdPersonMovement : MonoBehaviour {
        public CharacterController controller;
        public Transform cam;
    
        public Vector3 velocity = new Vector3(0f, 0f, 0f);
        public float movementSpeed = 2f;
        public float turnSmoothingTime = 0.1f;
        public bool isSprinting;
        public bool isJumping;
        public float nextFootstep = 0f;
    
        private const float Gravity = 0.08f;
        private float _turnSmoothingVel;

        private void Update() {
            // TODO: migrate to new input system
            var horizInput = Input.GetAxisRaw("Horizontal");
            var vertInput = Input.GetAxisRaw("Vertical");
            var inputDirection = new Vector3(horizInput, 0f, vertInput).normalized;
		
            if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded) {
                velocity.y = 16f;
                // Jump liftoff
                AkSoundEngine.PostEvent("Player_Jump", gameObject);
                isJumping = true;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && controller.isGrounded) {
                isSprinting = true;
                nextFootstep = Time.time;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift)) {
                isSprinting = false;
            }
        
            velocity.x = movementSpeed * (isSprinting ? 1.5f : 1f);
            velocity.z = movementSpeed * (isSprinting ? 1.5f : 1f);
            
            if (!controller.isGrounded) {
                velocity.y -= Gravity;
            }

            Vector3 finalVel;

            if (inputDirection.magnitude < 0.1f) {
                finalVel = new Vector3(0f, velocity.y, 0f);
            } else {
                var targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                var smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothingVel, turnSmoothingTime);
                transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

                var playerDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                finalVel = new Vector3(velocity.x * playerDirection.x, playerDirection.y + velocity.y, velocity.z * playerDirection.z);
                if (controller.isGrounded) {
                    if (Time.time > nextFootstep) {
                        nextFootstep = Time.time + (isSprinting ? 0.25f : 0.5f);
                        AkSoundEngine.PostEvent("Player_Footstep", gameObject);
                    }
                }
            }
            controller.Move(finalVel * Time.deltaTime);
            
            if (!controller.isGrounded || !isJumping) return;
            isJumping = false;
            velocity.y = 0f;
            // Jump landing
            AkSoundEngine.PostEvent("Player_Jump", gameObject);
            nextFootstep = Time.time;
        }
    }
}