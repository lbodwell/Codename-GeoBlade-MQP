using UnityEngine;
using UnityEngine.Events;

public class DamageEvent : UnityEvent<StatCollider> {}
public class DamageCollider : MonoBehaviour {
    public Collider collider;
    public float damage = 0;
    public bool active = true;
    public DamageEvent onDamage = new DamageEvent();

    private void OnCollisionEnter(Collision collision) {
        OnCollision(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Trigger was entered");
        OnCollision(other.gameObject);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Debug.Log("Collider was hit");
        OnCollision(hit.gameObject);
    }

    private void OnCollision(GameObject otherObject) {
        if (active && otherObject.TryGetComponent(out StatCollider other)) {
            other.stats.DamageCharacter(damage);
            AkSoundEngine.PostEvent("Player_Attack_Impact", gameObject);
            Debug.Log("Dealing " + damage + "  to " + otherObject.name);
            onDamage?.Invoke(other);
            active = false;
        }
    }

    //TODO: Implement a continuous damage state
}
