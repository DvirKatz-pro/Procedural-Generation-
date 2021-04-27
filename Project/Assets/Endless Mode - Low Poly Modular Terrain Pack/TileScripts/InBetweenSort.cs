using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script to sort any "Inbetween" tiles - Dvir
public class InBetweenSort : MonoBehaviour
{
    //refernces to all tiles array, and the buffer
    private Tile[,] allTiles;
    private Tile[,] buffer;
    float[,] biomeNoiseMap;
    private Vector2 allTilesIndex;
    private Vector2 allTilesCurrentIndex;
    private List<Tile> inBetweenList;

    //refernces to script
    private ObjectPicker objectPicker;
    private void Start()
    {
        
    }
    /*
     * Initialize the script
     * rowCol - how big is the buffer
     * m_allTiles - a 2D of all tiles
     * m_allTilesIndex - the currentIndex of allTiles
     * m_biomeNoiseMap - a noise map describing the values of biomes
     */
    public void init(Vector2 rowCol, Tile[,] m_allTiles, Vector2 m_allTilesIndex, float[,] m_biomeNoiseMap)
    {
        //add 8 as a padding of 4*2 so that we can figure out adjuacency of buffer tiles, we use 8 instead of 4 because river sort uses 4 and inbetween has more priority than rivers
        buffer = new Tile[(int)rowCol.x + 8, (int)rowCol.y + 8];
        allTiles = m_allTiles;
        allTilesCurrentIndex = m_allTilesIndex;
        //adjuast the alltiles index for this script so that we can figure out adjuacency rather than just the 3x3
        allTilesIndex.x = m_allTilesIndex.x - 2;
        allTilesIndex.y = m_allTilesIndex.y - 2;
        biomeNoiseMap = m_biomeNoiseMap;

        objectPicker = GetComponent<ObjectPicker>();

        inBetweenList = new List<Tile>();
    }
    /*
     * figure out which tiles are inbetween and sort them accordingly
     */
    public void sort()
    {
        Vector2 allTilesCurrentIndex = allTilesIndex;
        //we start at 1 because we need to check the adjuacncy at 0
        for (int i = 1; i < biomeNoiseMap.GetUpperBound(0); i++)
        {
            for (int j = 1; j < biomeNoiseMap.GetUpperBound(1); j++)
            {
                Tile currentTile = allTiles[(int)allTilesCurrentIndex.x, (int)allTilesCurrentIndex.y];
                
                //if tile is null/new figure out its biome and which tiles are adjuacent, then add it to the buffer and the all tiles array
                if (currentTile == null || currentTile.getTile() == null)
                {
                    currentTile = new Tile();
                    (Biome, ObjectPicker.BiomeType) type = objectPicker.biomePicker2(biomeNoiseMap[i, j]);
                    
                    currentTile.setBiome(type.Item1, type.Item2);
                   
                    currentTile.setAdjacencyArray(inBetweenAdjuacency(currentTile, new Vector2(i, j)));

                    currentTile.setPos(allTilesCurrentIndex);

                    (bool, ObjectPicker.BiomeType) surrounded = checkIfSurrounded(currentTile, currentTile.getAdjacencyArray());
                    if (surrounded.Item1)
                    {
                        currentTile.setBiome(objectPicker.grassBiome,surrounded.Item2);
                    }
                    
                    
                }
                allTiles[(int)allTilesCurrentIndex.x, (int)allTilesCurrentIndex.y] = currentTile;
                buffer[i - 1, j - 1] = currentTile;
                allTilesCurrentIndex.y++;
            }
            allTilesCurrentIndex.y = allTilesIndex.y;
            allTilesCurrentIndex.x++;
        }
        allTilesCurrentIndex = allTilesIndex;
        //check for inBetween tiles, tried having these functions in the for loops above for efficency but "checkIfSurrounded" method call interfered with showing correct adjuacency of other tiles when checking the adjuacency of tiles
        for (int i = 1; i < biomeNoiseMap.GetUpperBound(0); i++)
        {
            for (int j = 1; j < biomeNoiseMap.GetUpperBound(1); j++)
            {
                Tile currentTile = allTiles[(int)allTilesCurrentIndex.x, (int)allTilesCurrentIndex.y];
                if (currentTile == null || currentTile.getTile() == null)
                {
                    bool found;
                    found = setInBetween(currentTile);
                    if (!found)
                    {
                        found = sortCorners(currentTile);
                    }
                    if (!found)
                    {
                        sortSpecial(currentTile);
                    }
                }
                allTilesCurrentIndex.y++;
            }
            allTilesCurrentIndex.y = allTilesIndex.y;
            allTilesCurrentIndex.x++;
        }
        

        inBetweenTerrainAdjuacency();
        isInBetween();
        setTiles();
        
    }
    /*
     * set the terrain adjuacency for inbetween tiles, so that adjuacent tiles could be other inBetween tiles
     */
    private void inBetweenTerrainAdjuacency()
    {
        foreach (Tile t in inBetweenList)
        {
            TerrainGeneration.directions initialDirection = TerrainGeneration.directions.SouthWest;
            Dictionary<TerrainGeneration.directions, Tile> adjuacencyArray = new Dictionary<TerrainGeneration.directions, Tile>();
            Vector2 initialIndex = t.getPos();
            initialIndex.x -= 1;
            initialIndex.y -= 1;
            for (int i = (int)initialIndex.x; i < initialIndex.x + 3; i++)
            {
                for (int j = (int)initialIndex.y; j < initialIndex.y + 3; j++)
                {
                    adjuacencyArray.Add(initialDirection, allTiles[i, j]);
                    initialDirection++;
                }
            }
            t.setAdjacencyArray(adjuacencyArray);
        }
    }
    /*
     * given a tile and its buffer index, figure out the adjuacent tiles
     * returns a dictionary with tile directions as key and the tiles themselves as values
     */
    private Dictionary<TerrainGeneration.directions, Tile> inBetweenAdjuacency(Tile m_currentTile, Vector2 m_iJ)
    {
        TerrainGeneration.directions initialDirection = TerrainGeneration.directions.SouthWest;
        
        Dictionary<TerrainGeneration.directions, Tile> adjuacencyArray = new Dictionary<TerrainGeneration.directions, Tile>();

        //find if this tile has adjuacent river tiles 

        ObjectPicker.BiomeType currentTileBiomeType = m_currentTile.getBiomeType();
        for (int i = (int)m_iJ.x - 1; i < (int)m_iJ.x + 2; i++)
        {
            for (int j = (int)m_iJ.y - 1; j < (int)m_iJ.y + 2; j++)
            {
             
                Tile tile = allTiles[(int)allTilesIndex.x + i-1, (int)allTilesIndex.y + j-1];
                if (allTilesIndex.x == 5005 && allTilesIndex.y == 5008)
                {
                    Debug.Log("here");
                }
                //if the adjuacent tile is null, create it and add it to our array
                if (tile == null)
                {

                    (Biome, ObjectPicker.BiomeType) stat = objectPicker.biomePicker2(biomeNoiseMap[i, j]);
                    ObjectPicker.BiomeType type = stat.Item2;
                    tile = new Tile();
                    tile.setBiome(stat.Item1, stat.Item2);

                    adjuacencyArray.Add(initialDirection, tile);
                    tile.setPos(new Vector2(allTilesIndex.x + i - 1,allTilesIndex.y + j - 1));
                    allTiles[(int)allTilesIndex.x + i - 1, (int)allTilesIndex.y + j - 1] = tile;

                }
                //if its already created, then add it to the adjuacency array
                else
                {

                    adjuacencyArray.Add(initialDirection, tile);

                }
                initialDirection++;
            }
        }
       
        return adjuacencyArray;
    }
    
