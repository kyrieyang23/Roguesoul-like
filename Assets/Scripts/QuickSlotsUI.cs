using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotsUI : MonoBehaviour
{
    public Image leftWeaponIcon;
    public Image rightWeaponIcon;
    public void UpdateWeaponQuickSlotsUI(bool isLeft, WeaponItem weapon)
    {
        if(isLeft == false)
        {
            if(weapon.itemIcon != null)
            {
                rightWeaponIcon.enabled = true;
                rightWeaponIcon.sprite = weapon.itemIcon;
            }
            else
            {
                rightWeaponIcon.enabled = false;
                rightWeaponIcon.sprite = null;
            }
        }
        else
        {
            if(weapon.itemIcon != null)
            {
                leftWeaponIcon.enabled = true;
                leftWeaponIcon.sprite = weapon.itemIcon;
            }
            else
            {
                leftWeaponIcon.enabled = false;
                leftWeaponIcon.sprite = null;
            }
        }
    }
}
