using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// PlayerWeaponManager
// Manages which weapon model the player is holding
//
// Written by: Cal
public class PlayerWeaponManager : MonoBehaviourPunCallbacks
{
    #region Variables

    public GameObject sword;
    public GameObject axe;
    public GameObject crowbar;
    public GameObject bat;
    private string currentWeapon;

    #endregion

    #region Player Weapon

    public void SetPlayerWeapon(string weaponToSet)
    {
        if (!string.Equals(weaponToSet, currentWeapon))
        {
            DisableOldPlayerWeapon();
            ActivateNewPlayerWeapon(weaponToSet);
            if (!PhotonNetwork.OfflineMode)
                this.gameObject.GetComponent<PhotonView>().RPC("SetPlayerWeaponRPC", RpcTarget.OthersBuffered, weaponToSet);
        }
    }

    public void ActivateNewPlayerWeapon(string weaponToSet)
    {
        switch (weaponToSet)
        {
            case "Sword":
                sword.SetActive(true);
                currentWeapon = "Sword";
                break;

            case "Axe":
                currentWeapon = "Axe";
                axe.SetActive(true);
                break;

            case "Crowbar":
                currentWeapon = "Crowbar";
                crowbar.SetActive(true);
                break;

            case "Bat":
                currentWeapon = "Bat";
                bat.SetActive(true);
                break;

            default:
                break;
        }
    }

    public void DisableOldPlayerWeapon()
    {
        switch (currentWeapon)
        {
            case "Sword":
                sword.SetActive(false);
                break;

            case "Axe":
                axe.SetActive(false);
                break;

            case "Crowbar":
                crowbar.SetActive(false);
                break;

            case "Bat":
                bat.SetActive(false);
                break;

            default:
                break;
        }
    }

    public string GetCurrentPlayerWeapon()
    {
        return currentWeapon;
    }

    public void DisableWeapon()
    {
        DisableOldPlayerWeapon();
    }

    public void EnableWeapon()
    {
        ActivateNewPlayerWeapon(currentWeapon);
    }

    #endregion

    #region RPC

    // Set the players weapon RPC
    [PunRPC]
    void SetPlayerWeaponRPC(string weaponToSet)
    {
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            DisableOldPlayerWeapon();
            ActivateNewPlayerWeapon(weaponToSet);
        }
    }

    #endregion
}
