using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LevelManager
// Manages the farm level
//
// Written by: Cal
public class LevelManager : MonoBehaviour
{
    #region Variables

    // Areas
    private int area;
    public GameObject[] areaTriggers = new GameObject[5];

    // Player
    public GameObject player;
    private PlayerStatus playerStatus;
    private Inventory inventory;
    [SerializeField] Camera mainCamera;

    // Boss
    public GameObject boss;
    private bool bossBattle;
    private FirstBossEnemy bossScript;
    private Reinforcement reinforcement;

    // Enemy AI Manager
    private EnemyAIManager enemyAIManager;

    #endregion

    #region Main

    // Start is called before the first frame update
    void Start()
    {
        enemyAIManager = this.GetComponentInChildren<EnemyAIManager>();
        if (enemyAIManager == null)
            Debug.LogError("EnemyAIManager on " + this.name + " is null.");

        playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus == null)
            Debug.LogError("PlayerStatus on " + this.name + " is null.");

        inventory = player.GetComponent<Inventory>();
        if (inventory == null)
            Debug.LogError("Inventory on " + this.name + " is null.");

        bossScript = boss.GetComponent<FirstBossEnemy>();
        if (bossScript == null)
            Debug.LogError("BossScript on " + this.name + " is null.");

        reinforcement = boss.GetComponent<Reinforcement>();
        if (bossScript == null)
            Debug.LogError("reinforcement on " + this.name + " is null.");
        area = 0;
        enemyAIManager.SpawnEnemies(area);

    }

    #endregion

    #region Areas

    // Called when an area is cleared of enemies
    public void AreaCleared()
    {
        if (area <= 4)
        {
            // Set the trigger
            AreaTrigger trigger = areaTriggers[area].GetComponent<AreaTrigger>();
            trigger.setAsTrigger();
        }
        else
        {
            reinforcement.setEnemyCount(0);
        }

    }

    // Called when moving through triggers, advances the level
    public void IncrementArea()
    {
        area++;

        // Heal the player
        playerStatus.setGainHealth(true);

        // Save values
        PlayerPrefs.SetInt("Area1", area);
        PlayerPrefs.SetFloat("AP1", playerStatus.getAp());
        PlayerPrefs.SetInt("Scrap1", inventory.GetScrap());

        // Spawn enemies
        if (area < areaTriggers.Length)
        {
            enemyAIManager.SpawnEnemies(area);
        }
    }

    // Initialize the boss fight
    public void InitializeBoss()
    {
        bossScript.StartBoss();
        Debug.Log("Boss fight initiated");
    }

    #endregion

    #region Checkpoint

    // Load the checkpoint
    // NOT COMPLETE
    public void LoadCheckpoint()
    {
        /*
        enemyAIManager.ClearLists();
        playerStatus.Revive();
        player.transform.position = PlayerCheckpointPosition();
        mainCamera.transform.position = new Vector3(player.transform.position.x - 28, 25, player.transform.position.z - 28);
        playerStatus.setGainHealth(true);
        playerStatus.changeAp(PlayerPrefs.GetFloat("AP1"));
        inventory.setScrap(PlayerPrefs.GetInt("Scrap1"));
        */
    }

    // Player checkpoint positions
    public Vector3 PlayerCheckpointPosition()
    {
        switch (area)
        {
            case 0:
                return new Vector3(10, 0.5f, 0);

            case 1:
                return new Vector3(-42, 0.5f, -147);

            case 2:
                return new Vector3(-42, 0.5f, -95);

            case 3:
                return new Vector3(-10, 0.5f, -49);

            case 4:
                return new Vector3(-40, 0.5f, 17);

            case 5:
                return new Vector3(-35, 0.5f, 62);

            default:
                return new Vector3(10, 0.5f, 0);
        }
    }

    #endregion
}
