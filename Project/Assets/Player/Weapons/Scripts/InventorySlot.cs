using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    private GameObject slot = null;
    public void setSlot(GameObject weapon)
    {
        slot = weapon;
    }
    public GameObject getSlot()
    {
        return slot;
    }
}
