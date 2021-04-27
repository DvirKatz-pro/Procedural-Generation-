using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

// LobbyManagers
// Manages the lobby
//
// Written by: Cal
public class LobbyManager : MonoBehaviourPunCallbacks
{
    // Game objects
    public GameObject startText;

    public GameObject[] playerListObjects = new GameObject[4];

    // Text
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI roomPlayerCountText;

    void Start()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        UpdateLobby();
    }

    private void UpdateLobby()
    {
        // Update 
        SetPlayerCount();
        if (PhotonNetwork.IsMasterClient)
            startText.SetActive(true);
        else
            startText.SetActive(false);
        SetPlayerList();
    }

    private void SetPlayerList()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (PhotonNetwork.PlayerList.Length > i)
            {
                GameObject currentPlayerList = playerListObjects[i];
                LobbyPlayerManager lobbyPlayerManager = currentPlayerList.GetComponent<LobbyPlayerManager>();
                if (lobbyPlayerManager == null)
                    Debug.LogError("Lobby player manage on " + currentPlayerList.name + " is null.");
                Photon.Realtime.Player currentPlayer = PhotonNetwork.PlayerList[i];
                lobbyPlayerManager.setPlayerName(currentPlayer.NickName);
                lobbyPlayerManager.setCrown(currentPlayer.IsMasterClient);
                currentPlayerList.SetActive(true);
            }
            else
            {
                playerListObjects[i].SetActive(false);
            }
        }
    }

    #region Photon

    // Set the player count
    private void SetPlayerCount()
    {
        roomPlayerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    // I leave the room
    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    // When I have left the room
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    // When I am the host and click start game
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Multiplayer");
        }
    }

    // When another player joins the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateLobby();
    }

    // When another player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobby();
    }

    #endregion
}
