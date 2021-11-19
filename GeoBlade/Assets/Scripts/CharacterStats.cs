using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour {
    public int health;
    public int maxHealth;

    public void DamageCharacter(int amount) {
        health = Math.Max(0, health - amount);
    }

    public void HealCharacter(int amount) {
        health = Math.Min(maxHealth, health + amount);
    }
}