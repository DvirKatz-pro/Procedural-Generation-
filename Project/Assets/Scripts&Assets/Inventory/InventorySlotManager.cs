using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// InventorySlotManager
// It sets and manages the properties of the inventory slot
//
// Written by: Cal
public class InventorySlotManager : MonoBehaviour
{
    #region Variables

    // Properties
    [SerializeField] private GameObject inventorySlotContents;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private RawImage weaponImage;
    [SerializeField] private TextMeshProUGUI weaponDamage;
    [SerializeField] private Image weaponDurability;
    [SerializeField] private Outline weaponColor;
    [SerializeField] private GameObject weaponEquip;
    [SerializeField] private GameObject weaponDrop;

    // Colors
    Color green = new Color(0.14f, 1f, 0.26f, 0.5f);
    Color yellow = new Color(0.95f, 1f, 0.14f, 0.5f);
    Color orange = new Color(1f, 0.68f, 0.14f, 0.5f);
    Color red = new Color(1f, 0.14f, 0.14f, 0.5f);

    #endregion

    #region Main

    public void ShowSlot(bool b)
    {
        inventorySlotContents.SetActive(b);
    }

    #endregion

    #region All Properties Setters

    // Sets all the properties of the weapon
    public void SetAllProperties(string name, Texture image, int damage, int durability, Color color)
    {
        SetName(name);
        SetImage(image);
        SetDamage(damage);
        SetDurability(durability);
        SetColor(color);
    }

    #endregion

    #region Individual Property Setters

    // Sets the name
    public void SetName(string name)
    {
        weaponName.text = name;
    }

    // Sets the image
    public void SetImage(Texture image)
    {
        weaponImage.texture = image;
    }

    // Sets the damage
    public void SetDamage(int damage)
    {
        weaponDamage.text = damage.ToString();
    }

    // Sets the durability
    public void SetDurability(int d)
    {
        // Divide by 100 since we want our value 
        float durability = (float)d / 100f;

        // Set the fill amount
        weaponDurability.fillAmount = durability;

        // Set the fill color
        if (durability >= 0.75)
            weaponDurability.color = green;
        else if (durability >= 0.5)
            weaponDurability.color = yellow;
        else if (durability >= 0.25)
            weaponDurability.color = orange;
        else
            weaponDurability.color = red;
    }

    // Sets the color
    public void SetColor(Color color)
    {
        weaponColor.effectColor = color;
    }

    #endregion

    #region Equippable and Droppable

    // Sets if the weapon is equippable
    public void SetEquippable(bool equippable)
    {
        if (equippable) { weaponEquip.SetActive(true); } else { weaponEquip.SetActive(false); }
    }

    // Sets if the weapon is droppable
    public void SetDroppable(bool droppable)
    {
        if (droppable) { weaponDrop.SetActive(true); } else { weaponDrop.SetActive(false); }
    }

    #endregion
}
