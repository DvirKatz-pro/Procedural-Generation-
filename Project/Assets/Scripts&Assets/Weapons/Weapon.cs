using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Weapon
// Contains all of the properties of a weapon
// 
// Writen by: Cal
public class Weapon : MonoBehaviour
{
    #region Variables

    // Weapon properies
    [SerializeField] private string weaponName;
    [SerializeField] private Color weaponColor;
    [SerializeField] private Texture weaponImage;
    private int weaponDamage;
    [SerializeField] private int weaponMaxDamage;
    private int weaponDurability;
    private int weaponMaxDurability = 100;
    private InventoryUIManager inventoryUIManager;

    #endregion

    #region Main

    // Set default values
    public void InstantiateWeapon(InventoryUIManager iuim)
    {
        weaponDamage = weaponMaxDamage;
        weaponDurability = weaponMaxDurability;
        inventoryUIManager = iuim;
    }

    // Set values with given durability
    public void InstantiateWeapon(InventoryUIManager iuim, int durability)
    {
        weaponDurability = durability;
        weaponDamage = weaponMaxDamage * weaponDurability / weaponMaxDurability;
        inventoryUIManager = iuim;
    }

    #endregion

    #region Get Properties

    // Get the weapon name
    public string GetWeaponName()
    {
        return weaponName;
    }

    // Get the weapon color
    public Color GetWeaponColor()
    {
        return weaponColor;
    }

    // Get the weapon image
    public Texture GetWeaponImage()
    {
        return weaponImage;
    }

    // Get the weapon damage
    public int GetWeaponDamage()
    {
        return weaponDamage;
    }

    // Get the weapon durability
    public int GetWeaponDurability()
    {
        return weaponDurability;
    }

    #endregion

    #region Weapon Functions

    // Lower the durability of the weapon
    public void AffectDurability(int amount)
    {
        // Lower the durability until 0
        weaponDurability = (weaponDurability - amount < 0) ? 0 : weaponDurability - amount;
        weaponDamage = weaponMaxDamage - 20 + (20 * weaponDurability / weaponMaxDurability);
        inventoryUIManager.UpdateCurrentWeaponUI();
    }

    #endregion
}