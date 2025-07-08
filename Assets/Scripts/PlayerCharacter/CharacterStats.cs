using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 15f;
    public float staminaRegenRate = 10f;
    public float staminaRegenDelay = 1.5f;

    private float regenTimer = 0f;

    public bool CanSprint => currentStamina > 0f;

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    public void DrainStamina(float deltaTime)
    {
        currentStamina -= staminaDrainRate * deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        regenTimer = 0f;
    }

    public void RegenerateStamina(float deltaTime)
    {
        regenTimer += deltaTime;
        if (regenTimer >= staminaRegenDelay)
        {
            currentStamina += staminaRegenRate * deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died.");
        // Respawn Logic
    }
}
