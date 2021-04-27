using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStats : MonoBehaviour
{
    public int damage;
    private int maxDamage;
    public int durability;
    private int currentDurability;
    private int maxDurability;
    void Start()
    {
        currentDurability = durability;
        maxDurability = (int)(0.2f * durability);
        maxDamage = damage;
    }

    public void lowerDurability(int amount)
    {
        currentDurability -= amount;
        if (currentDurability <= maxDurability)
        {
            currentDurability = maxDurability;
        }
        float unroundedDamage = (float)maxDamage * (float)(currentDurability / (float)durability);
        damage = Mathf.RoundToInt(unroundedDamage);
        Debug.Log(unroundedDamage);
    }
    public int getDurability()
    {

        return Mathf.RoundToInt(100 * (float)(currentDurability / (float)durability));
    }
}
