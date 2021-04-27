using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



//script to generate terrain - Dvir
public class TerrainGeneration : MonoBehaviour
{
    Vector3 playerPosition;
    //the row length and column length of the buffer
    [SerializeField] private Vector2 rowColLen;
 
    //scales for the noise maps  
    [SerializeField] private float biomeScale;
    [SerializeField] private float tileScale;
    [SerializeField] private float objectScale;
    [SerializeField] private float riverScale;
    [SerializeField] private float townScale;

    [SerializeField] private float enemyTimer = 0.5f;
    private float currentEnemyTime = 0;

    //amount of objects (trees/rocks) we want per tile
    [SerializeField] private int objectNumber;

    //reference to gameobjects we need
    [SerializeField] private GameObject trigger;
    [SerializeField] private GameObject grid;

    //offset for the noise maps
    [SerializeField] private Vector2 biomeOffSet;
    [SerializeField] private Vector2 tileOffSet;
    [SerializeField] private Vector2 objectOffSet;
    [SerializeField] private Vector2 riverOffSet;
    [SerializeField] private Vector2 townOffSet;

    //a list that holds all objects (trees/rocks)
    private List<GameObject> allObjects; 

    //script references
    private ObjectPicker objectPicker;
    private NoiseGenerator noise;
    private EnemySpawn enemySpawn;
    private TownSpawn townSpawn;
    private LightingControl light;

    //2d array that holds the buffer tiles
    private GameObject[,] tileGrids;

    //2d array (Tile reference,tile type,initalRotation,pivot rotation)
    //private (GameObject, string,float)[,] allTileGrids;
    private Tile[,] allTileGrids;
    //keeps track of our current index for the 2d array above ^
    private Vector2 allTileGridsIndex;
    private Vector2 currentAllTileGridsIndex;

    //dictionaries that convert a Vector to a direction and vice versa
    private Dictionary<directions, Vector2> dirToIndex;
    private Dictionary<Vector2, directions> indexToDir;

    //how much space each object (tree/rock) can occupy in world units
    private float objectTileSize;

    //the size of 1 tile in world units
    private Vector3 tileSize;

    //variables for noise function
    public int octaves;
    public float persistance;
    public float lacunarity;

    //reference to our current world position
    private Vector3 tilePositions;

    

    private List<float> worldSeed = new List<float>();


    //handy enum to access directions
    public enum directions
    {
        SouthWest,
        West,
        NorthWest,
        South,
        Center,
        North,
        SouthEast,
        East,
        NorthEast
      
    }

