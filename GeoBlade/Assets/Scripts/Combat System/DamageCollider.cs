using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageEvent : UnityEvent<StatCollider>
{
}
public class DamageCollider : MonoBehaviour
{
    public Collider collider;
    public float damage = 0;
    public bool active = true;
    public DamageEvent onDamage = new DamageEvent();
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger was entered  ");
        OnCollision(other.gameObject);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Collider was hit");
        OnCollision(hit.gameObject);
    }

    private void OnCollision(GameObject otherObject)
    {
        if (active && otherObject.TryGetComponent(out StatCollider other))
        {
            other.stats.DamageCharacter(this.damage);
            Debug.Log("Dealing " + this.damage + "  to " + otherObject.name);
            onDamage?.Invoke(other);
        }
    }

    //TODO: Implement a continuous damage state
}
