using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Weapon Manager
// The weapon manger maps the weapon objects to scripts
//
// Written by: Cal
public class WeaponManager : MonoBehaviour
{
    #region Variables

    public GameObject[] weaponGameObjects = new GameObject[4];
    public Weapon[] weaponScripts = new Weapon[4];

    #endregion

    #region Get Weapon Gameobject

    // Get a specific weapon gameobject
    public GameObject getWeaponGameObject(string weaponName)
    {
        foreach (GameObject weapon in weaponGameObjects)
        {
            if (Equals(weapon.name, weaponName))
            {
                return weapon;
            }
        }
        Debug.Log("Weapon gameobject '" + weaponName + "' could not be found in weapon manager.");
        return null;
    }

    // Get random weapon gameobject
    public GameObject getRandomWeaponGameObject()
    {
        int rand = Random.Range(0, weaponGameObjects.Length);
        return weaponGameObjects[rand];
    }

    #endregion

    #region Get Weapon Script

    // Get a specific weapon script
    public Weapon getWeaponScript(string weaponName)
    {
        foreach (Weapon weapon in weaponScripts)
        {
            if (Equals(weapon.GetWeaponName(), weaponName))
            {
                return weapon;
            }
        }
        Debug.Log("Weapon script '" + weaponName + "' could not be found in weapon manager.");
        return null;
    }

    #endregion
}
