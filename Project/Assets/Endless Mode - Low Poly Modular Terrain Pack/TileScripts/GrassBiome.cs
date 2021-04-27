using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
//GrassBiome overriding Biome - Dvir
public class GrassBiome : Biome
{
    //Lists of "regular" tile Gameobjects fitting with this biome 
    public List<GameObject> grassTerrain;
    public List<GameObject> grassSoftHills;
    public List<GameObject> grassTrees;

    //"inBetween" tile Gameobjects
    public GameObject adjuacentTile;
    public GameObject adjuacentCorner;
    public GameObject adjuacentDiagonal;


    //Lists of "river" tile Gameobjects
    public List<GameObject> riverStrighet;
    public GameObject riverTurn;
    public GameObject riverStart;

    //GrassBiome materials
    public Material grassMaterial;
    public Material inBetweenMaterialRock;
    public Material inBetweenMaterialSnow;
    public Material defaultMaterial;
    /*
     * based on a given value return a tile depending on its value
     */
    public override GameObject tilePicker(float num)
    {
        //if > 0.7 return a hill
        if (num > 0.7f)
        {
            
            int type = (int)(num * 1000);
            type = type % grassSoftHills.Count;
            if (type < 0)
            {
                type *= -1;
            }
            return grassSoftHills[type];

        }
        //otherwise return a flat terrain tile
        else
        {

            int type = (int)(num * 100);
            type = type % grassTerrain.Count;
            if (type < 0)
            {
                type *= -1;
            }
            return grassTerrain[type];

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
            type = type % grassTrees.Count;
            
            return new KeyValuePair<GameObject, int>(grassTrees[type], 3);
        }

        return new KeyValuePair<GameObject, int>(null, 0);
    }
    //return the inbetween material corresponding to the biome type
    public override Material getInBetweenTileMaterial(ObjectPicker.BiomeType biomeType)
    {
        if (biomeType == ObjectPicker.BiomeType.rockBiome)
        {
            return inBetweenMaterialRock;
        }
        else
        {
            return inBetweenMaterialSnow;
        }
    }
    //return this biomes material
    public override Material getTileMaterial()
    {
        return grassMaterial;
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
    //return the default tile
    public override GameObject getDefaultTile()
    {
        return grassTerrain[0];
    }
    //return default material
    public override Material getDefaultMaterial()
    {
        return defaultMaterial;
    }


}



