using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script that corresponds to a tiles "Trigger" collider, telling the terrain generator which tile the player stepped on - Dvir
public class GridTrigger : MonoBehaviour
{
    //if the player has stepped on this tile, change the buffer based on this tiles position

    private TerrainGeneration terrain;
    private TerrainGeneration.directions position;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
           terrain.buffer(position);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setTerrainGenerator(TerrainGeneration m_terrain)
    {
        terrain = m_terrain;
    }
    public void resetTerrain()
    {
        terrain.buffer(position);
    }
    public void setPosition(TerrainGeneration.directions m_position)
    {
        position = m_position;
    }

}
