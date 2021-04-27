using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [NO LONGER IN USE]
// TotalWeapon
// Combined weapon model with weapon script as well as some common proprties
//
// Written by: Cal
public class TotalWeapon : MonoBehaviour
{
    public string weaponName;
    public GameObject weapon;
    public GameObject weaponGameObject;

    public string getWeaponName()
    {
        return weaponName;
    }

    public GameObject getWeapon()
    {
        return weapon;
    }

    public GameObject getWeaponGameObject()
    {
        return weaponGameObject;
    }
}
