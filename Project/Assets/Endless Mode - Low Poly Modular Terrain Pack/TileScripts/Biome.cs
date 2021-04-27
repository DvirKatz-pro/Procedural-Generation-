using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
/*
 * Biome class thats meant to be overridden for reuseability - Dvir
 */
public class Biome : MonoBehaviour
{
    [SerializeField] private GameObject waterTile;
    public virtual GameObject tilePicker(float num)
    {
        return null;
    }
    public virtual KeyValuePair<GameObject, int> objectPicker(float num)
    {
        return new KeyValuePair<GameObject, int>(null, 0);
    }

    public virtual GameObject riverPicker(ObjectPicker.TileType tileName)
    {
        return null;
    }
    public virtual GameObject waterPicker()
    {
        return waterTile;
    }
    public virtual GameObject adjuacentPicker(ObjectPicker.TileType tileName)
    {
        return null;
    }
    public virtual Material getTileMaterial()
    {
        return null;
    }
    public virtual Material getInBetweenTileMaterial(ObjectPicker.BiomeType biomeType)
    {
        return null;
    }

    public virtual GameObject  getDefaultTile()
    {
        return null;
    }
    public virtual Material getDefaultMaterial()
    {
        return null;
    }
   



}
