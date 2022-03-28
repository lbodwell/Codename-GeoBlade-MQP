using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : CharacterStats {
    public float maxGeo;
    public float geo;
    
    public override async void DamageCharacter(float amount) {
        health = Math.Max(0, health - amount);
        HUDManager.Instance.UpdateBar(HUDManager.BarType.Health, health / maxHealth);
        
        if (health == 0) {
            // TODO: make this more robust
            Debug.Log("Game over!");
            Destroy(gameObject);
            AkSoundEngine.StopAll();
            await DialogueManager.Instance.CancelDialogue();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public override void HealCharacter(float amount) {
        health = Math.Min(maxHealth, health + amount);
        HUDManager.Instance.UpdateBar(HUDManager.BarType.Health, health / maxHealth);
    }
    
    public void ConsumeGeo(float amount) {
        geo = Math.Max(0, geo - amount);
        HUDManager.Instance.UpdateBar(HUDManager.BarType.Geo, geo / maxGeo);
    }

    public void ReplenishGeo(float amount) {
        geo = Math.Min(maxGeo, geo + amount);
        HUDManager.Instance.UpdateBar(HUDManager.BarType.Health, geo / maxGeo);
    }
}