    //after we know the adjuacncy of all the tiles we need, we need to figure out which tiles we can classify as inBetween
    private bool setInBetween(Tile currentTile)
    {
        bool found = false;
        if (currentTile != null && currentTile.getAdjacencyArray() != null && currentTile.getTile() == null)
        {
            
            Dictionary<TerrainGeneration.directions, Tile> adjuacencyArray = currentTile.getAdjacencyArray();
            ObjectPicker.BiomeType currentTileBiomeType = currentTile.getBiomeType();
            //we dont check grassbiome tiles so that we dont have double the inbetween tiles
            if (currentTile.getBiomeType() != ObjectPicker.BiomeType.grassBiome)
            {
                //all of these if/else are rules that classify if a tile is inbetween, if it is, we add to our inbetween list   
                if (adjuacencyArray[TerrainGeneration.directions.East].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.NorthEast].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.North].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.East].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.SouthEast].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.South].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.South].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.SouthWest].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.NorthWest].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.North].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    inBetweenList.Add(currentTile);
                    found = true;
                }

                else if (adjuacencyArray[TerrainGeneration.directions.NorthWest].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.North].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.NorthEast].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.NorthEast].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.East].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.SouthEast].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.SouthWest].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.South].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.SouthEast].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.SouthWest].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.NorthWest].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                        


            }
                    
                    
                
            
        }
        return found;
    }
   

   /*
    * same as above but for grass biome tiles
    */
    private bool sortCorners(Tile currentTile)
    {
        bool found = false;
        if (currentTile != null && currentTile.getAdjacencyArray() != null && currentTile.getTile() == null)
        {
            Dictionary<TerrainGeneration.directions, Tile> adjuacencyArray = currentTile.getAdjacencyArray();
            ObjectPicker.BiomeType currentTileBiomeType = currentTile.getBiomeType();
            //we only check grass biome tiles because a grass tile might exist in between 2 "inbetween" tiles at a corner
            if (currentTileBiomeType == ObjectPicker.BiomeType.grassBiome)
            {
                //these are rules that determine if a tile is in between, if it is we add it to our array
                if (adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.NorthWest].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.North].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.North].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.NorthEast].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.East].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.East].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.SouthEast].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.South].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.South].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.SouthWest].getBiomeType() != currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                       

            }
        }
        return found;
            
        
    }

    /*
     * same as above except we check for inbetween tiles in unique situations that are not coverd in the above functions
     * could probebly accept a tile as input instead of having all of these for loops, will try after presentation created
     */
    private bool sortSpecial(Tile currentTile)
    {
        bool found = false;
        if (currentTile != null && currentTile.getAdjacencyArray() != null && currentTile.getTile() == null)
        {
            Dictionary<TerrainGeneration.directions, Tile> adjuacencyArray = currentTile.getAdjacencyArray();
            ObjectPicker.BiomeType currentTileBiomeType = currentTile.getBiomeType();
            ObjectPicker.TerrainType terrainType = ObjectPicker.TerrainType.inBetween;
            if (currentTile.getBiomeType() != ObjectPicker.BiomeType.grassBiome)
            {
                if (adjuacencyArray[TerrainGeneration.directions.NorthWest].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.South].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.NorthEast].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.East].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.SouthEast].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.South].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.SouthWest].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.East].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.South].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.NorthEast].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.North].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.NorthWest].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.East].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.North].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.SouthWest].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.North].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }
                else if (adjuacencyArray[TerrainGeneration.directions.SouthEast].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.North].getBiomeType() == currentTileBiomeType && adjuacencyArray[TerrainGeneration.directions.East].getBiomeType() != currentTileBiomeType)
                {
                    currentTile.setTerrainType(ObjectPicker.TerrainType.inBetween);
                    currentTile.setSpawnRiver(false);
                    inBetweenList.Add(currentTile);
                    found = true;
                }


            }
        }

        return found;
    }
    private void isInBetween()
    {
        for (int i = inBetweenList.Count - 1; i >= 0; i--)
        {
            Tile t = inBetweenList[i];

            if (t.getTile() == null)
            {
                Dictionary<TerrainGeneration.directions, Tile> adjuacncyArray = t.getAdjacencyArray();
                if (adjuacncyArray != null)
                {
                    if (t.getPos().x >= allTilesCurrentIndex.x && t.getPos().y >= allTilesCurrentIndex.y && t.getPos().x < allTilesCurrentIndex.x + 3 && t.getPos().y < allTilesCurrentIndex.y + 3)
                    {
                        if (adjuacncyArray[TerrainGeneration.directions.South].getTerrainType() == ObjectPicker.TerrainType.inBetween && adjuacncyArray[TerrainGeneration.directions.West].getTerrainType() == ObjectPicker.TerrainType.inBetween && adjuacncyArray[TerrainGeneration.directions.North].getTerrainType() == ObjectPicker.TerrainType.inBetween && adjuacncyArray[TerrainGeneration.directions.East].getTerrainType() == ObjectPicker.TerrainType.inBetween)
                        {
                            inBetweenList.RemoveAt(i);
                            t.setTerrainType(ObjectPicker.TerrainType.normal);
                            allTiles[(int)t.getPos().x, (int)t.getPos().y] = t;
                        }
                    }
                }
            }
            else
            {
                inBetweenList.RemoveAt(i);
            }
        }

    }
    //now that we know which tiles are in between, we need to know that kind of inbetween tiles they are, based on their adjuacency to other "in between" tiles
    private void setTiles()
    {
        foreach (Tile t in inBetweenList)
        {
            Dictionary<TerrainGeneration.directions, Tile> adjuacencyArray = t.getAdjacencyArray();
            //we need to check that the tiles position is inside the buffer, so that we dont run the risk of looking at tiles without having their adjuacency checked
            if (t.getPos().x >= allTilesCurrentIndex.x && t.getPos().y >= allTilesCurrentIndex.y && t.getPos().x < allTilesCurrentIndex.x + 3 && t.getPos().y < allTilesCurrentIndex.y + 3)
            {
                if (t.getPos().x == 5005 && t.getPos().y == 5008)
                {
                  Debug.Log("Here");
                }
                if (adjuacencyArray[TerrainGeneration.directions.North].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.East].getTerrainType() == t.getTerrainType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.corner));
                    t.setRotation(0);
                    t.setSpawnRiver(false);
                    t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.North].getBiomeType()));
                }
                else if (adjuacencyArray[TerrainGeneration.directions.East].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.South].getTerrainType() == t.getTerrainType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.corner));
                    t.setRotation(90);
                    t.setSpawnRiver(false);
                    t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.East].getBiomeType()));
                }
                else if (adjuacencyArray[TerrainGeneration.directions.South].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.West].getTerrainType() == t.getTerrainType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.corner));
                    t.setRotation(180);
                    t.setSpawnRiver(false);
                    t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.South].getBiomeType()));
                }
                else if (adjuacencyArray[TerrainGeneration.directions.West].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.North].getTerrainType() == t.getTerrainType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.corner));
                    t.setRotation(-90);
                    t.setSpawnRiver(false);
                    t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.West].getBiomeType()));
                }

                else if (adjuacencyArray[TerrainGeneration.directions.North].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.South].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.East].getBiomeType() == t.getBiomeType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.streight));
                    t.setRotation(-90);
                    t.setSpawnRiver(false);
                    t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.North].getBiomeType()));
                }
                else if (adjuacencyArray[TerrainGeneration.directions.North].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.South].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() == t.getBiomeType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.streight));
                    t.setRotation(90);
                    t.setSpawnRiver(false);
                    t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.North].getBiomeType()));
                }
                else if (adjuacencyArray[TerrainGeneration.directions.East].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.West].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.North].getBiomeType() == t.getBiomeType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.streight));
                    t.setRotation(180);
                    t.setSpawnRiver(false);
                    t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.East].getBiomeType()));
                }
                else if (adjuacencyArray[TerrainGeneration.directions.East].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.West].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.South].getBiomeType() == t.getBiomeType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.streight));
                    t.setRotation(0);
                    t.setSpawnRiver(false);
                    t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.East].getBiomeType()));
                }
                //special cases below
                else if (adjuacencyArray[TerrainGeneration.directions.East].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.SouthWest].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.South].getBiomeType() != t.getBiomeType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.corner));
                    t.setRotation(180);
                    t.setSpawnRiver(false);
                    if (t.getBiomeType() == ObjectPicker.BiomeType.grassBiome)
                    {
                        t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.South].getBiomeType()));
                    }
                    else
                    {
                        t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.North].getBiomeType()));
                    }
                }
                else if (adjuacencyArray[TerrainGeneration.directions.NorthEast].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() == t.getBiomeType() && adjuacencyArray[TerrainGeneration.directions.East].getBiomeType() != t.getBiomeType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.streight));
                    t.setRotation(90);
                    t.setSpawnRiver(false);
                    if (t.getBiomeType() == ObjectPicker.BiomeType.grassBiome)
                    {
                        t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.East].getBiomeType()));
                    }
                    else
                    {
                        t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.West].getBiomeType()));
                    }
                }
                else if (adjuacencyArray[TerrainGeneration.directions.SouthWest].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.North].getBiomeType() == t.getBiomeType() && adjuacencyArray[TerrainGeneration.directions.South].getBiomeType() != t.getBiomeType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.streight));
                    t.setRotation(180);
                    t.setSpawnRiver(false);
                    if (t.getBiomeType() == ObjectPicker.BiomeType.grassBiome)
                    {
                        t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.South].getBiomeType()));
                    }
                    else
                    {
                        t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.North].getBiomeType()));
                    }
                }
                else if (adjuacencyArray[TerrainGeneration.directions.SouthWest].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.North].getBiomeType() == t.getBiomeType() && adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() != t.getBiomeType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.streight));
                    t.setRotation(180);
                    t.setSpawnRiver(false);
                    if (t.getBiomeType() == ObjectPicker.BiomeType.grassBiome)
                    {
                        t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.West].getBiomeType()));
                    }
                    else
                    {
                        t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.North].getBiomeType()));
                    }
                }
                
                else if (adjuacencyArray[TerrainGeneration.directions.South].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.NorthWest].getTerrainType() == t.getTerrainType() && adjuacencyArray[TerrainGeneration.directions.West].getBiomeType() != t.getBiomeType())
                {
                    t.setTile(t.getBiome().adjuacentPicker(ObjectPicker.TileType.corner));
                    t.setRotation(180);
                    t.setSpawnRiver(false);
                    if (t.getBiomeType() == ObjectPicker.BiomeType.grassBiome)
                    {
                        t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.West].getBiomeType()));
                    }
                    else
                    {
                        t.setMaterial(t.getBiome().getInBetweenTileMaterial(adjuacencyArray[TerrainGeneration.directions.North].getBiomeType()));
                    }
                }
                

            }

            allTiles[(int)t.getPos().x, (int)t.getPos().y] = t;

        }
    }
    /*
     * check if a tile is surronded by 5 or more tiles of a different biome, if so convert the tile to the majority biome
     */
    private (bool,ObjectPicker.BiomeType) checkIfSurrounded(Tile m_currentTile,Dictionary<TerrainGeneration.directions, Tile> m_adjuacencyArray)
    {
        ObjectPicker.BiomeType currentBiomeType = m_currentTile.getBiomeType();
        ObjectPicker.BiomeType otherBiomeType = 0;
        int count = 0;
        foreach (KeyValuePair<TerrainGeneration.directions, Tile> entry in m_adjuacencyArray)
        {
            if (entry.Value.getBiomeType() != currentBiomeType && entry.Key != TerrainGeneration.directions.Center)
            {
                count++;
                otherBiomeType = entry.Value.getBiomeType();
            }
        }
        if (count >= 5)
        {
            m_currentTile.setTerrainType(ObjectPicker.TerrainType.normal);
            return (true, otherBiomeType);
            
        }

        return (false,0);
    }
    
        
    public Tile[,] getAllTiles()
    {
        return allTiles;
    }
}
