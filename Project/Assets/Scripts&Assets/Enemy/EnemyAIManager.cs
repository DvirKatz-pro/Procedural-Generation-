using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy AI Manager
// Used to manage the enemy AI for the farm level
//
// Written by: Cal
public class EnemyAIManager : MonoBehaviour
{
    #region Variables

    // Player
    [SerializeField] private GameObject player;

    // Enemy Prefabs
    public GameObject pin;
    public GameObject punisher;

    // Enemy lists
    List<Enemy> closeEnemies = new List<Enemy>();
    List<Enemy> rangedEnemies = new List<Enemy>();

    // Enemy formation positions
    static readonly Vector3[] closeCoordinates = {
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(-0.70f, 0.0f, 0.70f),
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3(-0.70f, 0.0f, -0.70f),
        new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(0.70f, 0.0f, -0.70f),
        new Vector3(1.00f, 0.0f, 0.0f),
        new Vector3(0.70f, 0.0f, 0.70f)
    };
    static Enemy[] closePositions = new Enemy[8];
    static readonly Vector3[] rangedCoordinates = {
        new Vector3(-0.5f, 0.0f, 0.86f),
        new Vector3(-0.86f, 0.0f, 0.5f),
        new Vector3(-0.86f, 0.0f, -0.5f),
        new Vector3(-0.5f, 0.0f, -0.86f),
        new Vector3(0.5f, 0.0f, -0.86f),
        new Vector3(0.86f, 0.0f, -0.5f),
        new Vector3(0.86f, 0.0f, 0.5f),
        new Vector3(0.5f, 0.0f, 0.86f),
    };
    static Enemy[] rangedPositions = new Enemy[8];

    // Manager
    private LevelManager manager;
    Enemy closeRangeEnemyAttacking;
    public bool playerSpotted = false;

    #endregion

    #region Main

    // Start is called before the first frame update
    void Start()
    {
        manager = this.GetComponentInParent<LevelManager>();
        if (manager == null)
            Debug.LogError("LevelManager on " + this.name + " is null.");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerSpotted && closeRangeEnemyAttacking == null && closeEnemies.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, closeEnemies.Count);
            Enemy theChosenOne = closeEnemies[random];
            closeRangeEnemyAttacking = theChosenOne;
            theChosenOne.SetState(EnemyState.Attack);
        }
    }

    #endregion

    #region Player Functions

    // Called when a close range enemy would like to attack
    public void PlayerSpottedAlert(Enemy enemy)
    {
        playerSpotted = true;

        // Sort close enemies and give them formation spots
        closeEnemies.Sort(DistanceToPlayerSort);
        foreach (Enemy e in closeEnemies)
            GetFormationSpot(e, closeCoordinates, closePositions);

        // Sort ranged enemies and give them formation spots
        rangedEnemies.Sort(DistanceToPlayerSort);
        foreach (Enemy e in rangedEnemies)
            GetFormationSpot(e, rangedCoordinates, rangedPositions);

        if (enemy.GetEnemyType() == EnemyType.Close)
        {
            if (closeRangeEnemyAttacking == null)
            {
                enemy.SetState(EnemyState.Attack);
                closeRangeEnemyAttacking = enemy;
            }
        }
    }

    // Used to .sort a list of enemies based on their distance to the player, descending
    private int DistanceToPlayerSort(Enemy x, Enemy y)
    {
        float xDistance = Vector3.Distance(x.transform.position, player.transform.position);
        float yDistance = Vector3.Distance(y.transform.position, player.transform.position);

        if (xDistance == yDistance)
        {
            return 0;
        }
        else if (xDistance > yDistance)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }

    #endregion

    #region EnemyAI

    // Get the formation spot for an enemy
    private void GetFormationSpot(Enemy e, Vector3[] coordinates, Enemy[] enemyPositions)
    {
        int closestX = 0;
        float closestDistance = 10000;

        for (int x = 0; x < coordinates.Length; x++)
        {
            if (enemyPositions[x] == null)
            {
                float distanceToSpot = Vector3.Distance(e.transform.position, player.transform.position + coordinates[x]);
                if (distanceToSpot < closestDistance)
                {
                    closestX = x;
                    closestDistance = distanceToSpot;
                }
            }
        }
        enemyPositions[closestX] = e;
        e.SetState(EnemyState.Formation);
        e.SetFormationPosition(coordinates[closestX]);
    }

    // Spawn the enemies based on combat area
    public void SpawnEnemies(int area)
    {
        switch (area)
        {
            case 0:
                Instantiate(pin, new Vector3(2, 0.1f, 45), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                Instantiate(pin, new Vector3(-10, 0.1f, 45), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                break;

            case 1:
                Instantiate(pin, new Vector3(4, 0.1f, 101), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                Instantiate(pin, new Vector3(-3, 0.1f, 106), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                break;

            case 2:
                Instantiate(pin, new Vector3(15, 0.1f, 161), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                Instantiate(pin, new Vector3(17, 0.1f, 150), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                Instantiate(punisher, new Vector3(24, 0.1f, 154), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                break;

            case 3:
                Instantiate(pin, new Vector3(40, 0.1f, 224), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                Instantiate(pin, new Vector3(20, 0.1f, 216), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                Instantiate(punisher, new Vector3(20, 0.1f, 227), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                Instantiate(punisher, new Vector3(30, 0.1f, 234), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                break;

            case 4:
                Instantiate(pin, new Vector3(-12, 0.1f, 291), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                Instantiate(pin, new Vector3(-5, 0.1f, 285), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                Instantiate(pin, new Vector3(-13, 0.1f, 288), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                Instantiate(punisher, new Vector3(-10, 0.1f, 296), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                Instantiate(punisher, new Vector3(0, 0.1f, 289), Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
                break;

            default:
                Debug.LogError("AI Manager has invalid area.");
                break;
        }
    }

    // Clear the lists of enemies
    public void ClearLists()
    {
        closeEnemies.Clear();
        rangedEnemies.Clear();
    }

    #endregion

    #region Register / Unregister

    // Allows an enemy to register themselves with the AI Manager
    public void Register(Enemy enemyToRegister, EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Close:
                closeEnemies.Add(enemyToRegister);
                break;

            case EnemyType.Ranged:
                rangedEnemies.Add(enemyToRegister);
                break;

        }
    }

    // Allows an enemy to unregister themselves with the AI Manager
    public void Unregister(Enemy enemyToUnregister, EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Close:
                if (enemyToUnregister == closeRangeEnemyAttacking)
                    closeRangeEnemyAttacking = null;
                closeEnemies.Remove(enemyToUnregister);
                break;

            case EnemyType.Ranged:
                rangedEnemies.Remove(enemyToUnregister);
                break;
        }
        if (closeEnemies.Count == 0 && rangedEnemies.Count == 0)
        {
            playerSpotted = false;
            manager.AreaCleared();
        }
    }

    #endregion
}
