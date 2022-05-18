using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    // Start is called before the first frame update
    HealthBar healthBar;
    PlayerManager playerManager;

    StaminaBar staminaBar;
    public float staminaRegenerationAmount = 5;
    public float staminaRegenTimer = 0;

    AnimatorHandler animatorHandler;
    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        healthBar = FindObjectOfType<HealthBar>();
        staminaBar = FindObjectOfType<StaminaBar>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }
    void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetCurrentHealth(currentHealth);


        maxStamina = SetMaxStaminaFromStaminaLevel();
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
        staminaBar.SetCurrentStamina(currentStamina);
    }

    private int SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    private float SetMaxStaminaFromStaminaLevel()
    {
        maxStamina = staminaLevel * 10;
        return maxStamina;
    }


    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth = currentHealth - damage;
        Debug.Log(currentHealth);
        healthBar.SetCurrentHealth(currentHealth);

        animatorHandler.PlayTargetAnimation("BodyHit", true);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animatorHandler.PlayTargetAnimation("Dead_01", true);
            isDead = true;
        }

    }

    public void TakeStaminaDamage(int damage)
    {
        currentStamina = currentStamina - damage;
        staminaBar.SetCurrentStamina(currentStamina);
    }

    public void RegenerateStamina()
    {
        if (playerManager.isInteracting)
        {
            staminaRegenTimer = 0;
        }
        else
        {
            staminaRegenTimer += Time.deltaTime;

            if (currentStamina < maxStamina && staminaRegenTimer > 1f)
            {
                currentStamina += staminaRegenerationAmount * Time.deltaTime;

                staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
            }
        }
    }
}
