using System;
using UnityEngine;

public class SecurityDroidStats : CharacterStats {
    public GameObject securityDroid;
    
    public override void DamageCharacter(float amount) {
        health = Math.Max(0, health - amount);
        print("health: " + health);
        
        if (health == 0) {
            Destroy(securityDroid);
            PlayerManager.Instance.player.GetComponent<PlayerStats>().ReplenishGeo(10);
        }
    }

    public override void HealCharacter(float amount) {
        health = Math.Min(maxHealth, health + amount);
    }
}