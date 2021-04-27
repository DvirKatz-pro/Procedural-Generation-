using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// InventoryButton
// Manages the buttons in inventory
//
// Written by: Cal
public class InventoryButton : MonoBehaviour
{
    public int weaponSlot;
    private Inventory inventory;

    private void Awake()
    {
        inventory = GameObject.Find("GameManager").GetComponent<GameManager>().GetPlayer().GetComponent<Inventory>();
        if (inventory == null)
            Debug.Log("Inventory on " + this.name + " is null.");
    }

    public void Clicked()
    {
        if (this.name == "Equip")
        {
            inventory.SwapWeapons(weaponSlot);
        }
    }

}
