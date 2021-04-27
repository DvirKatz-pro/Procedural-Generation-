using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PickupManager
// Manages the pickup and its properties
//
// Written by: Cal
public class PickupManager : MonoBehaviour
{
    public enum pickupType { Weapon, Scrap };
    public bool pickedUp;
    public string pickupName;
    public pickupType pickup;
    public int durability;

    void Start()
    {
        pickedUp = false;
    }
}
