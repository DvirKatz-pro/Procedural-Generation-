using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains information about a tile- Dvir
public class Tile
{
    //member variables
    Biome biome;
    GameObject tile;
    float rotation;
    ObjectPicker.BiomeType biomeType = 0;
    ObjectPicker.TerrainType terrainType = ObjectPicker.TerrainType.normal;
    bool spawnObjects = true;
    bool spawnRivers = true;
    Material material;
    Vector2 pos;

    private Dictionary<TerrainGeneration.directions, Tile> adjuacentArray;

    /*
     * set this tiles biome and biome type
     */
    public void setBiome(Biome m_biome, ObjectPicker.BiomeType m_type)
    {
        biome = m_biome;
        biomeType = m_type;
    }
    /*
    * set this tiles Gameobject
    */
    public void setTile(GameObject m_tile)
    {
        tile = m_tile;
    }
    /*
   * set this tiles BiomeType
   */
    public void setBiomeType(ObjectPicker.BiomeType m_type)
    {
        biomeType = m_type;
    }
    /*
    * set this tiles rotation
    */
    public void setRotation(float m_rotation)
    {
        rotation = m_rotation;
    }
    /*
    * set if this tile should spawn objects
    */
    public void setSpawnObjects(bool m_spawn)
    {
        spawnObjects = m_spawn;
    }
    /*
    * return if this should spawn objects
    */
    public bool shouldSpawn()
    {
        return spawnObjects;
    }
    /*
    * set if this tile can spawn a river
    */
    public void setSpawnRiver(bool m_spawn)
    {
        spawnRivers = m_spawn;
    }
    /*
    * return if this tile can spawn a river
    */
    public bool shouldSpawnRiver()
    {
        return spawnRivers;
    }
    /*
    * return tile rotation
    */
    public float getRotation()
    {
        return rotation;
    }
    /*
   * return tile biome
   */
    public Biome getBiome()
    {
        return biome;
    }
    /*
   * return tile biomeType
   */
    public ObjectPicker.BiomeType getBiomeType()
    {
        return biomeType;
    }
    /*
   * set tile terrainType
   */
    public void setTerrainType(ObjectPicker.TerrainType m_type)
    {
        terrainType = m_type;
    }
    /*
   * return tile terrainType
   */
    public ObjectPicker.TerrainType getTerrainType()
    {
        return terrainType;
    }
    /*
   * return tile gameobject
   */
    public GameObject getTile()
    {
        return tile;
    }
    /*
    * set tile material
    */
    public void setMaterial(Material m)
    {
        material = m;
    }
    /*
    * return tile material
    */
    public Material getMaterial()
    {
        return material;
    }
    /*
    * set the tiles that are adjacent to this one
    */
    public void setAdjacencyArray(Dictionary<TerrainGeneration.directions, Tile> m_adjuacentArray)
    {
        adjuacentArray = m_adjuacentArray;
    }
    /*
    * return the tiles that are adjacent to this one
    */
    public Dictionary<TerrainGeneration.directions, Tile> getAdjacencyArray()
    {
        return adjuacentArray;
    }
    /*
    * set the tiles position in the allTiles array
    */
    public void setPos(Vector2 m_pos)
    {
        pos = m_pos;
    }
    /*
    * return the tiles position in the allTiles array
    */
    public Vector2 getPos()
    {
        return pos;
    }

   


}
