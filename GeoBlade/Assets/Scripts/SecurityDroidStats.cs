using System;
using UnityEngine;
using UnityEngine.UI;

public class SecurityDroidStats : CharacterStats {
    public GameObject healthBarUI;
    public Slider slider;

    private void Start() {
        health = maxHealth;
    }

    public override void DamageCharacter(float amount) {
        health = Math.Max(0, health - amount);
        
        if (health == 0) {
            Destroy(gameObject);
            PlayerManager.Instance.player.GetComponent<PlayerStats>().ReplenishGeo(10);
        }
        
        healthBarUI.SetActive(true);

        slider.value = health / maxHealth;
    }
    
    public override void HealCharacter(float amount) {
        health = Math.Min(maxHealth, health + amount);

        if (health == maxHealth) {
            healthBarUI.SetActive(false);
        }
        
        slider.value = health / maxHealth;
    }
}