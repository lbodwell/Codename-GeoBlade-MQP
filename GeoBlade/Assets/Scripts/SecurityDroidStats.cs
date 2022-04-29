using System;
using UnityEngine;
using UnityEngine.UI;

public class SecurityDroidStats : CharacterStats {
    public GameObject healthBarUI;
    public Slider slider;

    private void Start() {
        health = maxHealth;
        healthBarUI.SetActive(true);
    }

    public override void DamageCharacter(float amount) {
        if (gameObject.GetComponent<SecurityDroidController>().isAggro) {
            health = Math.Max(0, health - amount);

            var playerStats = PlayerManager.Instance.player.GetComponent<PlayerStats>();

            // Incredibly jank way to check for heavy attack. This should not be handled like this.
            if (Math.Abs(amount - 20) < 0.5) {
                playerStats.ConsumeGeo(5);
            }
        
            if (health == 0) {
                Destroy(gameObject);
                playerStats.ReplenishGeo(10);
                playerStats.HealCharacter(10);
            }
        
            Debug.Log("got to damage char -> set health bar active");
            //healthBarUI.SetActive(true);

            slider.value = health / maxHealth;
        }
    }
    
    public override void HealCharacter(float amount) {
        health = Math.Min(maxHealth, health + amount);

        if (health == maxHealth) {
            healthBarUI.SetActive(false);
        }
        
        slider.value = health / maxHealth;
    }
}