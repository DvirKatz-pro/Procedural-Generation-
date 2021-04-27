using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Inventory
// Manages the weapons of the player, has support for both singleplayer and multiplayer
//
// Written by: Cal
public class Inventory : MonoBehaviourPunCallbacks
{
    #region Values
    protected Weapon currentWeapon;
    private int maxWeapons = 4;
    protected List<Weapon> weapons = new List<Weapon>();
    private bool inventoryFull;
    private UIElements uiElements;
    private InventoryUIManager inventoryUIManager;
    private GameManager gameManager;
    private WeaponManager weaponManager;
    private PlayerStatus playerStatus;
    [SerializeField] private PlayerWeaponManager playerWeaponManager;

    private int currentScrap;

    #endregion

    #region Main

    // Start is called before the first frame update
    void Start()
    {
        // Get UI elements
        uiElements = GameObject.Find("GameCanvas").GetComponent<UIElements>();
        if (uiElements == null)
            Debug.LogError("UI Elements on " + this.name + " is null.");

        // Get inventory UI manager
        inventoryUIManager = uiElements.inventory.GetComponent<InventoryUIManager>();
        if (inventoryUIManager == null)
            Debug.LogError("Inventory UI manager on " + this.name + " is null.");

        // Get game manager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager == null)
            Debug.LogError("Game manager on " + this.name + " is null.");

        // Get weapon manager
        weaponManager = gameManager.GetComponent<WeaponManager>();
        if (weaponManager == null)
            Debug.LogError("Weapon manager on " + this.name + " is null.");

        // Get player status
        playerStatus = this.gameObject.GetComponent<PlayerStatus>();


        // Set starting values
        currentScrap = 0;
        weapons.Capacity = maxWeapons;
        inventoryFull = false;
        addWeapon("Bat", 100);
    }

    #endregion

    #region Inventory

    // Check to see if we have room to pickup a weapon
    public bool canPickupWeapon()
    {
        return inventoryFull;
    }

    // Used for demonstatings weapon durability and damage, press L to lower durability
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            currentWeapon.AffectDurability(5);
            inventoryUIManager.UpdateCurrentWeaponUI();
        }
    }
    */

    // Adds a weapon to the inventory
    public void addWeapon(string weaponName, int durability)
    {
        // Check to make sure we have room to add weapon
        if (!inventoryFull)
        {
            // Get the weapon to add
            Weapon newWeapon = weaponManager.getWeaponScript(weaponName);
            newWeapon.InstantiateWeapon(inventoryUIManager, durability);
            if (newWeapon != null)
            {
                if (currentWeapon == null)
                {
                    currentWeapon = newWeapon;
                    playerWeaponManager.SetPlayerWeapon(currentWeapon.GetWeaponName());
                }
                else
                {
                    weapons.Add(newWeapon);
                    if (weapons.Count == 4)
                        inventoryFull = true;
                }
                inventoryUIManager.RenderInventoryUI();
            }

        }
    }

    public void SwapWeapons(int weaponSlot)
    {
        if (weaponSlot >= 1 && weaponSlot <= 4)
        {
            Weapon oldCurrentWeapon = currentWeapon;
            currentWeapon = weapons[weaponSlot - 1];
            weapons.RemoveAt(weaponSlot - 1);
            weapons.Add(oldCurrentWeapon);
            playerWeaponManager.SetPlayerWeapon(currentWeapon.GetWeaponName());
            inventoryUIManager.RenderInventoryUI();
        }
    }

    public void addScrap()
    {
        currentScrap += 50 > 100 ? currentScrap = 100 : currentScrap += 50;
        playerStatus.changeScrap(currentScrap);
    }

    public void setScrap(int newAmount)
    {
        currentScrap = newAmount;
        playerStatus.changeScrap(currentScrap);
    }

    public void removeWeapon(int index)
    {
        weapons.RemoveAt(index);
        inventoryFull = false;
        inventoryUIManager.RenderInventoryUI();
    }

    #endregion

    #region Get Values

    // Get weapon damage
    public int GetWeaponDamage()
    {
        return currentWeapon.GetWeaponDamage();
    }

    // Get current weapon
    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    // Get all weapons
    public List<Weapon> GetWeapons()
    {
        return weapons;
    }

    // Get scrap
    public int GetScrap()
    {
        return currentScrap;
    }

    #endregion

    #region PUN

    [PunRPC]
    void PickupWeaponRPC(int viewID, string weaponName, int durability, int weaponViewID)
    {
        // Check if this pickup is mine and still exists
        if (PhotonNetwork.GetPhotonView(viewID).IsMine && PhotonNetwork.GetPhotonView(weaponViewID) != null)
        {
            addWeapon(weaponName, durability);
            Debug.Log("I picked up: " + weaponName);
        }
        if (PhotonNetwork.GetPhotonView(weaponViewID).IsMine && PhotonNetwork.GetPhotonView(weaponViewID) != null)
        {
            Debug.Log("I own this and I destroy");
            PhotonNetwork.Destroy(PhotonNetwork.GetPhotonView(weaponViewID).gameObject);
        }
    }

    #endregion
}
