using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ItemPickup
// Manages the picking up of an item
//
// Written by: Cal & Dvir
public class ItemPickup : MonoBehaviourPunCallbacks
{
    // Variables
    private Inventory inventory;
    private GameManager gameManager;

    // Set variables
    void Start()
    {
        inventory = this.GetComponentInParent<Inventory>();
        if (inventory == null)
            Debug.Log("Inventory on " + this.name + " is null.");

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager == null)
            Debug.Log("Game Manager on " + this.name + " is null.");
    }

    // When we enter the collider of a trigger
    private void OnTriggerEnter(Collider other)
    {
        // If the trigger was for a pickup
        if (other.gameObject.tag == "Pickup")
        {
            // Get the pickup manager of the item so we can see the details
            PickupManager pickupManager = other.GetComponent<PickupManager>();
            if (pickupManager == null)
                Debug.LogError("Pickup Manager on " + other.name + " is null.");
            else
            {
                // If offline
                if (PhotonNetwork.OfflineMode)
                {
                    // Pickup the item by calling appropriate method
                    switch (pickupManager.pickup)
                    {
                        case PickupManager.pickupType.Weapon:
                            inventory.addWeapon(pickupManager.pickupName, pickupManager.durability);
                            Destroy(other.gameObject);
                            break;

                        case PickupManager.pickupType.Scrap:
                            inventory.addScrap();
                            Destroy(other.gameObject);
                            break;

                        default:
                            break;
                    }
                }
                // Therefore we are online, and if it hasnt been marked as picked up
                else if (!pickupManager.pickedUp)
                {
                    // Mark it as picked up
                    pickupManager.pickedUp = true;

                    // Call the appropriate pickup RPC
                    switch (pickupManager.pickup)
                    {
                        case PickupManager.pickupType.Weapon:
                            gameManager.GetPlayer().GetComponent<PhotonView>().RPC("PickupWeaponRPC", RpcTarget.AllViaServer, gameManager.GetPlayer().GetComponent<PhotonView>().ViewID, pickupManager.pickupName, pickupManager.durability, other.GetComponent<PhotonView>().ViewID);
                            break;

                        case PickupManager.pickupType.Scrap:
                            gameManager.GetPlayer().GetComponent<PhotonView>().RPC("PickupScrapRPC", RpcTarget.AllViaServer, gameManager.GetPlayer().GetComponent<PhotonView>().ViewID, pickupManager.pickupName, other.GetComponent<PhotonView>().ViewID);
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}