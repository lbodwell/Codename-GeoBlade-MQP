using UnityEngine;

public class SecurityDroidController : MonoBehaviour {
    public CharacterController controller;
    public SecurityDroidStats securityDroidStats;
    public float movementSpeed = 2f;
    public float chaseRadius = 10f;
    public float attackRadius = 5f;
    public float turnSmoothingTime = 0.2f;
    public float attackCooldown = 3f;
    public float attackGracePeriod = 1f;
    public float attackDamage = 10f;
    public bool isAggro;

    private GameObject _player;
    private Transform _target;
    private Collider _playerWeaponCollider;
    private bool _isInAttackRange;
    private float _turnSmoothingVel;
    private float _nextAttackAttempt;

    private void Start() {
        _target = PlayerManager.Instance.player.transform;
        _playerWeaponCollider = PlayerManager.Instance.geoBlade.GetComponent<Collider>();
    }
    
    private void Update() {
        if (isAggro && _target != null) {
            var dist = Vector3.Distance(_target.position, transform.position);

            if (dist <= chaseRadius) {
                // TODO: Re-write this. Target angle is always off by 90 degrees for some reason, so I hardcoded -90 for now.
                var dir = (_target.position - transform.position).normalized;
                var targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg - 90;
                var smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothingVel,
                    turnSmoothingTime);
                transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

                if (dist <= attackRadius) {
                    if (!_isInAttackRange) {
                        _nextAttackAttempt = Time.time + attackGracePeriod;
                    }

                    _isInAttackRange = true;
                    if (Time.time >= _nextAttackAttempt) {
                        PlayerManager.Instance.player.GetComponent<PlayerStats>().DamageCharacter(attackDamage);
                        AkSoundEngine.PostEvent("Security_Droid_Attack", gameObject);
                        _nextAttackAttempt = Time.time + attackCooldown;
                    }
                } else {
                    _isInAttackRange = false;
                    controller.SimpleMove(new Vector3(dir.x * movementSpeed, dir.y, dir.z * movementSpeed));
                }
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
