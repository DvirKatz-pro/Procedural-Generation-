using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private int possibleAmount;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject player;
    private List<GameObject> enemyList = new List<GameObject>();
    public void reGenerateEnemies(Vector3 m_tilePositions, float[,] m_noiseMap)
    {
       
       
        //create a 2x2 array of objects     
        GameObject[,] objectGrids = new GameObject[25*3, 25*3];
        int MaxCount = 5;
        Vector3 southWestPosition = m_tilePositions;
        southWestPosition.x += 1;
        southWestPosition.z += 1;
        Vector3 objectPosition = southWestPosition;
       

        for (int i = 0; i < objectGrids.GetUpperBound(0); i+=2)
        {
            for (int j = 0; j < objectGrids.GetUpperBound(1); j+=2)
            {
                //generate the noise value for this object
                float noiseValue = m_noiseMap[i, j];
                objectPosition.z += j;
                objectPosition.x += i;
                // get the object, size of it
                if (noiseValue >= 0.85f && Vector3.Distance(objectPosition,player.transform.position) > 30)
                {
                    if (enemyList.Count < MaxCount)
                    {
                        RaycastHit hit;
                        Ray ray = new Ray(objectPosition + Vector3.up * 100, Vector3.down);
                        LayerMask mask = LayerMask.GetMask("Terrain");
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                        {
                            if (hit.collider != null)
                            {
                                objectPosition = new Vector3(objectPosition.x, hit.point.y, objectPosition.z);
                                GameObject enemyObject = Instantiate(enemy, objectPosition, Quaternion.identity);
                                objectPosition.x += 3;
                                objectPosition.z += 3;
                                enemyList.Add(enemyObject);
                            }
                            else
                            {
                                Debug.Log("No Ground");
                            }
                        }
     
                    }
                }
                objectPosition.z = southWestPosition.z;
                objectPosition.x = southWestPosition.x;

            }
            



        }

    }
    public void removeEnemy(GameObject enemy)
    {
        enemyList.Remove(enemy);
    }
    public void deleteEnemies()
    {
        for (int i = enemyList.Count-1; i >= 0; i--)
        {
            GameObject player = GameObject.Find("Player");
            if (Vector3.Distance(enemyList[i].transform.position, player.transform.position) > 35)
            {
                GameObject enemy = enemyList[i];
                enemyList.RemoveAt(i);
                Destroy(enemy);
            }
        }
    }
}
