using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Player_Controller.Scripts {
    public class ThirdPersonMovement : MonoBehaviour {
        public CharacterController controller;
        public Animator animator;
        public Transform cam;
        private List<Attack> attacks;
    
        public Vector3 velocity = new Vector3(0f, 0f, 0f);
        public float movementSpeed = 0.5f;
        public float turnSmoothingTime = 0.5f;

        // TODO: encapsulate timeouts within Attack class for more customizability
        public const float AttackCooldown = 0.75f;
        public const float ComboTimeout = 1.0f;
        public const float AttackInactivityTimeout = 5.0f;
        private const float Gravity = 0.24f;
        
        private bool _isSprinting;
        private bool _isJumping;
        private bool _isWeaponActive;
        private float _nextFootstep;
        private float _nextAttackWindowStart;
        private float _nextAttackWindowClose;
        private float _nextWeaponSheathe;
        private int _nextAttackIndex;
        private float _turnSmoothingVel;
        private Vector2 _movementInput = new Vector2(0, 0);

        private void Start() {
            attacks = new List<Attack> {
                new Attack("Light1", 5),
                new Attack("Light2", 10),
                new Attack("Heavy", 20)
            };
        }

        private void Update() {
            AkSoundEngine.SetState("Material", "Concrete");
            
            var horizInput = _movementInput.x;
            var vertInput = _movementInput.y;
            var inputDirection = new Vector3(horizInput, 0f, vertInput).normalized;

            if (Input.GetKeyUp(KeyCode.LeftShift)) {
                _isSprinting = false;
            }
            
            velocity.x = movementSpeed * (_isSprinting ? 1.5f : 1f);
            velocity.z = movementSpeed * (_isSprinting ? 1.5f : 1f);
            
            if (!controller.isGrounded) {
                velocity.y -= Gravity;
            }
            print(velocity.y);

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
                    if (Time.time > _nextFootstep) {
                        if (_isSprinting) {
                            AkSoundEngine.SetState("Footstep_Type", "Run");
                            _nextFootstep = Time.time + 0.225f;
                        } else {
                            AkSoundEngine.SetState("Footstep_Type", "Walk");
                            _nextFootstep = Time.time + 0.275f;
                        }

                        AkSoundEngine.PostEvent("Player_Footstep", gameObject);
                    }
                }
            }

            // Move the player controller using deltatime
            controller.Move(finalVel * Time.deltaTime);

            if (_isWeaponActive && Time.time > _nextWeaponSheathe) {
                _isWeaponActive = false;
                Debug.Log("Geoblade sheathed");
                animator.SetInteger("Combo", 0);
                Debug.Log("Combo set to " + animator.GetInteger("Combo"));
            }
            
            //Update Animator Parameters
            animator.SetBool("Grounded", controller.isGrounded);
            animator.SetFloat("Speed", finalVel.magnitude);

            if (Time.time >= _nextAttackWindowClose && animator.GetInteger("Combo") != 0)
            {
                animator.SetInteger("Combo", 0);
                Debug.Log("Combo set to " + animator.GetInteger("Combo"));
            }
            
            if (!controller.isGrounded || !_isJumping) return;
            _isJumping = false;
            velocity.y = 0f;
            // Jump landing
            AkSoundEngine.PostEvent("Player_Jump", gameObject);
            _nextFootstep = Time.time;
        }

        public void Move(InputAction.CallbackContext context) {
            var inputVector = context.ReadValue<Vector2>();

            _movementInput = new Vector2(inputVector.x, inputVector.y);
        }

        public void Jump(InputAction.CallbackContext context) {
            if (!controller.isGrounded) return;
            
            velocity.y = 16f;
            // Jump liftoff
            AkSoundEngine.PostEvent("Player_Jump", gameObject);
            _isJumping = true;
        }

        public void Attack(InputAction.CallbackContext context) {
            // TODO: clean up
            
            animator.SetInteger("Combo", (_nextAttackIndex + 1) % 3);
            Debug.Log("Combo set to " + animator.GetInteger("Combo"));
            
            if (_isWeaponActive) {
                if (Time.time < _nextAttackWindowStart) return;
                if (Time.time < _nextAttackWindowClose) {
                    _nextAttackIndex = (_nextAttackIndex + 1) % 3;
                    animator.SetInteger("Combo", (_nextAttackIndex + 1) % 3);
                    Debug.Log("Combo set to " + animator.GetInteger("Combo"));
                } else {
                    _nextAttackIndex = 0;
                    animator.SetInteger("Combo", 1);
                    Debug.Log("Combo set to " + animator.GetInteger("Combo"));
                }
            } else {
                _isWeaponActive = true;
                _nextAttackIndex = 0;
                
                Debug.Log("Geoblade unsheathed");
                AkSoundEngine.PostEvent("Player_Unsheathe", gameObject);
                animator.SetInteger("Combo", 1);
                Debug.Log("Combo set to " + animator.GetInteger("Combo"));
            }

            var currAttack = attacks[_nextAttackIndex];

            _nextAttackWindowStart = Time.time + AttackCooldown;
            _nextAttackWindowClose = Time.time + ComboTimeout;
            _nextWeaponSheathe = Time.time + AttackInactivityTimeout;

            Debug.Log($"Attack (type: {currAttack.name}, damage: {currAttack.damage})");
            
            AkSoundEngine.SetState("Attack_Type", currAttack.name);
            AkSoundEngine.PostEvent("Player_Attack", gameObject);
        }

        public void Sprint(InputAction.CallbackContext context) {
            _isSprinting = true;
            _nextFootstep = Time.time;
        }
    }

    public class Attack {
        public string name;
        public float damage;

        public Attack(string name, float damage) {
            this.name = name;
            this.damage = damage;
        }
    }
}