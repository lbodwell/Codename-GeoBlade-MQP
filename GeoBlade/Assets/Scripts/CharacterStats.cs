using UnityEngine;

public abstract class CharacterStats : MonoBehaviour {
    public float health;
    public float maxHealth;

    public abstract void DamageCharacter(float amount);

    public abstract void HealCharacter(float amount);
}