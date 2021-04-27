using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//RockBiome overriding Biome - Dvir
public class RockBiome : Biome
{
    //Lists of "regular" tile Gameobjects fitting with this biome 
    public List<GameObject> rockTerrain;
    public List<GameObject> rockHills;
    public List<GameObject> rockBoulders;
    public List<GameObject> rockMountains;

    //"inBetween" tile Gameobjects
    public GameObject adjuacentTile;
    public GameObject adjuacentCorner;
    public GameObject adjuacentDiagonal;

    //Lists of "river" tile Gameobjects
    public List<GameObject> riverStrighet;
    public GameObject riverTurn;
    public GameObject riverStart;

    //RockBiome materials
    public Material rockMaterial;
    public Material inBetweenMaterial;
    public Material defaultMaterial;

    /*
     * based on a given value return a tile depending on its value
     */
    public override GameObject tilePicker(float num)
    {
        //if > 0.4 return a hill
        if (num > 0.4f)
        {

            int type = (int)(num * 1000);
            type = type % rockHills.Count;
            if (type < 0)
            {
                type *= -1;
            }
            GameObject g = rockHills[type];
            return g;

        }
        //if > 0.6 return mountain
        else if(num > 0.6f)
        {
            int type = (int)(num * 1000);
            type = type % rockMountains.Count;
            if (type < 0)
            {
                type *= -1;
            }
            GameObject g = rockMountains[type];
            return g;
        }
        //otherwise return a flat terrain tile
        else
        {

            int type = (int)(num * 100);
            type = type % rockTerrain.Count;
            GameObject g = rockTerrain[type];
            return g;

        }
    }
    //return a river tile based on a given string
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
    //return an object(tree/rocks) and the distance that other objects must spawn away from
    public override KeyValuePair<GameObject, int> objectPicker(float num)
    {

        if (num > 0.7f)
        {
            int type = (int)(num * 100);
            type = type % rockBoulders.Count;

            return new KeyValuePair<GameObject, int>(rockBoulders[type], 5);
        }

        return new KeyValuePair<GameObject, int>(null, 0);
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
    //return the inbetween material corresponding to the biome type
    public override Material getInBetweenTileMaterial(ObjectPicker.BiomeType biomeType)
    {
        return inBetweenMaterial;
    }
    //return this biomes material
    public override Material getTileMaterial()
    {
        return rockMaterial;
    }
    //return the default tile
    public override GameObject getDefaultTile()
    {
        return rockTerrain[0];
    }
    //return default material
    public override Material getDefaultMaterial()
    {
        return defaultMaterial;
    }
}
