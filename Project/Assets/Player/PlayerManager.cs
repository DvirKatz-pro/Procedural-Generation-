using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// PlayerManager
// Used to enable scripts on the client side of the owner
//
// Written by: Cal
public class PlayerManager : MonoBehaviourPunCallbacks
{
    #region PlayerSetup

    // Setup the player for single player
    public void SingleplayerSetup()
    {

    }

    // Set up the player for multiplayer
    public void MultiplayerSetup()
    {
        // Enable all the neccessary components

        this.GetComponent<PlayerMovement>().enabled = true;
        this.GetComponent<PlayerDash>().enabled = true;
        this.GetComponent<PlayerBlock>().enabled = true;

        this.GetComponent<CombatController>().enabled = true;
        this.GetComponent<PlayerBasicAttack>().enabled = true;

        this.GetComponent<PlayerStatus>().enabled = true;
        this.GetComponent<Inventory>().enabled = true;

        this.GetComponent<Grenade>().enabled = true;

        this.GetComponent<SpinAttack>().enabled = true;
        this.GetComponent<SplashAttack>().enabled = true;
        this.GetComponent<LaserAttack>().enabled = true;

        this.GetComponentInChildren<ItemPickup>().enabled = true;
    }

    #endregion
}