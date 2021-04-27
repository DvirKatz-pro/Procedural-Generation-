using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//SnowBiome overriding Biome - Dvir
public class SnowBiome : Biome
{
    //Lists of "regular" tile Gameobjects fitting with this biome 
    public List<GameObject> snowTerrain;
    public List<GameObject> snowObjects;
    public List<GameObject> snowMountains;

    //RockBiome materials
    public Material snowMaterial;
    public Material inBetweenMaterial;
    public Material defaultMaterial;

    //"inBetween" tile Gameobjects
    public GameObject adjuacentTile;
    public GameObject adjuacentCorner;
    public GameObject adjuacentDiagonal;

    //Lists of "river" tile Gameobjects
    public List<GameObject> riverStrighet;
    public GameObject riverTurn;
    public GameObject riverStart;

    
    public override GameObject tilePicker(float num)
    {
        //if > 0.4 return mountain
        if (num > 0.4f)
        {

            int type = (int)(num * 1000);
            type = type % snowMountains.Count;
            if (type < 0)
            {
                type *= -1;
            }
            GameObject g = snowMountains[type];
            return g;

        }
      
        //otherwise return a flat terrain tile
        else
        {

            int type = (int)(num * 100);
            type = type % snowTerrain.Count;
            if (type < 0)
            {
                type *= -1;
            }
            GameObject g = snowTerrain[type];
            return g;

        }
    }
    //return a river tile based on enum name
    public override GameObject riverPicker(ObjectPicker.TileType tileName)
    {
        switch (tileName)
        {
            case ObjectPicker.TileType.end:
                {
                    GameObject g = riverStart;
                    g.transform.rotation = Quaternion.Euler(0, 180, 0);
                    return g;

                }
            case ObjectPicker.TileType.turn:
                {
                    GameObject g = riverTurn;
                    return g;
                }
            case ObjectPicker.TileType.streight:
                {
                    GameObject g = riverStrighet[0];
                    return g;
                }
        }

        return null;
    }
    //return an inbetween tile based on a given string
    public override GameObject adjuacentPicker(ObjectPicker.TileType tileName)
    {
        switch (tileName)
        {
            case ObjectPicker.TileType.streight:
            {
                    
                return adjuacentTile;
            }
            case ObjectPicker.TileType.corner:
            {
                return adjuacentCorner;
            }
        }
        return null;
        
    }
    //return an object(tree/rocks) and the distance that other objects must spawn away from
    public override KeyValuePair<GameObject, int> objectPicker(float num)
    {

        if (num > 0.7f)
        {
            int type = (int)(num * 100);
            type = type % snowObjects.Count;

            return new KeyValuePair<GameObject, int>(snowObjects[type], 3);
        }

        return new KeyValuePair<GameObject, int>(null, 0);
    }
    //return the inbetween material corresponding to the biome type
    public override Material getInBetweenTileMaterial(ObjectPicker.BiomeType biomeType)
    {
        return inBetweenMaterial;
    }
    //return this biomes material
    public override Material getTileMaterial()
    {
        return snowMaterial;
    }
    //return the default tile
    public override GameObject getDefaultTile()
    {
        return snowTerrain[0];
    }
    //return default material
    public override Material getDefaultMaterial()
    {
        return defaultMaterial;
    }
}
