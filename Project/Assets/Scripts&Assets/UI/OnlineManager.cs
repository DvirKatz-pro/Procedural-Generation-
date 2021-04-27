using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// OnlineManager
// Manages the online connection to the internet, photon servers, and manages joining and creating rooms
//
// Written by: Cal
public class OnlineManager : MonoBehaviourPunCallbacks
{
    #region Connect / Disconnect

    // Connect to the photon servers
    public void Connect()
    {
        Debug.Log("Attempting to connect...");
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "cae";
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // When connected to the internet
    public override void OnConnected()
    {
        Debug.Log("Connected to the internet.");
    }

    // When connected to the photon servers
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to photon server. On server: " + PhotonNetwork.CloudRegion);
    }

    // Disconnect
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        Debug.Log("Disconnecting.");
    }

    #endregion

    #region Hosting / Joining

    // Host a room
    public static void Host(string roomName, string nickname)
    {
        PhotonNetwork.NickName = nickname;
        Debug.Log("Host room. Room name: " + roomName + ". Nickname: " + PhotonNetwork.NickName);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        Debug.Log("Attempting to create room...");
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    // When the room is created
    public override void OnCreatedRoom()
    {
        Debug.Log("Room " + PhotonNetwork.CurrentRoom.Name + " has been created.");
    }

    // Join a room
    public static void Join(string roomName, string nickname)
    {
        PhotonNetwork.NickName = nickname;
        Debug.Log("Join room. Room name: " + roomName + ". Nickname: " + PhotonNetwork.NickName);

        PhotonNetwork.JoinRoom(roomName);
    }

    // When the room is joined
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined room " + PhotonNetwork.CurrentRoom.Name + ".");
        PhotonNetwork.LoadLevel("Lobby");
    }

    // When the room joining fails
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Error join room: " + returnCode + ". " + message);
    }

    #endregion
}
