using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

// OnlineButtonManager
// Manages the buttons related to online
//
// Written by: Cal
public class OnlineButtonManager : MonoBehaviourPunCallbacks
{

    // Variables
    public TextMeshProUGUI roomNameHostInput;
    public TextMeshProUGUI nicknameHostInput;

    public TextMeshProUGUI roomNameJoinInput;
    public TextMeshProUGUI nicknameJoinInput;

    // Click
    public void Click()
    {
        switch (this.name)
        {
            case "Text Create":
                OnlineManager.Host(roomNameHostInput.text, nicknameHostInput.text);
                break;

            case "Text Join":
                OnlineManager.Join(roomNameJoinInput.text, nicknameJoinInput.text);
                break;
        }
    }
}
