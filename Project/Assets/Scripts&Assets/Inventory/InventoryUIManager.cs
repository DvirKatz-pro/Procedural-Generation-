using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// InventoryUIManager
// Manages the inventory UI
//
// Written by: Cal
public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private InventorySlotManager[] inventorySlots = new InventorySlotManager[4];
    [SerializeField] private InventorySlotManager inventoryCurrentWeaponSlot;

    private Inventory inventory;

    private void Awake()
    {
        inventory = GameObject.Find("GameManager").GetComponent<GameManager>().GetPlayer().GetComponent<Inventory>();
        if (inventory == null)
            Debug.LogError("Inventory on " + this.name + " is null.");
    }

    // Returns the inventory slot at given slot number (1-4)
    public InventorySlotManager GetInventorySlot(int slot)
    {
        return inventorySlots[slot - 1];
    }

    // Returns the current inventory slot
    public InventorySlotManager GetCurrentInventorySlot()
    {
        return inventoryCurrentWeaponSlot;
    }

    public void RenderInventoryUI()
    {
        Awake();
        Weapon currentWeapon = inventory.GetCurrentWeapon();
        inventoryCurrentWeaponSlot.SetAllProperties(
            currentWeapon.GetWeaponName(),
            currentWeapon.GetWeaponImage(),
            currentWeapon.GetWeaponDamage(),
            currentWeapon.GetWeaponDurability(),
            currentWeapon.GetWeaponColor()
        );
        inventoryCurrentWeaponSlot.ShowSlot(true);

        List<Weapon> weapons = inventory.GetWeapons();
        int weaponsLength = weapons.Count;
        for (int i = 0; i < weaponsLength; i++)
        {
            Weapon weaponToRender = weapons[i];
            inventorySlots[i].SetAllProperties(
                weaponToRender.GetWeaponName(),
                weaponToRender.GetWeaponImage(),
                weaponToRender.GetWeaponDamage(),
                weaponToRender.GetWeaponDurability(),
                weaponToRender.GetWeaponColor()
            );
            inventorySlots[i].ShowSlot(true);
        }
        for (int i = weaponsLength; i < 4; i++)
        {
            inventorySlots[i].ShowSlot(false);
        }
    }

    public void UpdateCurrentWeaponUI()
    {
        inventoryCurrentWeaponSlot.SetDamage(inventory.GetCurrentWeapon().GetWeaponDamage());
        inventoryCurrentWeaponSlot.SetDurability(inventory.GetCurrentWeapon().GetWeaponDurability());
    }
}
