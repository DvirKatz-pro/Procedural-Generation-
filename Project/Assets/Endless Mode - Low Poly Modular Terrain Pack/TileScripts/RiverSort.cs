using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//script to sort river tiles - Dvir
public class RiverSort : MonoBehaviour
{
    private Tile[,] allTiles;
    private Tile[,] buffer;
    float[,] riverNoiseMap;
    private Vector2 allTilesIndex;
    private Vector2 allTilesCurrentIndex;
    List<Tile> riverTiles;

   
    private void Start()
    {

    }
    public void init(Vector2 rowCol, Tile[,] m_allTiles, Vector2 m_allTilesIndex, float[,] m_riverNoiseMap,float[,] m_biomeNoiseMap)
    {
        //add 4 as a padding so that we can figure out adjuacency of buffer tiles
        buffer = new Tile[(int)rowCol.x + 4, (int)rowCol.y + 4];
        allTiles = m_allTiles;
        allTilesCurrentIndex = m_allTilesIndex;
        //adjuast the alltiles index for this script so that we can figure out adjuacency rather than just the 3x3
        allTilesIndex.x = m_allTilesIndex.x - 1;
        allTilesIndex.y = m_allTilesIndex.y - 1;
        riverNoiseMap = m_riverNoiseMap;

        riverTiles = new List<Tile>();
    }
    /*
     * figure out which tiles are rivers and sort them accordingly
     */
    public void sort()
    {
        Vector2 allTilesCurrentIndex = allTilesIndex;
        //we start at 1 because we need to check adjuacncy which could be i = 0 and j = 0
        for (int i = 1; i < riverNoiseMap.GetUpperBound(0); i++)
        {
            for (int j = 1; j < riverNoiseMap.GetUpperBound(1); j++)
            {
                Tile tile = allTiles[(int)allTilesCurrentIndex.x, (int)allTilesCurrentIndex.y];

                //if the noise map indicates a river and the tile can spawn a river, figure out the adjuacncy and set this tile to river
                if (riverNoiseMap[i, j] <= 0.0085f && tile.shouldSpawnRiver() && tile.getTile() == null)
                {
                    tile.setAdjacencyArray(inBetweenAdjuacency(new Vector2(i, j), allTilesCurrentIndex));
                    riverTiles.Add(tile);
                    tile.setTerrainType(ObjectPicker.TerrainType.river);
                }


                
                buffer[i - 1, j - 1] = tile;

                allTilesCurrentIndex.y++;
            }
            allTilesCurrentIndex.y = allTilesIndex.y;
            allTilesCurrentIndex.x++;
        }
        isRiver();
        setTiles();
    }
    private Dictionary<TerrainGeneration.directions, Tile> inBetweenAdjuacency(Vector2 m_iJ, Vector2 m_currentAllTilesIndex)
    {


        TerrainGeneration.directions initialDirection = TerrainGeneration.directions.SouthWest;

        Dictionary<TerrainGeneration.directions, Tile> adjuacencyArray = new Dictionary<TerrainGeneration.directions, Tile>();


        //find if this tile has adjuacent river tiles 

        for (int i = (int)m_iJ.x - 1; i < (int)m_iJ.x + 2; i++)
        {
            for (int j = (int)m_iJ.y - 1; j < (int)m_iJ.y + 2; j++)
            {
                Tile tile = allTiles[(int)allTilesIndex.x + i - 1, (int)allTilesIndex.y + j - 1];
                
                if (riverNoiseMap[i, j] <= 0.0085f && tile.shouldSpawnRiver())
                {
                    adjuacencyArray[initialDirection] = tile;
                }
                else
                {
                    adjuacencyArray[initialDirection] = tile;
                }
                initialDirection++;

            }
        }
        return adjuacencyArray;
    }
    /*
     * check if a river tile is indeed a part of a river, if its not, remove it. If the tile already has a river gameobject set, remove it also because we dont need to check it
     */
    private void isRiver()
    {
        for(int i = riverTiles.Count - 1; i >= 0; i--)
        {
            Tile t = riverTiles[i];
            
            if (t.getTile() == null)
            {
                Dictionary<TerrainGeneration.directions, Tile> adjuacncyArray = t.getAdjacencyArray();
                if (adjuacncyArray != null)
                {
                    if (t.getPos().x >= allTilesCurrentIndex.x && t.getPos().y >= allTilesCurrentIndex.y && t.getPos().x < allTilesCurrentIndex.x + 3 && t.getPos().y < allTilesCurrentIndex.y + 3)
                    {
                        if (adjuacncyArray[TerrainGeneration.directions.South].getTerrainType() != ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.West].getTerrainType() != ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.North].getTerrainType() != ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.East].getTerrainType() != ObjectPicker.TerrainType.river)
                        {
                            riverTiles.RemoveAt(i);
                            t.setTerrainType(ObjectPicker.TerrainType.normal);
                            allTiles[(int)t.getPos().x, (int)t.getPos().y] = t;
                        }
                    }
                }
            }
            else
            {
                riverTiles.RemoveAt(i);
            }
        }
       
    }
    /*
     * set river tiles based on certain adjuacency rules
     */
    private void setTiles()
    {
        foreach (Tile t in riverTiles)
        { 
            if (t.getTile() == null)
            {
                Dictionary<TerrainGeneration.directions, Tile> adjuacncyArray = t.getAdjacencyArray();
                if (adjuacncyArray != null)
                {
                    if (t.getPos().x >= allTilesCurrentIndex.x && t.getPos().y >= allTilesCurrentIndex.y && t.getPos().x < allTilesCurrentIndex.x + 3 && t.getPos().y < allTilesCurrentIndex.y + 3)
                    {
                      
                        if (adjuacncyArray[TerrainGeneration.directions.West].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.East].getTerrainType() == ObjectPicker.TerrainType.river)
                        {
                            t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.streight));
                            t.setRotation(90);
                            t.setSpawnObjects(false);
                            t.setMaterial(t.getBiome().getTileMaterial());
                        }
                        else if (adjuacncyArray[TerrainGeneration.directions.South].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.North].getTerrainType() == ObjectPicker.TerrainType.river)
                        {
                            t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.streight));
                            t.setRotation(0);
                            t.setSpawnObjects(false);
                            t.setMaterial(t.getBiome().getTileMaterial());

                        }
                        else if (adjuacncyArray[TerrainGeneration.directions.South].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.West].getTerrainType() == ObjectPicker.TerrainType.river)
                        {
                            t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                            t.setRotation(90);
                            t.setSpawnObjects(false);
                            t.setMaterial(t.getBiome().getTileMaterial());

                        }
                        else if (adjuacncyArray[TerrainGeneration.directions.South].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.East].getTerrainType() == ObjectPicker.TerrainType.river)
                        {
                            t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                            t.setRotation(0);
                            t.setSpawnObjects(false);
                            t.setMaterial(t.getBiome().getTileMaterial());

                        }
                        else if (adjuacncyArray[TerrainGeneration.directions.North].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.East].getTerrainType() == ObjectPicker.TerrainType.river)
                        {
                            t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                            t.setRotation(180);
                            t.setSpawnObjects(false);
                            t.setMaterial(t.getBiome().getTileMaterial());
                        }
                        else if (adjuacncyArray[TerrainGeneration.directions.North].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.West].getTerrainType() == ObjectPicker.TerrainType.river)
                        {
                            t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                            t.setRotation(0);
                            t.setSpawnObjects(false);
                            t.setMaterial(t.getBiome().getTileMaterial());
                        }
                        else if (!checkArtificial(t))
                        {
                            if (adjuacncyArray[TerrainGeneration.directions.South].getTerrainType() != ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.West].getTerrainType() != ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.North].getTerrainType() != ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.East].getTerrainType() == ObjectPicker.TerrainType.river)
                            {
                                t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.end));
                                t.setRotation(-90);
                                t.setSpawnObjects(false);
                                t.setMaterial(t.getBiome().getTileMaterial());

                            }
                            else if (adjuacncyArray[TerrainGeneration.directions.South].getTerrainType() != ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.West].getTerrainType() != ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.North].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.East].getTerrainType() != ObjectPicker.TerrainType.river)
                            {
                                t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.end));
                                t.setRotation(180);
                                t.setSpawnObjects(false);
                                t.setMaterial(t.getBiome().getTileMaterial());
                            }
                            else if (adjuacncyArray[TerrainGeneration.directions.South].getTerrainType() != ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.West].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.North].getTerrainType() != ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.East].getTerrainType() != ObjectPicker.TerrainType.river)
                            {
                                t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.end));
                                t.setRotation(90);
                                t.setSpawnObjects(false);
                                t.setMaterial(t.getBiome().getTileMaterial());
                            }
                            else if (adjuacncyArray[TerrainGeneration.directions.South].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.West].getTerrainType() != ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.North].getTerrainType() != ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.East].getTerrainType() != ObjectPicker.TerrainType.river)
                            {
                                t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.end));
                                t.setRotation(0);
                                t.setSpawnObjects(false);
                                t.setMaterial(t.getBiome().getTileMaterial());
                            }
                            
                        }
                    }
                }
            }

                    
        }
          
    }
    /*
    * create artifical river tile if able based on certain adjuacency rules
    */
    private bool checkArtificial(Tile t)
    {
        Vector2 localAllTilesIndex = allTilesIndex;
        bool found = false;
        Tile neighbour = null;
        Dictionary<TerrainGeneration.directions, Tile> adjuacncyArray = t.getAdjacencyArray();
        if (adjuacncyArray[TerrainGeneration.directions.South].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.NorthEast].getTerrainType() == ObjectPicker.TerrainType.river)
        {
            Vector2 neighbourPos = t.getPos();
            neighbourPos.x += 1;
            if (allTiles[(int)neighbourPos.x, (int)neighbourPos.y].shouldSpawnRiver())
            {
                t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                t.setRotation(0);
                t.setSpawnObjects(false);
                t.setMaterial(t.getBiome().getTileMaterial());

                neighbour = allTiles[(int)neighbourPos.x, (int)neighbourPos.y];
                neighbour.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                neighbour.setRotation(180);
                neighbour.setSpawnObjects(false);
                neighbour.setMaterial(t.getBiome().getTileMaterial());
                neighbour.setTerrainType(ObjectPicker.TerrainType.river);
            }
        }
        else if (adjuacncyArray[TerrainGeneration.directions.South].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.NorthWest].getTerrainType() == ObjectPicker.TerrainType.river)
        {
            Vector2 neighbourPos = t.getPos();
            neighbourPos.x -= 1;
            if (allTiles[(int)neighbourPos.x, (int)neighbourPos.y].shouldSpawnRiver())
            {
                t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                t.setRotation(0);
                t.setSpawnObjects(false);
                t.setMaterial(t.getBiome().getTileMaterial());

                neighbour = allTiles[(int)neighbourPos.x, (int)neighbourPos.y];
                neighbour.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                neighbour.setRotation(180);
                neighbour.setSpawnObjects(false);
                neighbour.setMaterial(t.getBiome().getTileMaterial());
                neighbour.setTerrainType(ObjectPicker.TerrainType.river);
            }
        }
        else if (adjuacncyArray[TerrainGeneration.directions.North].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.SouthWest].getTerrainType() == ObjectPicker.TerrainType.river)
        {
            Vector2 neighbourPos = t.getPos();
            neighbourPos.x -= 1;
            if (allTiles[(int)neighbourPos.x, (int)neighbourPos.y].shouldSpawnRiver())
            {
                t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                t.setRotation(-90);
                t.setSpawnObjects(false);
                t.setMaterial(t.getBiome().getTileMaterial());

                neighbour = allTiles[(int)neighbourPos.x, (int)neighbourPos.y];
                neighbour.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                neighbour.setRotation(90);
                neighbour.setSpawnObjects(false);
                neighbour.setMaterial(t.getBiome().getTileMaterial());
                neighbour.setTerrainType(ObjectPicker.TerrainType.river);
            }
        }
        else if (adjuacncyArray[TerrainGeneration.directions.North].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.SouthEast].getTerrainType() == ObjectPicker.TerrainType.river)
        {
            Vector2 neighbourPos = t.getPos();
            neighbourPos.x += 1;
            if (allTiles[(int)neighbourPos.x, (int)neighbourPos.y].shouldSpawnRiver())
            {
                t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                t.setRotation(90);
                t.setSpawnObjects(false);
                t.setMaterial(t.getBiome().getTileMaterial());

                neighbour = allTiles[(int)neighbourPos.x, (int)neighbourPos.y];
                neighbour.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                neighbour.setRotation(-90);
                neighbour.setSpawnObjects(false);
                neighbour.setMaterial(t.getBiome().getTileMaterial());
                neighbour.setTerrainType(ObjectPicker.TerrainType.river);
            }
        }
        else if (adjuacncyArray[TerrainGeneration.directions.East].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.NorthWest].getTerrainType() == ObjectPicker.TerrainType.river)
        {
            Vector2 neighbourPos = t.getPos();
            neighbourPos.y += 1;
            if (allTiles[(int)neighbourPos.x, (int)neighbourPos.y].shouldSpawnRiver())
            {
                t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                t.setRotation(0);
                t.setSpawnObjects(false);
                t.setMaterial(t.getBiome().getTileMaterial());

                neighbour = allTiles[(int)neighbourPos.x, (int)neighbourPos.y];
                neighbour.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                neighbour.setRotation(0);
                neighbour.setSpawnObjects(false);
                neighbour.setMaterial(t.getBiome().getTileMaterial());
                neighbour.setTerrainType(ObjectPicker.TerrainType.river);
            }
           
        }
        else if (adjuacncyArray[TerrainGeneration.directions.East].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.SouthWest].getTerrainType() == ObjectPicker.TerrainType.river)
        {
            Vector2 neighbourPos = t.getPos();
            neighbourPos.y -= 1;
            if (allTiles[(int)neighbourPos.x, (int)neighbourPos.y].shouldSpawnRiver())
            {
                t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                 t.setRotation(0);
                t.setSpawnObjects(false);
                t.setMaterial(t.getBiome().getTileMaterial());


                neighbour = allTiles[(int)neighbourPos.x, (int)neighbourPos.y];
                neighbour.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                neighbour.setRotation(180);
                neighbour.setSpawnObjects(false);
                neighbour.setMaterial(t.getBiome().getTileMaterial());
                neighbour.setTerrainType(ObjectPicker.TerrainType.river);
            }

        }
        else if (adjuacncyArray[TerrainGeneration.directions.West].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.SouthEast].getTerrainType() == ObjectPicker.TerrainType.river)
        {
            Vector2 neighbourPos = t.getPos();
            neighbourPos.y -= 1;
            if (allTiles[(int)neighbourPos.x, (int)neighbourPos.y].shouldSpawnRiver())
            {
                t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                t.setRotation(90);
                t.setSpawnObjects(false);
                t.setMaterial(t.getBiome().getTileMaterial());


                neighbour = allTiles[(int)neighbourPos.x, (int)neighbourPos.y];
                neighbour.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                neighbour.setRotation(-90);
                neighbour.setSpawnObjects(false);
                neighbour.setMaterial(t.getBiome().getTileMaterial());
                neighbour.setTerrainType(ObjectPicker.TerrainType.river);
            }
        }
        else if (adjuacncyArray[TerrainGeneration.directions.West].getTerrainType() == ObjectPicker.TerrainType.river && adjuacncyArray[TerrainGeneration.directions.NorthEast].getTerrainType() == ObjectPicker.TerrainType.river)
        {
            Vector2 neighbourPos = t.getPos();
            neighbourPos.y += 1;
            if (allTiles[(int)neighbourPos.x, (int)neighbourPos.y].shouldSpawnRiver())
            {
                t.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                t.setRotation(180);
                t.setSpawnObjects(false);
                t.setMaterial(t.getBiome().getTileMaterial());

                neighbour = allTiles[(int)neighbourPos.x, (int)neighbourPos.y];
                neighbour.setTile(t.getBiome().riverPicker(ObjectPicker.TileType.turn));
                neighbour.setRotation(0);
                neighbour.setSpawnObjects(false);
                neighbour.setMaterial(t.getBiome().getTileMaterial());
                neighbour.setTerrainType(ObjectPicker.TerrainType.river);
            }
        }
        if (neighbour != null)
        {
            found = true;
        }
        return found;
    }
}
