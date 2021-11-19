using System;
using HoudiniEngineUnity;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public CharacterController controller;
    public float movementSpeed = 2f;
    public float chaseRadius = 10f;
    public float attackRadius = 5f;
    public float turnSmoothingTime = 0.2f;
    public float attackCooldown = 2f;

    private Transform _target;
    private float _turnSmoothingVel;
    private float _nextAttackAttempt;

    private void Start() {
        _target = PlayerManager.Instance.player.transform;
    }

    private void OnEnable() {
        PlayerController.OnPlayerAttack += Test;
    }
    
    private void OnDisable() {
        PlayerController.OnPlayerAttack += Test;
    }

    private void Test() {
        print("Test");
    }

    private void Update() {
        var dist = Vector3.Distance(_target.position, transform.position);
        
        if (dist <= chaseRadius) {
            // TODO: Re-write this. Target angle is always off by 90 degrees for some reason, so I hardcoded -90 for now.
            var dir = (_target.position - transform.position).normalized;
            var targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg - 90;
            var smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothingVel, turnSmoothingTime);
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
            
            if (dist <= attackRadius) {
                if (Time.time >= _nextAttackAttempt) {
                    // Attack player
                    PlayerManager.Instance.player.GetComponent<PlayerStats>().DamageCharacter(10);
                    _nextAttackAttempt = Time.time + attackCooldown;
                }
            } else {
                controller.SimpleMove(new Vector3(dir.x * movementSpeed, dir.y, dir.z * movementSpeed));
            }
        }
    }

    private void OnDrawGizmosSelected() {
        var pos = transform.position;
        
        // Debug targeting volumes
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, chaseRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, attackRadius);
    }
}
