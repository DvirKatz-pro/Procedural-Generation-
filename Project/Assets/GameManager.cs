using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

// GameManager
// Used to manage the overall game and initialize the level
//
// Written by: Cal
public class GameManager : MonoBehaviourPunCallbacks
{
    #region Variables

    public enum GameType { Singleplayer, Multiplayer };
    [SerializeField] private GameType gameType;

    [SerializeField] private GameObject multiplayerPlayerPrefab;
    [SerializeField] private GameObject gameUI;

    private GameObject myPlayer;
    private PhotonView myPhotonView;

    public List<int> playerPhotonViewIDs = new List<int>();

    #endregion

    #region Main

    void Awake()
    {
        // Multiplayer
        if (gameType == GameType.Multiplayer)
        {
            // Instantiate player
            GameObject player = PhotonNetwork.Instantiate(multiplayerPlayerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);

            // Setup my player
            if (player.GetPhotonView().IsMine)
            {
                myPlayer = player;
                myPhotonView = player.GetPhotonView();
                playerPhotonViewIDs.Add(myPhotonView.ViewID);
                this.gameObject.GetComponent<PhotonView>().RPC("AddPlayerID", RpcTarget.OthersBuffered, myPhotonView.ViewID);

                // Set up my player
                PlayerManager playerManager = player.GetComponent<PlayerManager>();
                if (playerManager == null)
                    Debug.LogError("PlayerManager on " + this.name + " is null.");
                playerManager.MultiplayerSetup();

                // Set my camera in the scene to follow me
                CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
                if (camFollow == null)
                    Debug.LogError("CamFollow on " + this.name + " is null.");
                camFollow.SetTarget(player.transform);

                // Set up game UI for me
                UIManager uiManager = gameUI.GetComponent<UIManager>();
                uiManager.SetPlayer(player);
            }
        }
        // Singleplayer
        else
        {
            myPlayer = GameObject.Find("Player");
            UIManager uiManager = gameUI.GetComponent<UIManager>();
            uiManager.SetPlayer(myPlayer);
            PhotonNetwork.OfflineMode = true;
            Debug.Log("Photon network set to offline mode.");
        }
    }

    // Get the player
    public GameObject GetPlayer()
    {
        return myPlayer;
    }

    #endregion

    #region Photon

    // Leave the room
    public void Leave()
    {
        PhotonNetwork.Destroy(myPlayer.GetComponent<PhotonView>());
        PhotonNetwork.LeaveRoom();
    }

    // Reload the level
    public void ReloadLevel()
    {
        PhotonNetwork.DestroyAll();
        PhotonNetwork.LoadLevel("Lobby");
    }

    // When I join the room
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " has joined " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // When other player has joined the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " has entered room " + PhotonNetwork.CurrentRoom.Name + ". There is now " + PhotonNetwork.CurrentRoom.PlayerCount + " players.");
    }

    // When I leave the room
    public override void OnLeftRoom()
    {
        this.gameObject.GetComponent<PhotonView>().RPC("RemovePlayerID", RpcTarget.OthersBuffered, myPlayer.GetPhotonView().ViewID);
        SceneManager.LoadScene(0);
    }

    #endregion

    #region RPCs

    [PunRPC]
    void AddPlayerID(int viewID)
    {
        playerPhotonViewIDs.Add(viewID);
        playerPhotonViewIDs.ToString();
    }

    [PunRPC]
    void RemovePlayerID(int viewID)
    {
        playerPhotonViewIDs.Remove(viewID);
        playerPhotonViewIDs.ToString();
    }

    #endregion
}
