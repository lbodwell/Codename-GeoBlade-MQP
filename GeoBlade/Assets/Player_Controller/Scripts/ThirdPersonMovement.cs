using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Player_Controller.Scripts
{
    public class ThirdPersonMovement : MonoBehaviour
    {
        public CharacterController controller;
        public Animator animator;
        public Transform cam;
        public List<Attack> attacks;

        public Vector3 velocity = new Vector3(0f, 0f, 0f);
        public float movementSpeed = 2f;
        public float turnSmoothingTime = 0.1f;
        public bool isSprinting;
        public bool isJumping;
        public bool isWeaponActive = false;
        public float nextFootstep = 0f;
        public float nextAttackWindowStart = 0f;
        public float nextAttackWindowClose = 0f;
        public float nextWeaponSheathe = 0f;

        public int nextAttackIndex = 0;

        // TODO: encapsulate timeouts within Attack class for more customizability
        public const float AttackCooldown = 0.5f;
        public const float ComboTimeout = 1.0f;
        public const float AttackInactivityTimeout = 5.0f;

        private const float Gravity = 0.08f;
        private float _turnSmoothingVel;
        private Vector2 movementInput = new Vector2(0, 0);

        private void Start()
        {
            attacks = new List<Attack>
            {
                new Attack("Light1", 5),
                new Attack("Light2", 10),
                new Attack("Heavy", 20)
            };
        }

        private void Update()
        {
            AkSoundEngine.SetState("Material", "Concrete");
            // TODO: migrate to new input system
            var horizInput = movementInput.x;
            var vertInput = movementInput.y;
            var inputDirection = new Vector3(horizInput, 0f, vertInput).normalized;

            if (controller.isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    isSprinting = true;
                    nextFootstep = Time.time;
                }

                // TODO: clean up
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                isSprinting = false;
            }

            velocity.x = movementSpeed * (isSprinting ? 1.5f : 1f);
            velocity.z = movementSpeed * (isSprinting ? 1.5f : 1f);

            if (!controller.isGrounded)
            {
                velocity.y -= Gravity;
            }

            Vector3 finalVel;

            if (inputDirection.magnitude < 0.1f)
            {
                finalVel = new Vector3(0f, velocity.y, 0f);
            }
            else
            {
                var targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                var smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothingVel,
                    turnSmoothingTime);
                transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

                var playerDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                finalVel = new Vector3(velocity.x * playerDirection.x, playerDirection.y + velocity.y,
                    velocity.z * playerDirection.z);
                if (controller.isGrounded)
                {
                    if (Time.time > nextFootstep)
                    {
                        if (isSprinting)
                        {
                            AkSoundEngine.SetState("Footstep_Type", "Run");
                            nextFootstep = Time.time + 0.25f;
                        }
                        else
                        {
                            AkSoundEngine.SetState("Footstep_Type", "Walk");
                            nextFootstep = Time.time + 0.25f;
                        }

                        AkSoundEngine.PostEvent("Player_Footstep", gameObject);
                    }
                }
            }

            controller.Move(finalVel * Time.deltaTime);

            if (isWeaponActive && Time.time > nextWeaponSheathe)
            {
                isWeaponActive = false;
                Debug.Log("Geoblade sheathed");
            }

            //Update Animator Parameters
            animator.SetBool("Grounded", controller.isGrounded);
            animator.SetFloat("Speed", finalVel.magnitude);

            if (!controller.isGrounded || !isJumping) return;
            isJumping = false;
            velocity.y = 0f;
            // Jump landing
            AkSoundEngine.PostEvent("Player_Jump", gameObject);
            nextFootstep = Time.time;
        }

        public void Move(InputAction.CallbackContext context)
        {
            Vector2 inputVector = context.ReadValue<Vector2>();

            movementInput = new Vector2(inputVector.x, inputVector.y);
        }

        public void Jump(InputAction.CallbackContext context)
        {
            if (controller.isGrounded)
            {
                velocity.y = 12f;
                // Jump liftoff
                AkSoundEngine.PostEvent("Player_Jump", gameObject);
                isJumping = true;
            }
        }

        public void Attack(InputAction.CallbackContext context)
        {
            if (Time.time > nextAttackWindowStart)
            {
                var currAttack = attacks[nextAttackIndex];

                if (isWeaponActive)
                {
                    if (Time.time < nextAttackWindowClose)
                    {
                        nextAttackIndex = (nextAttackIndex + 1) % 3;
                    }
                    else
                    {
                        nextAttackIndex = 0;
                    }
                }
                else
                {
                    isWeaponActive = true;
                    Debug.Log("Geoblade unsheathed");
                    AkSoundEngine.PostEvent("Player_Unsheathe", gameObject);
                    nextAttackIndex = 0;
                }

                nextAttackWindowStart = Time.time + AttackCooldown;
                nextAttackWindowClose = Time.time + ComboTimeout;
                nextWeaponSheathe = Time.time + AttackInactivityTimeout;

                Debug.Log($"Attack (type: {currAttack.name}, damage: {currAttack.damage})");
                AkSoundEngine.SetState("Attack_Type", currAttack.name);
                AkSoundEngine.PostEvent("Player_Attack", gameObject);
            }
        }

        public void Sprint(InputAction.CallbackContext context)
        {
        }
    }

    public class Attack
    {
        public string name;
        public float damage;

        public Attack(string name, float damage)
        {
            this.name = name;
            this.damage = damage;
        }
    }
}