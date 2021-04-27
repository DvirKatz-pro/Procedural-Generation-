using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// LobbyPlayerManager
// Manages a single player UI card in the lobby
//
// Written by: Cal
public class LobbyPlayerManager : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public RawImage crown;

    public void setPlayerName(string name)
    {
        playerName.text = name;
    }

    public void setCrown(bool isCrown)
    {
        if (isCrown)
            crown.gameObject.SetActive(true);
        else
            crown.gameObject.SetActive(false);
    }
}
