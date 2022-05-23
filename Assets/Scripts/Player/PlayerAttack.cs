using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerAttack : MonoBehaviour
{
    AnimatorHandler animationHandler;
    PlayerManager playerManager;
    PlayerInventory playerInventory;
    InputManager inputManager;
    WeaponSlotManager weaponSlotManager;
    PlayerStats playerStats;


    public string lastAttack;

    private void Awake()
    {
        animationHandler = GetComponentInChildren<AnimatorHandler>();
        playerManager = GetComponentInParent<PlayerManager>();
        playerInventory = GetComponentInParent<PlayerInventory>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        inputManager = GetComponentInParent<InputManager>();
        playerStats = GetComponentInParent<PlayerStats>();
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if (playerStats.currentStamina <= 0)
        {
            return;
        }

        if (inputManager.comboFlag)
        {
            animationHandler.anim.SetBool("canDoCombo", false);

            if ((lastAttack == weapon.OH_Light_Attack_01) && (inputManager.lattackInput))
            {
                weaponSlotManager.attackingWeapon = weapon;
                weaponSlotManager.DrainStaminaLightAttack();
                animationHandler.PlayTargetAnimation(weapon.OH_Light_Attack_02, true);
                lastAttack = weapon.OH_Light_Attack_02;
                // Debug.Log("Light Attack 2");
            }
            else if ((lastAttack == weapon.OH_Light_Attack_02) && (inputManager.lattackInput))
            {
                weaponSlotManager.attackingWeapon = weapon;
                weaponSlotManager.DrainStaminaLightAttack();
                animationHandler.PlayTargetAnimation(weapon.OH_Light_Attack_03, true);
            }
            else if ((lastAttack == weapon.OH_Heavy_Attack_01) && (inputManager.hattackInput))
            {
                weaponSlotManager.attackingWeapon = weapon;
                weaponSlotManager.DrainStaminaHeavyAttack();
                animationHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_02, true);
                lastAttack = weapon.OH_Heavy_Attack_02;
                // Debug.Log("Heavy Attack 2");
            }
            else if ((lastAttack == weapon.OH_Heavy_Attack_02) && (inputManager.hattackInput))
            {
                weaponSlotManager.attackingWeapon = weapon;
                weaponSlotManager.DrainStaminaHeavyAttack();
                animationHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_03, true);
            }
        }

    }
    public void HandleLightAttack(WeaponItem weapon)
    {
        if (playerStats.currentStamina <= 0)
        {
            return;
        }
        weaponSlotManager.attackingWeapon = weapon;
        weaponSlotManager.DrainStaminaLightAttack();
        animationHandler.PlayTargetAnimation(weapon.OH_Light_Attack_01, true);
        lastAttack = weapon.OH_Light_Attack_01;
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        if (playerStats.currentStamina <= 0)
        {
            return;
        }
        weaponSlotManager.attackingWeapon = weapon;
        weaponSlotManager.DrainStaminaHeavyAttack();
        animationHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_01, true);
        lastAttack = weapon.OH_Heavy_Attack_01;
    }
    #region Input Action
    public void HandleRBAction()
    {
        if (playerInventory.rightWeapon.isMeleeWeapon)
        {
            PerformRBMeleeAction();
        }
        else if (playerInventory.rightWeapon.isSpellCaster || playerInventory.rightWeapon.isFaithCaster || playerInventory.rightWeapon.isPyroCaster)
        {
            PerformRBMagicAction(playerInventory.rightWeapon);
        }

    }
    #endregion
    #region attack Actions
    private void PerformRBMeleeAction()
    {
        if (playerManager.canDoCombo)
        {
            inputManager.comboFlag = true;
            HandleWeaponCombo(playerInventory.rightWeapon);
            inputManager.comboFlag = false;
        }
        else
        {
            if (playerManager.isInteracting)
                return;
            if (playerManager.canDoCombo)
                return;

            animationHandler.anim.SetBool("isUsingRightHand", true);
            HandleLightAttack(playerInventory.rightWeapon);
        }
    }
    #endregion

    private void PerformRBMagicAction(WeaponItem weapon)
    {
        if (weapon.isFaithCaster)
        {
            if (playerInventory.currentSpell != null && playerInventory.currentSpell.isFaithSpell)
            {
                //Attempt to Cast spell
            }
        }
    }
}