    // Start is called before the first frame update
    private void Start()
    {
        light = GameObject.Find("Directional Light").GetComponent<LightingControl>();
        //generate random seed
        biomeOffSet = new Vector2(Random.Range(-1000000, 1000000), Random.Range(-1000000, 1000000));
        tileOffSet = new Vector2(Random.Range(-1000000, 1000000), Random.Range(-1000000, 1000000));
        riverOffSet = new Vector2(Random.Range(-1000000, 1000000), Random.Range(-1000000, 1000000));
        objectOffSet = new Vector2(Random.Range(-1000000, 1000000), Random.Range(-1000000, 1000000));
        //town offset is set to a much smaller offset because the town scale is already a very small number, so calculating the offset for the buildings lead to unpredictable behaviour
        //because of the way that floats are stored, note: doubles can not be used because perlin noise function from unity only takes floats
        townOffSet = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
        
        Init();
    }
    void Init()
    {
        

        allObjects = new List<GameObject>();
        
        
       //set world seed

        worldSeed.Add(biomeOffSet.x);
        worldSeed.Add(biomeOffSet.y);

        worldSeed.Add(tileOffSet.x);
        worldSeed.Add(tileOffSet.y);

        worldSeed.Add(objectOffSet.x);
        worldSeed.Add(objectOffSet.y);

        worldSeed.Add(riverOffSet.x);
        worldSeed.Add(riverOffSet.y);

        worldSeed.Add(townOffSet.x);
        worldSeed.Add(townOffSet.y);

        string x = "";
        foreach (float t in worldSeed)
        {
            x += " " + t.ToString();
        }
        //print world seed
        Debug.Log(x);
        objectPicker = GetComponent<ObjectPicker>();
        noise = GetComponent<NoiseGenerator>();
        objectPicker.setTerrainGenerator(GetComponent<TerrainGeneration>(),GetComponent<InBetweenSort>(),GetComponent<RiverSort>(),GetComponent<TileSort>());
        enemySpawn = GetComponent<EnemySpawn>();
        townSpawn = GetComponent<TownSpawn>();

        //set our array position to be the center 
        
        allTileGrids = new Tile[10000, 10000];
        allTileGridsIndex.x = allTileGrids.GetLength(0) / 2 - 1;
        allTileGridsIndex.y = allTileGrids.GetLength(0) / 2 - 1;
        currentAllTileGridsIndex = allTileGridsIndex;

        playerPosition = new Vector3(0f, 100f,0f);

        GameObject.Find("Player").transform.position = playerPosition;

        
        tileGrids = new GameObject[(int)rowColLen.x,(int)rowColLen.y];

        //Initialize our dictionaries
        dirToIndex = new Dictionary<directions, Vector2>()
        {
            {directions.SouthWest,new Vector2(0,0)},
            {directions.West, new Vector2(0, 1)},
            {directions.NorthWest,new Vector2(0, 2)},
            {directions.South, new Vector2(1, 0)},
            {directions.Center , new Vector2(1, 1) },
            {directions.North, new Vector2(1, 2) },
            {directions.SouthEast, new Vector2(2, 0) },
            {directions.East, new Vector2(2, 1)},
            { directions.NorthEast, new Vector2(2, 2)}
        };



        indexToDir = new Dictionary<Vector2, directions>()
        {
            {new Vector2(0, 0), directions.SouthWest},
            {new Vector2(0, 1), directions.West},
            {new Vector2(0, 2), directions.NorthWest},
            {new Vector2(1, 0), directions.South},
            {new Vector2(1, 1), directions.Center},
            {new Vector2(1, 2), directions.North},
            {new Vector2(2, 0), directions.SouthEast },
            {new Vector2(2, 1), directions.East},
            {new Vector2(2, 2), directions.NorthEast},
        };

        //calculate the size each object can be in world units 
        tilePositions = Vector3.zero;
        float size = 25;
        objectTileSize = (float)(size * 2) / objectNumber;

        //generate the terrain for the first time
        reGenerateTerrain();

        //get the size of the center tile, calculate the middle position of the tile and place the player there
        tilePositions = Vector3.zero;
        Vector2 index = dirToIndex[directions.Center];
        GameObject centerTile = tileGrids[(int)index.x,(int)index.y].transform.GetChild(0).gameObject;
        float sizeX = centerTile.GetComponent<MeshRenderer>().bounds.size.x;
        float sizeZ = centerTile.GetComponent<MeshRenderer>().bounds.size.z;
        
        
        playerPosition.x += sizeX + (float)(sizeX / 2.0f);
        playerPosition.y += 10f;
        playerPosition.z += sizeX + (float)(sizeZ / 2.0f);

        //spawn player on top of ground
        RaycastHit hit;
        Ray ray = new Ray(playerPosition + Vector3.up * 100, Vector3.down);
        LayerMask mask = LayerMask.GetMask("Default");
        bool found = false;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity,mask,QueryTriggerInteraction.Ignore))
        {
            if (hit.collider != null)
            {
                
                if (hit.collider.gameObject.tag == "Building")
                {
                    playerPosition.z += 12;
                    playerPosition.x += 12;
                    playerPosition.y = 0;
                    found = true;
                }
            }
            
        }
        RaycastHit hit2;
        mask = LayerMask.GetMask("Terrain");
        if (!found)
        {
            if (Physics.Raycast(ray, out hit2, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
            {
                if (hit2.collider != null)
                {

                    playerPosition.y = hit2.point.y + 0.2f;
                }

            }
        }
        playerPosition.y += 0.2f;
        GameObject.Find("Player").transform.position = playerPosition;

        //set the size of 1 tile in world units
        tileSize = new Vector3(sizeX, 0,sizeZ);
        //set the time to morining time
        light.resetDay();


       
    }

    // Update is called once per frame
    void Update()
    {
        //every time the timer reaches zero, try to spawn enemies
        if (currentEnemyTime < enemyTimer)
        {
            currentEnemyTime += Time.deltaTime;
        }
        else
        {
            float[,] enemyNoiseMap = noise.generateNoiseMap(25 * 3, 25 * 3, tileScale, tileOffSet);
            enemySpawn.reGenerateEnemies(tilePositions, enemyNoiseMap);
            currentEnemyTime = 0;
        }
    }
   /*
    * takes a string position, and shift noise maps, world position and allTileGrids array in that direction
    */
    public void buffer(directions position)
    {
        switch (position)        
        {
            case (directions.SouthWest):
            {

                    tilePositions.x -= tileSize.x;
                    tilePositions.z -= tileSize.z;
                    biomeOffSet.x -= biomeScale / (rowColLen.x + 8.0f);
                    biomeOffSet.y -= biomeScale / (rowColLen.y + 8.0f);
                    tileOffSet.x -= tileScale / (rowColLen.x + 8.0f);
                    tileOffSet.y -= tileScale / (rowColLen.y + 8.0f);
                    townOffSet.x -= townScale / (rowColLen.x);
                    townOffSet.y -= townScale / (rowColLen.y);
                    objectOffSet.x -= objectScale;
                    objectOffSet.y -= objectScale;
                    riverOffSet.x -= 1;
                    riverOffSet.y -= 1;
                    allTileGridsIndex.x -= 1;
                    allTileGridsIndex.y -= 1;
                    currentAllTileGridsIndex = allTileGridsIndex;
                    reGenerateTerrain();
                    break;
            }
            case (directions.West):
            {
                    tilePositions.x -= tileSize.x;
                    biomeOffSet.x -= biomeScale / (rowColLen.x + 8.0f);
                    tileOffSet.x -= tileScale / (rowColLen.x + 8.0f);
                    townOffSet.x -= townScale / (rowColLen.x);
                    objectOffSet.x -= objectScale;
                    riverOffSet.x -= 1;
                    allTileGridsIndex.x -= 1;

                    currentAllTileGridsIndex = allTileGridsIndex;
                    reGenerateTerrain();

                    break;
            }
            case (directions.NorthWest):
            {
                    tilePositions.x -= tileSize.x;
                    tilePositions.z += tileSize.z;
                    biomeOffSet.x -= biomeScale / (rowColLen.x + 8.0f);
                    biomeOffSet.y += biomeScale / (rowColLen.y + 8.0f);
                    tileOffSet.x -= tileScale / (rowColLen.x + 8.0f);
                    tileOffSet.y += tileScale / (rowColLen.y + 8.0f);
                    townOffSet.x -= townScale / (rowColLen.x);
                    townOffSet.y += townScale / (rowColLen.y);
                    objectOffSet.x -= objectScale;
                    objectOffSet.y += objectScale;
                    riverOffSet.x -= 1;
                    riverOffSet.y += 1;
                    allTileGridsIndex.x -= 1;
                    allTileGridsIndex.y += 1;
                    currentAllTileGridsIndex = allTileGridsIndex;
                    reGenerateTerrain();

                    break;
            }
            case (directions.South):
            {                   
                    tilePositions.z -= tileSize.z;
                    biomeOffSet.y -= biomeScale / (rowColLen.y + 8.0f);
                    tileOffSet.y -= tileScale / (rowColLen.y + 8.0f);
                    townOffSet.y -= townScale / (rowColLen.y);
                    objectOffSet.y -= objectScale;
                    riverOffSet.y -= 1;
                    allTileGridsIndex.y -= 1;
                    currentAllTileGridsIndex = allTileGridsIndex;
                    reGenerateTerrain();
                    break;
            }
            case (directions.Center):
            {
                break;
            }
            case (directions.North):
            {
                    tilePositions.z += tileSize.z;
                    biomeOffSet.y += biomeScale / (rowColLen.y + 8.0f);
                    tileOffSet.y += tileScale / (rowColLen.y + 8.0f);
                    townOffSet.y += townScale / (rowColLen.y);
                    objectOffSet.y += objectScale;                   
                    riverOffSet.y += 1;

                    allTileGridsIndex.y += 1;
                    currentAllTileGridsIndex = allTileGridsIndex;
                    reGenerateTerrain();
                    break;
            }
            case (directions.SouthEast):
            {
                    tilePositions.x += tileSize.x;
                    tilePositions.z -= tileSize.z;
                    biomeOffSet.x += biomeScale / (rowColLen.x + 8.0f);
                    biomeOffSet.y -= biomeScale / (rowColLen.y + 8.0f);
                    tileOffSet.x += tileScale / (rowColLen.x + 8.0f);
                    tileOffSet.y -= tileScale / (rowColLen.y + 8.0f);
                    townOffSet.x += townScale / (rowColLen.x);
                    townOffSet.y -= townScale / (rowColLen.y);
                    objectOffSet.x += objectScale;
                    objectOffSet.y -= objectScale;
                    riverOffSet.x += 1;
                    riverOffSet.y -= 1;
                    allTileGridsIndex.x += 1;
                    allTileGridsIndex.y -= 1;
                    currentAllTileGridsIndex = allTileGridsIndex;
                    reGenerateTerrain();
                    break;
            }
            case (directions.East):
            {
                    tilePositions.x += tileSize.x;
                    biomeOffSet.x += biomeScale / (rowColLen.x + 8.0f);
                    tileOffSet.x += tileScale / (rowColLen.x + 8.0f);
                    townOffSet.x += townScale / (rowColLen.x);
                    objectOffSet.x += objectScale;
                    riverOffSet.x += 1;
                    allTileGridsIndex.x += 1;
                    currentAllTileGridsIndex = allTileGridsIndex;
                    reGenerateTerrain();

                  
                 
                    break;
            }
            case (directions.NorthEast):
            {
                    tilePositions.x += tileSize.x;
                    tilePositions.z += tileSize.z;
                    biomeOffSet.x += biomeScale / (rowColLen.x + 8.0f);
                    biomeOffSet.y += biomeScale / (rowColLen.y + 8.0f);
                    tileOffSet.x += tileScale / (rowColLen.x + 8.0f);
                    tileOffSet.y += tileScale / (rowColLen.y + 8.0f);
                    townOffSet.x += townScale / (rowColLen.x);
                    townOffSet.y += townScale / (rowColLen.y);
                    objectOffSet.x += objectScale;
                    objectOffSet.y += objectScale;
                    riverOffSet.x += 1;
                    riverOffSet.y += 1;
                    allTileGridsIndex.x += 1;
                    allTileGridsIndex.y += 1;
                    currentAllTileGridsIndex = allTileGridsIndex;
                    reGenerateTerrain();
                    break;
            }
        }
    }
    /*
     * iterate through buffer and spawn tiles
     */
    private void reGenerateTerrain()
    {
        //get local variables so that we are modifing a copy and not the original
        Vector3 currentTilePositions = tilePositions;
        Vector2 currentObjectOffSet = objectOffSet;
        //calculate the noise maps
        float[,] riverNoiseMap = noise.generateFractalNoiseMap((int)rowColLen.x + 4, (int)rowColLen.y + 4, riverScale, octaves, persistance, lacunarity, riverOffSet);
        float[,] biomeNoiseMap = noise.generateNoiseMap((int)rowColLen.x + 8, (int)rowColLen.y + 8, biomeScale, biomeOffSet);
        float[,] tileNoiseMap = noise.generateNoiseMap((int)rowColLen.x + 8, (int)rowColLen.y + 8, tileScale, tileOffSet);
        float[,] townNoiseMap = noise.generateNoiseMap((int)rowColLen.x, (int)rowColLen.y, townScale, townOffSet);
        //bool to know if we should generate objects (trees/rocks) for a tile
        bool genObjects = false;

        allTileGrids = objectPicker.sortNoiseMap(biomeNoiseMap, tileNoiseMap, riverNoiseMap, allTileGrids,currentAllTileGridsIndex);
      
        //destroy all objects so that we dont have any leftover objects that shouldnt exist
        foreach (GameObject g in allObjects)
        {
            Destroy(g);
        }
        allObjects.Clear();
        enemySpawn.deleteEnemies();
        townSpawn.destroyBuildings();

        //start generating tiles for our buffer
        for (int i = 0; i < rowColLen.x; i++)
        {
            for (int j = 0; j < rowColLen.y; j++)
            {
                //destroy all tiles, so that tiles that shouldnt exist are not leftover
                if (tileGrids[i, j] != null)
                {
                    Destroy(tileGrids[i, j]);
                }

                //generate an empty gameobject with trigger collider to know where player is in the buffer
                GameObject triggerClone = Instantiate(trigger, new Vector3(currentTilePositions.x + (25/2.0f),0, currentTilePositions.z + (25/2.0f)), Quaternion.identity);
                triggerClone.transform.parent = grid.transform;
                tileGrids[i, j] = triggerClone;
                triggerClone.GetComponent<GridTrigger>().setTerrainGenerator(GetComponent <TerrainGeneration>());
                triggerClone.GetComponent<GridTrigger>().setPosition(indexToDirection(new Vector2(i, j)));

                //get the tile for this position and spawn it using our saved data from its tile script
                Tile tile = allTileGrids[(int)currentAllTileGridsIndex.x, (int)currentAllTileGridsIndex.y];
                genObjects = tile.shouldSpawn();
                GameObject tileObject = tile.getTile();
                tileObject.layer = LayerMask.NameToLayer("Terrain");
                float rot = tile.getRotation();
               
                tileObject = Instantiate(tileObject, new Vector3(currentTilePositions.x, 0, currentTilePositions.z), tileObject.transform.rotation);
                Material m = tile.getMaterial();
                tileObject.transform.parent = triggerClone.transform;
                triggerClone.transform.rotation = Quaternion.Euler(0, rot, 0);
                tileObject.GetComponent<Renderer>().material = m;

                //if this tile is a river, spawn water
                if (tile.getTerrainType() == ObjectPicker.TerrainType.river)
                {
                    GameObject waterTile = Instantiate(tile.getBiome().waterPicker(), new Vector3(currentTilePositions.x, -0.5f, currentTilePositions.z), Quaternion.identity);
                    waterTile.transform.parent = tileObject.transform;
                }

                //if we can generate objects, generate buildings
                if (genObjects)
                {
                    bool canSpawn = townSpawn.spawnBuildings(tile, triggerClone, townNoiseMap[i, j]);
                    //if we cant spawn buildings, then we can spawn objects
                    if (canSpawn)
                    {
                        reGenerateObjects(currentTilePositions, currentObjectOffSet, tile.getBiome(), tileObject);
                    }
                    
                    genObjects = false;
                }
                 
                //set the world position and the index position of where the next tile should be placed 
                currentObjectOffSet.y += 25;
                currentTilePositions.z += 25;
                currentAllTileGridsIndex.y++;
            }
            //set the world position and the index position of where the next tile should be placed, also move the object noise offset so that we dont generate the same objects
            currentObjectOffSet.y = objectOffSet.y;
            currentObjectOffSet.x += 25;
            currentTilePositions.z = tilePositions.z;
            currentTilePositions.x += 25;
            currentAllTileGridsIndex.y = allTileGridsIndex.y;
            currentAllTileGridsIndex.x++;
        }
        currentAllTileGridsIndex = allTileGridsIndex;
        
    }

    /*
     * generate generic objects at a tile
     */
    private void reGenerateObjects(Vector3 m_tilePositions, Vector2 m_objectOffset,Biome m_biome,GameObject m_tile)
    {

        
        //create an array of objects and generate noise map   
        GameObject[,] objectGrids = new GameObject[objectNumber / 2, objectNumber / 2];
        
        float [,] objectNoiseMap = noise.generateNoiseMap(objectGrids.GetUpperBound(0) + 1, objectGrids.GetUpperBound(1) + 1, objectScale, m_objectOffset);

        int distance = 0;
        for (int i = 0; i < objectGrids.GetUpperBound(0)+1; i++)
        {
            for (int j = 0; j < objectGrids.GetUpperBound(1)+1; j++)
            {
                //generate the noise value for this object
                float objectNoiseValue = objectNoiseMap[i, j];

               // get the object, size of it
                KeyValuePair<GameObject,int> pair = m_biome.objectPicker(objectNoiseValue);
                
                //if we have an object
                if (pair.Key != null)
                {
                    
                    distance = pair.Value;
                   
                    //get its position on the tile 
                    Vector3 objectPosition = m_tilePositions;
                    objectPosition.x += objectTileSize * i;
                    objectPosition.z += objectTileSize * j;

                    //spawn on top of ground
                    RaycastHit hit;
                    Ray ray = new Ray(objectPosition + Vector3.up * 100, Vector3.down);
                    LayerMask mask = LayerMask.GetMask("Terrain");
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                    {
                        if (hit.collider != null)
                        {
                            objectPosition = new Vector3(objectPosition.x, hit.point.y, objectPosition.z);
                            objectPosition.y -= 0.5f;
                        }
                        else
                        {
                            Debug.Log("No Ground");
                        }
                    }

                    //Instantiate the object and add it to the list
                    GameObject gameObject = Instantiate(pair.Key, objectPosition, Quaternion.identity);
                    gameObject.AddComponent<NavMeshObstacle>().carving = true;

                    objectGrids[i, j] = gameObject;
                    allObjects.Add(gameObject);

                    //up the distance of the object so other objects dont spawn too close
                    j += distance;
                }
                else
                {
                    distance = 0;
                }
            }
            //up the distance of the object so other objects dont spawn too close
            i += distance;
        }
        
    }
    /*
     * return world seed
     */
    public List<float> getWorldSeed()
    {
        return worldSeed;
    }
   
    /*
     * set the world seed 
     */
    public void setWorldSeed(List<float> num)
    {
        biomeOffSet.x = num[0];
        biomeOffSet.y = num[1];

        tileOffSet.x = num[2];
        tileOffSet.y = num[3];

        objectOffSet.x = num[4];
        objectOffSet.y = num[5];

        riverOffSet.x = num[6];
        riverOffSet.y = num[7];

        townOffSet.x = num[8];
        townOffSet.y = num[9];

        for (int i = 0; i < rowColLen.x; i++)
        {
            for (int j = 0; j < rowColLen.y; j++)
            {
                //destroy all tiles, so that tiles that shouldnt exist are not leftover
                if (tileGrids[i, j] != null)
                {
                    Destroy(tileGrids[i, j]);
                }
            }
        }
        //destroy all objects
        foreach (GameObject g in allObjects)
        {
            Destroy(g);
        }
        allObjects.Clear();
        enemySpawn.deleteEnemies();
        townSpawn.destroyBuildings();

        Init();
    }
   
    /*
     * convert directions to vectors and vice versa
     */
    public Vector2 directionToIndex(directions dir)
    {
        return dirToIndex[dir];
    }
    public directions indexToDirection(Vector2 indx)
    {
        return indexToDir[indx];
    }
       
     
}

