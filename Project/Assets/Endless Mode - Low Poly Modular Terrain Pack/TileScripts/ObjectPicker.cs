using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script to sort terrain - Dvir
public class ObjectPicker : MonoBehaviour
{
    //handy enums for classification of tiles
    public enum BiomeType
    {
        grassBiome,
        rockBiome,
        snowBiome,

    }

    public enum TerrainType
    {
        river,
        normal,
        inBetween
    }
    public enum TileType
    {
        corner,
        streight,
        diagonal,
        end,
        turn
    }

    //refernces to our biomes
    public GrassBiome grassBiome;
    [SerializeField] private RockBiome rockBiome;
    [SerializeField] private SnowBiome snowBiome;

    //refernces to our sorting scripts
    private InBetweenSort inBetweenScript;
    private RiverSort riverScript;
    private TileSort tileScript;

    //refernce to alltTiles array and currentIndex
    Tile[,] allTiles;
    Vector2 allTilesIndex;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setAllTiles(Tile[,] m_allTiles)
    {
        allTiles = m_allTiles;
    }
    public void setTerrainGenerator(TerrainGeneration m_terrain, InBetweenSort m_inBetween, RiverSort m_riverSort, TileSort m_tileSort)
    {
        inBetweenScript = m_inBetween;
        riverScript = m_riverSort;
        tileScript = m_tileSort;
    }

   
    //Pick a biome and biomeType based on given value
    public (Biome, BiomeType) biomePicker2(float num)
    {
        if (num < 0.3f)
        {
            return (rockBiome, BiomeType.rockBiome);
        }

        else if (num >= 0.3f && num < 0.7f)
        {
            return (grassBiome, BiomeType.grassBiome);
        }

        return (snowBiome, BiomeType.snowBiome);


    }


    //given all of the noise maps for the terrain generate all the tiles we need, we sort by priority: "inbetween" > river > regular
    public Tile[,] sortNoiseMap(float[,] m_biomeNoiseMap, float[,] m_tileNoiseMap, float[,] riverNoiseMap, Tile[,] m_allTiles, Vector2 m_allTilesIndex)
    {
        Vector2 allTilesCurrentIndex = m_allTilesIndex;
        allTiles = m_allTiles;
        allTilesIndex = m_allTilesIndex;


        inBetweenScript.init(new Vector2(3, 3), allTiles, allTilesIndex, m_biomeNoiseMap);
        inBetweenScript.sort();
        allTiles = inBetweenScript.getAllTiles();

        riverScript.init(new Vector2(3, 3), allTiles, allTilesIndex, riverNoiseMap, m_biomeNoiseMap);
        riverScript.sort();


        tileScript.init(new Vector2(3, 3), allTiles, allTilesIndex, m_tileNoiseMap);
        tileScript.sort();
        allTiles = tileScript.getAllTiles();

        return allTiles;

    }
}
