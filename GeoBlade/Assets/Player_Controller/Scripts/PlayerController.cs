using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour {
    public delegate void AttackAction(float damage);
    public static event AttackAction OnPlayerAttack;
    public PlayerStats playerStats;
    public CharacterController controller;
    public Animator animator;
    public Transform cam;
    public float gravity = 0.1f;
    public float movementSpeed = 0.5f;
    public float turnSmoothingTime = 0.1f;
    // TODO: encapsulate timeouts within Attack class for more customizability
    public float attackCooldown = 0.5f;
    public float comboTimeout = 0.75f;
    public float attackInactivityTimeout = 5.0f;
    public DamageCollider weaponCollider;
    public bool isAttacking;

    private List<Attack> _attacks;
    private Vector3 _velocity = new Vector3(0f, 0f, 0f);
    private bool _isSprinting;
    private bool _isJumping;
    private bool _isWeaponActive;
    private float _nextFootstep;
    private float _nextAttackWindowStart;
    private float _nextAttackWindowClose;
    private float _nextWeaponSheathe;
    private int _nextAttackIndex;
    private float _turnSmoothingVel;
    private Vector2 _movementInput = new Vector2(0f, 0f);

    private void Awake() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
    
    private async void Start() {
        // TODO: Use enums for attack names
        _attacks = new List<Attack> {
            new Attack("Light1", 5),
            new Attack("Light2", 10),
            new Attack("Heavy", 20)
        };

        if (DialogueManager.Instance != null) {
            await DialogueManager.Instance.PlayDialogueSequence("lvl1_stasis_room_seru_01");
        }
    }

    private void OnDestroy() {
        AkSoundEngine.StopAll();
        
        //weaponCollider.onDamage += (other) => {}
    }

    private void knockback(StatCollider other)
    {
        
    }

    private void Update() {
        AkSoundEngine.SetState("Material", "Concrete");
        
        var horizInput = _movementInput.x;
        var vertInput = _movementInput.y;
        var inputDirection = new Vector3(horizInput, 0f, vertInput).normalized;

        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            _isSprinting = false;
        }
        
        _velocity.x = movementSpeed * (_isSprinting ? 1.5f : 1f);
        _velocity.z = movementSpeed * (_isSprinting ? 1.5f : 1f);
        
        if (!controller.isGrounded) {
            _velocity.y -= gravity * Time.deltaTime;
        }

        Vector3 finalVel;

        if (inputDirection.magnitude < 0.1f) {
            finalVel = new Vector3(0f, _velocity.y, 0f);
        } else {
            var targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            var smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothingVel, turnSmoothingTime);
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            var playerDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            finalVel = new Vector3(_velocity.x * playerDirection.x, playerDirection.y + _velocity.y, _velocity.z * playerDirection.z);
            if (controller.isGrounded) {
                if (Time.time > _nextFootstep) {
                    // TODO: Make this dynamic
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
        
        controller.Move(finalVel * Time.deltaTime);

        if (_isWeaponActive && Time.time > _nextWeaponSheathe) {
            _isWeaponActive = false;
            Debug.Log("GeoBlade sheathed");
        }
        
        // Update Animator Parameters
        Vector3 horizontalVel = new Vector3(finalVel.x, finalVel.z, 0);
        
        animator.SetBool("Grounded", controller.isGrounded);
        animator.SetFloat("Speed", horizontalVel.magnitude);
        
        if (Time.time >= _nextAttackWindowClose && animator.GetInteger("Combo") != 0)
        {
            animator.SetInteger("Combo", 0);
            //Debug.Log("Combo set to " + animator.GetInteger("Combo"));
        }

        // Triggered when animator switches states (for several frames)
        if (animator.IsInTransition(0))
        {
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0);
            if (nextState.IsName("Run")) // Set animation speed to player speed
                animator.speed = movementSpeed / 12;
            else
                animator.speed = 1;
        }

        // TODO: Is there any reason this is so scuffed?
        if (!controller.isGrounded || !_isJumping) return;
        _isJumping = false;
        _velocity.y = 0f;
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
        
        _velocity.y = 10f;
        // Jump liftoff
        AkSoundEngine.PostEvent("Player_Jump", gameObject);
        _isJumping = true;
    }

    public void Attack(InputAction.CallbackContext context) {
        if (_isWeaponActive) {
            if (Time.time < _nextAttackWindowStart) return;
            if (Time.time < _nextAttackWindowClose) {
                _nextAttackIndex = (_nextAttackIndex + 1) % 3;
                animator.SetInteger("Combo", (_nextAttackIndex + 1) % 3);
                //Debug.Log("Combo set to " + animator.GetInteger("Combo"));
            } else {
                _nextAttackIndex = 0;
                animator.SetInteger("Combo", 1);
                //Debug.Log("Combo set to " + animator.GetInteger("Combo"));
            }
        } else {
            _isWeaponActive = true;
            _nextAttackIndex = 0;
            
            Debug.Log("GeoBlade unsheathed");
            AkSoundEngine.PostEvent("Player_Unsheathe", gameObject);
            animator.SetInteger("Combo", 1);
            //Debug.Log("Combo set to " + animator.GetInteger("Combo"));
        }

        if (_nextAttackIndex == 2 && playerStats.geo < 5) {
            _nextAttackIndex = 0;
        }
        
        var currAttack = _attacks[_nextAttackIndex];

        _nextAttackWindowStart = Time.time + attackCooldown;
        _nextAttackWindowClose = Time.time + comboTimeout;
        _nextWeaponSheathe = Time.time + attackInactivityTimeout;

        //Debug.Log($"Attack (type: {currAttack.Name}, damage: {currAttack.Damage})");
        
        AkSoundEngine.SetState("Attack_Type", currAttack.Name);
        AkSoundEngine.PostEvent("Player_Attack", gameObject);
        
        OnPlayerAttack?.Invoke(currAttack.Damage);
        isAttacking = true;
        StartCoroutine(ResetAttacking());
        //OnPlayerAttack?.Invoke(currAttack.Damage); TODO: Make less jenk
        weaponCollider.damage = currAttack.Damage;
        
        if (_nextAttackIndex == 2) {
            playerStats.ConsumeGeo(5);
        }
    }

    public void Sprint(InputAction.CallbackContext context) {
        _isSprinting = true;
        _nextFootstep = Time.time;
    }
    
    // This is janky
    private IEnumerator ResetAttacking() {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
}

public class Attack {
    public readonly string Name;
    public readonly float Damage;

    public Attack(string name, float damage) {
        Name = name;
        Damage = damage;
    }
}