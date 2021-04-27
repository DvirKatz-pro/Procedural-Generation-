using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

// LevelManagerMultiplayer
// Manages the level for multiplayer
//
// Written by: Cal
public class LevelManagerMultiplayer : MonoBehaviourPunCallbacks
{
    // Areas
    private int area;
    public GameObject[] areaTriggers = new GameObject[5];

    private WeaponManager weaponManager;
    private EnemyAIManagerMultiplayer enemyManager;

    // Start is called before the first frame update
    void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
        if (weaponManager == null)
            Debug.Log("Weapon Manage on " + this.name + " is null.");

        enemyManager = GetComponent<EnemyAIManagerMultiplayer>();
        if (enemyManager == null)
            Debug.Log("Enemy Manager on " + this.name + " is null.");

        area = 0;

        SpawnWeapons();
        SpawnEnemies();
    }

    // Spawn weapons
    private void SpawnWeapons()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            // Multiplayer Test Scene
            case "Multiplayer":
                // If online, and I am master client
                //     Spawn weapons
                if (!PhotonNetwork.OfflineMode && PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.InstantiateRoomObject(weaponManager.getRandomWeaponGameObject().name, new Vector3(8f, 0, 0), Quaternion.identity);
                    PhotonNetwork.InstantiateRoomObject(weaponManager.getRandomWeaponGameObject().name, new Vector3(-8f, 0, 0), Quaternion.identity);
                    PhotonNetwork.InstantiateRoomObject(weaponManager.getRandomWeaponGameObject().name, new Vector3(0, 0, 8f), Quaternion.identity);
                    PhotonNetwork.InstantiateRoomObject(weaponManager.getRandomWeaponGameObject().name, new Vector3(0, 0, -8f), Quaternion.identity);
                }
                break;

            default:
                Debug.Log("Level manager: Active scene " + SceneManager.GetActiveScene().name + " does not contain a case for spawning weapons.");
                break;
        }
    }

    // Spawn enemies
    private void SpawnEnemies()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            // Multiplayer Test Scene
            case "Multiplayer":
                // If online, and I am master client
                //      Tell enemy manager to spawn enemies
                if (!PhotonNetwork.OfflineMode && PhotonNetwork.IsMasterClient)
                {
                    enemyManager.SpawnEnemies("Multiplayer", area);
                }
                break;

            default:
                Debug.Log("Level manager: Active scene " + SceneManager.GetActiveScene().name + " does not contain a case for spawning enemies.");
                break;
        }

    }

    // Spawn a specific weapon
    public void SpawnWeapon(string weaponName, Vector3 spawnLocation)
    {
        // If online, and I am master client
        //      Spawn weapon for room
        if (!PhotonNetwork.OfflineMode && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateRoomObject(weaponManager.getWeaponGameObject(weaponName).name, new Vector3(spawnLocation.x, 0, spawnLocation.z), Quaternion.identity);
        }
        // If offline
        //      Spawn the weapon locally
        else if (PhotonNetwork.OfflineMode)
        {
            Instantiate(weaponManager.getWeaponGameObject(weaponName), new Vector3(spawnLocation.x, 0, spawnLocation.z), Quaternion.identity);
        }
    }

}
