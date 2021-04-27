using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//script to generate a small town - Dvir
public class TownSpawn : MonoBehaviour
{
    [SerializeField] private List<GameObject> buildings = new List<GameObject>();
    [SerializeField] private List<GameObject> accesories = new List<GameObject>();
    [SerializeField] private GameObject road;
    [SerializeField] private GameObject junction;
    private List<GameObject> allObjects = new List<GameObject>();

    NoiseGenerator noise;
    private float size = 0;
    // Start is called before the first frame update
    void Start()
    {
        noise = GetComponent<NoiseGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool spawnBuildings(Tile tile,GameObject tileTrigger ,float value)
    {
        //check if we are initialized
        if (noise == null)
        {
            noise = GetComponent<NoiseGenerator>();
        }
        if (size == 0)
        {
            float sizeX = tileTrigger.GetComponent<Collider>().bounds.size.x;
            float sizeZ = tileTrigger.GetComponent<Collider>().bounds.size.z;

            size = (sizeX + sizeZ) / 2.0f;
        }
        //check with noisemap if we can spawn building
        if (value <= 0.3f)
        {
            //calculate the building position
            Vector3 pos = tileTrigger.transform.position;

            pos.x += size / 2.0f - 8.0f;
            pos.z += size / 2.0f - 8.0f;

            int type = (int)(value * 1000);
            type = type % buildings.Count;
            GameObject g = Instantiate(buildings[type], pos, Quaternion.identity);

            //scale the building up or down
            Vector3 scale = g.GetComponent<Collider>().bounds.size;
            float correctSizeX = 15 / scale.x;
           float correctSizeZ = 15 / scale.z;
            Vector3 scaleVector = g.transform.localScale;
            scaleVector.x *= correctSizeX;
            scaleVector.z *= correctSizeZ;

            g.transform.localScale = scaleVector;

            g.gameObject.tag = "Building";
            addObstecule(g);

            //instantiate default tile under the building
            GameObject child = tileTrigger.transform.GetChild(0).gameObject;
            Vector3 childPos = child.transform.position;
            Quaternion childRot = child.transform.rotation;
            Destroy(child);
            child = Instantiate(tile.getBiome().getDefaultTile(), childPos, childRot);
            child.GetComponent<MeshRenderer>().material = tile.getBiome().getDefaultMaterial();
            
            child.layer = LayerMask.NameToLayer("Terrain");
            
            allObjects.Add(child);
            
            


            allObjects.Add(g);
            spawnRoad(tileTrigger);
            spawnAccesories(tileTrigger, value);
            return false;
        }
        return true;
    }
    /*
     * spawn roads for a building
     */
    private void spawnRoad(GameObject tileTrigger)
    {
        
        //calculate road positions and junction positions

        Vector3 pos1 = tileTrigger.transform.position;
        Vector3 pos2 = tileTrigger.transform.position;
        Vector3 posJu = tileTrigger.transform.position;

        pos1.x += (size / 2.0f);
        pos1.z += (size / 2.0f) - 5.0f;
        pos1.y += 0.2f;

        pos2.x += (size / 2.0f) - 5.0f;
        pos2.z += (size / 2.0f) - 5.0f;
        pos2.y += 0.2f;

        posJu.x += (size / 2.0f);
        posJu.z += (size / 2.0f) - 5.0f;
        posJu.y += 0.2f;

        GameObject road1 = Instantiate(road, pos1, Quaternion.identity);
        road1.transform.localScale = new Vector3(1, 1, 4f);

        GameObject road2 = Instantiate(road, pos2, Quaternion.Euler(0,90,0));
        road2.transform.localScale = new Vector3(1, 1, 4);

        GameObject Ju = Instantiate(junction, posJu, Quaternion.Euler(0, 90, 0));

        posJu = tileTrigger.transform.position;
        posJu.x -= (size / 2.0f) + 0.8f;
        posJu.z += (size / 2.0f) - 5.0f;
        posJu.y += 0.2f;
        
        GameObject Ju2 = Instantiate(junction, posJu, Quaternion.Euler(0, 90, 0));

        allObjects.Add(road1);
        allObjects.Add(road2);
        allObjects.Add(Ju);
        allObjects.Add(Ju2);
    }
    /*
     * spawn accesories for the roads
     */
    private void spawnAccesories(GameObject tileTrigger,float value)
    {
        //for 4 postions for each 2 roads in a grid, spawn an accesory every so often
        int type = (int)(value * 10000);
        int type1 = type % accesories.Count;
        int type2 = (type + 1) % accesories.Count;


        Vector3 pos = tileTrigger.transform.position;
        Vector3 initialX = pos;


        initialX.x = initialX.x - size / 2.0f;
        initialX.z = initialX.z + size / 2.0f;

        Vector3 initialZ = pos;
        initialZ.x = initialZ.x + size / 2.0f;
        initialZ.z = initialZ.z + size / 2.0f - 8.0f;

        GameObject one = Instantiate(accesories[type1], initialX, Quaternion.identity);
        GameObject two = Instantiate(accesories[type2], initialZ, Quaternion.Euler(0, -90, 0));
  
        addObstecule(one);
        addObstecule(two);

        allObjects.Add(one);
        allObjects.Add(two);
        bool alternate = false;
        Vector3 rot = Vector3.zero;
        for (int i = 0; i < 3; i++)
        {

            initialX.x += 3;
            if (alternate)
            {
                initialX.z += 5;
                alternate = false;
                rot = new Vector3(0, 180, 0);
            }
            else
            {
                alternate = true;
                initialX.z -= 5;
                rot = new Vector3(0, 0, 0);
            }

            type1 = (type + i) * 7 % accesories.Count;
            one = Instantiate(accesories[type1], initialX, Quaternion.Euler(rot));
            addObstecule(one);
            
            allObjects.Add(one);
        }
        alternate = false;
        for (int i = 0; i < 3; i++)
        {
            initialX.z += 3;
            if (alternate)
            {
                initialZ.x += 5;
                alternate = false;
                rot = new Vector3(0, 90, 0);
            }
            else
            {
                initialZ.x -= 5;
                alternate = true;
                rot = new Vector3(0, -90, 0);
            }

            type2 = (type + i + 1) * 7 % accesories.Count;
            two = Instantiate(accesories[type2], initialZ, Quaternion.Euler(rot));
            addObstecule(two);
            allObjects.Add(two);
        }

    }
    public void destroyBuildings()
    {
        foreach (GameObject g in allObjects)
        {
            Destroy(g);
        }
        allObjects.Clear();
    }
    private void addObstecule(GameObject obstecule)
    {
        if (obstecule.GetComponent<NavMeshObstacle>() == null)
        {
            obstecule.AddComponent<NavMeshObstacle>().carving = true;
        }
    }
}
