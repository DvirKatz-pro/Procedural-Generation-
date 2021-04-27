using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script to generate generic tiles - Dvir
public class TileSort : MonoBehaviour
{
    private Tile[,] allTiles;
    private Tile[,] buffer;
    private float[,] tileNoiseMap;
    private Vector2 allTilesIndex;

    private void Start()
    {
       
    }
    public void init(Vector2 rowCol, Tile[,] m_allTiles, Vector2 m_allTilesIndex,float[,] m_tileNoiseMap)
    {
        buffer = new Tile[(int)rowCol.x, (int)rowCol.y];
        allTiles = m_allTiles;
        allTilesIndex = m_allTilesIndex;
        tileNoiseMap = m_tileNoiseMap;

    }

    /*
     * if after we sort inbetween tiles and river tiles and there are still tiles in the buffer without a gameobject, assign it a generic tile
     */
    public void sort()
    {
        Vector2 allTilesCurrentIndex = allTilesIndex;

        for (int i = 0; i < buffer.GetUpperBound(0) + 1; i++)
        {
            for (int j = 0; j < buffer.GetUpperBound(1) + 1; j++)
            {
               
                Tile tile = allTiles[(int)allTilesCurrentIndex.x, (int)allTilesCurrentIndex.y];
                if (tile.getTile() == null)
                {                  
                    tile.setTile(tile.getBiome().tilePicker(tileNoiseMap[i, j]));
                    tile.setMaterial(tile.getBiome().getTileMaterial());
                    tile.setTerrainType(ObjectPicker.TerrainType.normal);
                }
                buffer[i, j] = tile;
                allTilesCurrentIndex.y++;
            }
            allTilesCurrentIndex.y = allTilesIndex.y;
            allTilesCurrentIndex.x++;
        }

        copyToAllTiles();
    }


    private void copyToAllTiles()
    {
        Vector2 allTilesCurrentIndex = allTilesIndex;
        for (int i = 0; i < buffer.GetUpperBound(0) + 1; i++)
        {
            for (int j = 0; j < buffer.GetUpperBound(1); j++)
            {           
                allTiles[(int)allTilesCurrentIndex.x, (int)allTilesCurrentIndex.y] = buffer[i, j];
                allTilesCurrentIndex.y++;
            }
            allTilesCurrentIndex.y = allTilesIndex.y;
            allTilesCurrentIndex.x++;
        }

    }
    public Tile[,] getAllTiles()
    {
        return allTiles;
    }
}
