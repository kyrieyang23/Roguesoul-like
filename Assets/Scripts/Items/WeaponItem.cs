using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject modelPrefab;
    public bool isUnarmed;

    [Header("One Handed Attack Animations")]
    public string OH_Light_Attack_01;
    public string OH_Light_Attack_02;
    public string OH_Light_Attack_03;
    public string OH_Heavy_Attack_01;
    public string OH_Heavy_Attack_02;
    public string OH_Heavy_Attack_03;

    [Header("Stamina Costs")]
    public int baseStamina;
    public float lightAttackMultiplier;
    public float heavyAttackMultiplier;

    [Header("Weapon Type")]
    public bool isSpellCaster;
    public bool isFaithCaster;
    public bool isPyroCaster;
    public bool isMeleeWeapon;
}
