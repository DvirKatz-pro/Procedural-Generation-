using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

// Enemy AI Manager
// Used to manage the enemy AI for multiplayer
//
// Written by: Cal
public class EnemyAIManagerMultiplayer : MonoBehaviour
{
    #region Variables

    // Enemies
    public GameObject pinEnemy;
    public GameObject punisherEnemy;

    // Enemy lists
    List<EnemyMultiplayer> closeEnemies = new List<EnemyMultiplayer>();
    List<EnemyMultiplayer> rangedEnemies = new List<EnemyMultiplayer>();

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
    private List<int> playersSpotted = new List<int>();

    #endregion

    #region Main

    // Start AI coroutine
    private void Awake()
    {
        StartCoroutine(EnemyAI());
    }

    #endregion

    // Multiplayer Test Scene
    public void SpawnEnemies(string scene, int area)
    {
        switch (scene)
        {
            // Multiplayer scene
            case "Multiplayer":
                switch (area)
                {
                    // Area 0
                    case 0:
                        PhotonNetwork.InstantiateRoomObject(pinEnemy.name, new Vector3(0, 0, 18f), Quaternion.identity);
                        PhotonNetwork.InstantiateRoomObject(pinEnemy.name, new Vector3(0, 0, -18f), Quaternion.identity);
                        break;
                }
                break;
        }
    }

    #region Player Functions

    // Alerts of a spotted player, and adds them to the list
    public void PlayerSpottedAlert(int viewID)
    {
        if (!playersSpotted.Contains(viewID))
        {
            Debug.Log("Added " + viewID + " to playersSpotted.");
            playersSpotted.Add(viewID);
        }
    }

    // Removes a player from the spotted list
    public void RemoveSpottedPlayer(int viewID)
    {
        playersSpotted.Remove(viewID);
    }

    // Returns the list of spotted players
    public List<int> GetPlayersSpotted()
    {
        return playersSpotted;
    }

    #endregion

    #region EnemyAI
    private IEnumerator EnemyAI()
    {
        // If I am the master client
        if (PhotonNetwork.IsMasterClient)
        {
            // If we have at least one player spotted
            if (playersSpotted.Count > 0)
            {
                // If we have close ranged enemies
                if (closeEnemies.Count > 0)
                {
                    // Temp lists of players and enemies
                    List<int> tempPlayersSpotted = new List<int>();
                    foreach (int player in playersSpotted)
                        tempPlayersSpotted.Add(player);
                    List<EnemyMultiplayer> tempCloseEnemies = new List<EnemyMultiplayer>();
                    foreach (EnemyMultiplayer enemy in closeEnemies)
                        tempCloseEnemies.Add(enemy);

                    // Update the lists
                    while (tempCloseEnemies.Count > 0)
                    {
                        // The enemy who has the closest player
                        EnemyMultiplayer closestEnemy = null;
                        float closestEnemyDistance = 1000000;
                        int playerToAttack = 0;

                        // Go through each enemy left without a player to attack
                        foreach (EnemyMultiplayer e in tempCloseEnemies)
                        {
                            // If we've gone through all the players, refill the list
                            if (tempPlayersSpotted.Count == 0)
                            {
                                foreach (int player in playersSpotted)
                                    tempPlayersSpotted.Add(player);
                            }

                            // Closest player to this specific enemy
                            int closestPlayerViewID = 0;
                            float closestPlayerDistance = 1000000;

                            // Go through players in temp list and find closest one
                            foreach (int playerViewID in tempPlayersSpotted)
                            {
                                // If this player is closer than the closest player, set them as closest player
                                float distanceToPlayer = e.DistanceToPlayer(playerViewID);
                                if (distanceToPlayer < closestPlayerDistance)
                                {
                                    closestPlayerViewID = playerViewID;
                                    closestPlayerDistance = distanceToPlayer;
                                }
                            }

                            // Make sure we have a closest player
                            if (closestPlayerViewID == 0)
                            {
                                Debug.LogError("We don't have a closest player for this enemy to attack!");
                                yield return new WaitForSeconds(2f);
                                StartCoroutine(EnemyAI());
                                yield break;
                            }
                            // If the closest player for this enemy is closer than the closest player for other enemies, set this as the enemy with the closest closest player
                            else if (closestPlayerDistance < closestEnemyDistance)
                            {
                                closestEnemy = e;
                                closestEnemyDistance = closestPlayerDistance;
                                playerToAttack = closestPlayerViewID;
                                tempPlayersSpotted.Remove(closestPlayerViewID);
                            }
                        }

                        // Make sure we have a closest player
                        if (playerToAttack == 0)
                        {
                            Debug.LogError("We don't have an overall closest player for this enemy to attack!");
                            yield return new WaitForSeconds(2f);
                            StartCoroutine(EnemyAI());
                            yield break;
                        }
                        else
                        {
                            // Set this player as the player for the enemy to attack, and remove the enemy from our list to check
                            closestEnemy.SetFocusPlayer(playerToAttack);
                            tempCloseEnemies.Remove(closestEnemy);
                            Debug.Log("Enemy has been sent after player " + playerToAttack);
                        }
                    }
                }
            }
        }

        // Wait 2 seconds before calling again
        yield return new WaitForSeconds(2f);
        StartCoroutine(EnemyAI());
    }

    #endregion

    #region Register / Unregister

    // Allows an enemy to register with the AI Manager
    public void Register(EnemyMultiplayer enemyToRegister, EnemyMType enemyType)
    {
        switch (enemyType)
        {
            case EnemyMType.Close:
                closeEnemies.Add(enemyToRegister);
                break;

            case EnemyMType.Ranged:
                rangedEnemies.Add(enemyToRegister);
                break;

        }
    }

    // Allows an enemy to unregister with the AI Manager
    public void Unregister(EnemyMultiplayer enemyToUnregister, EnemyMType enemyType)
    {
        switch (enemyType)
        {
            case EnemyMType.Close:
                closeEnemies.Remove(enemyToUnregister);
                break;

            case EnemyMType.Ranged:
                rangedEnemies.Remove(enemyToUnregister);
                break;
        }
        if (closeEnemies.Count == 0 && rangedEnemies.Count == 0)
        {
            playersSpotted.Clear();
            //levelmanager.AreaCleared();
        }
    }

    #endregion

    #region RPC

    // RPC to destory an enemy
    [PunRPC]
    void DestroyEnemy(int viewID)
    {
        if (PhotonNetwork.GetPhotonView(viewID).IsMine && PhotonNetwork.GetPhotonView(viewID).gameObject != null)
        {
            PhotonNetwork.Destroy(PhotonNetwork.GetPhotonView(viewID).gameObject);
        }
    }

    // RPC when a player dies
    [PunRPC]
    void PlayerDiedRPC(int viewID)
    {
        // I am the master client
        //      Remove player from players spotted list
        if (PhotonNetwork.IsMasterClient)
        {
            RemoveSpottedPlayer(viewID);
        }
    }

    #endregion
}
