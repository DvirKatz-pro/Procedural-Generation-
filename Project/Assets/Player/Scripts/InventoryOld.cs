using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

//Inventory Control - Dvir/Cal
public class InventoryOld : MonoBehaviour
{
    //current Weapon
    public GameObject currentWeapon;
    [SerializeField] private GameObject WeaponPosition;
    [SerializeField] private Transform weapon;

    //refernce to UI
    private GameObject inventoryUI;

    //refernce to player status
    private PlayerStatus status;
    //invetory references 
    private int scrap = 0;
    private List<GameObject> inventory;
    private GameObject currentWeaponSlot;

    private UIElements uiElements;


    // Start is called before the first frame update
    void Start()
    {
        // UI components
        uiElements = GameObject.Find("GameCanvas").GetComponent<UIElements>();
        if (uiElements == null)
            Debug.LogError("UIElements on " + this.name + " is null.");

        currentWeaponSlot = uiElements.currentWeaponSlot;
        inventoryUI = uiElements.inventory;

        //add all the slots to an inventory list
        inventory = new List<GameObject>();
        // weapon = currentWeapon.transform;
        status = GetComponent<PlayerStatus>();
        foreach (Transform child in inventoryUI.transform)
        {
            inventory.Add(child.gameObject);
        }
        currentWeaponSlot.GetComponent<InventorySlot>().setSlot(currentWeapon);
        GameObject child1 = currentWeaponSlot.transform.GetChild(0).gameObject;
        Image sprite = child1.GetComponent<Image>();
        WeaponStats stats = currentWeapon.GetComponent<WeaponStats>();
        //sprite.sprite = stats.inventoryImage;
        sprite.gameObject.SetActive(true);
        // COMMENT THIS BACK IN sprite.GetComponent<TextPopUp>().setText(stats.name, stats.damage, stats.getDurability());
        //currentWeaponText.text = "            Current Weapon \n\nName:        " + currentWeapon.name + "\n\n" + "Damage:        " + currentWeapon.GetComponent<WeaponStats>().damage;
    }

    // Update is called once per frame
    /*
    void Update()
    {

        //enable or disable inventory
        if (Input.GetKeyDown(KeyCode.I) && !inventoryActive)
        {
            Time.timeScale = 0.00001f;
            inventoryUI.SetActive(true);
            inventoryActive = true;
            GetComponent<CombatController>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerStatus>().enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.I) && inventoryActive)
        {
            Time.timeScale = 1f;
            inventoryUI.SetActive(false);
            inventoryActive = false;
            GetComponent<CombatController>().enabled = true;
            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<PlayerStatus>().enabled = true;

        }

    }
    */

    public void setActive(bool active)
    {
        if (active)
        {
            GetComponent<CombatController>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerStatus>().enabled = false;
        }
        else
        {
            GetComponent<CombatController>().enabled = true;
            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<PlayerStatus>().enabled = true;
        }

    }

    /*
    public void addToInventory(GameObject item)
    {
        //disable the item
        item.tag = "Untagged";
        item.transform.GetChild(0).gameObject.SetActive(false);
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        //if the item is scrap, add to the scrap bar
        if (item.name == "Scrap(Clone)")
        {
            scrap += 50;

            if (scrap >= 100)
            {
                scrap = 100;
            }
            status.changeScrap(scrap);
        }
        else
        {
            //find an empty slot to add the wepon to
            foreach (GameObject slot in inventory)
            {
                if (slot.name != "CurrentWeapon" && slot.GetComponent<InventorySlot>().getSlot() == null)
                {

                    slot.GetComponent<InventorySlot>().setSlot(item);
                    GameObject child = slot.transform.GetChild(0).gameObject;
                    Image sprite = child.GetComponent<Image>();
                    WeaponStats stats = item.GetComponent<WeaponStats>();
                    sprite.sprite = stats.inventoryImage;
                    sprite.gameObject.SetActive(true);
                    sprite.GetComponent<TextPopUp>().setText(stats.name, stats.damage, stats.getDurability());
                    break;

                }
            }

        }
    }
    */

    // Add to inventory, written by Cal
    public void addToInventory(string itemName)
    {
        switch (itemName)
        {
            case "Scrap":
                scrap = scrap += 50 > 100 ? scrap + 50 : 100;
                status.changeScrap(scrap);
                break;

            default:
                break;
        }
    }

    /* add a weapon as current Weapon
     * item - the item we want to add
     */
    public void addAsCurrent(GameObject item)
    {
        //set inventory slots and activate the weapon
        currentWeaponSlot.GetComponent<InventorySlot>().setSlot(item);
        GameObject child = currentWeaponSlot.transform.GetChild(0).gameObject;
        Image sprite = child.GetComponent<Image>();
        WeaponStats stats = item.GetComponent<WeaponStats>();
        //sprite.sprite = stats.inventoryImage;
        sprite.gameObject.SetActive(true);
        sprite.GetComponent<TextPopUp>().setText(stats.name, stats.damage, stats.getDurability());
    }
    /*
     * switch to a weapon given its name
     */
    public void switchWeapon(string name)
    {
        //if the inventory slot is not empty, switch to that weapon
        foreach (GameObject slot in inventory)
        {
            if (slot.name != "CurrentWeapon" && slot.GetComponent<InventorySlot>().getSlot() != null)
            {
                GameObject testWeapon = slot.GetComponent<InventorySlot>().getSlot();
                WeaponStats stats = testWeapon.GetComponent<WeaponStats>();
                if (stats.name == name)
                {
                    StartCoroutine(Switch(slot, testWeapon));
                }
            }
        }


    }
    /*
     * return current weapon damage
     */
    public int getWeaponDamage()
    {
        return currentWeapon.GetComponent<WeaponStats>().damage;
    }
    //return scrap amount
    public int getScrap()
    {
        return scrap;
    }
    //set scrap amount
    public void setScrap(int amount)
    {
        scrap = amount;
    }
    /*
     * switch to a weapon given the gameobject and its slot position
     */
    IEnumerator Switch(GameObject m_slot, GameObject m_testWeapon)
    {
        //add the weapon to the hand position, and update the inventory slots 
        m_slot.GetComponent<InventorySlot>().setSlot(null);
        GameObject child = m_slot.transform.GetChild(0).gameObject;
        GameObject child1 = currentWeaponSlot.transform.GetChild(0).gameObject;
        child.SetActive(false);
        child1.SetActive(false);
        yield return new WaitForSeconds(0.000005f);
        addToInventory(currentWeapon.name);
        addAsCurrent(m_testWeapon);


        currentWeapon.SetActive(false);
        currentWeapon = m_testWeapon;
        currentWeapon.SetActive(true);
        currentWeapon.transform.parent = WeaponPosition.transform;
        currentWeapon.transform.position = weapon.position;
        currentWeapon.transform.rotation = weapon.rotation;

    }
    /*
     * find a weapon to be discarded given its name
     */
    public void disacrdWeapon(string name)
    {
        //find the slot and discard the weapon
        foreach (GameObject slot in inventory)
        {
            if (slot.name != "CurrentWeapon" && slot.GetComponent<InventorySlot>().getSlot() != null)
            {
                GameObject testWeapon = slot.GetComponent<InventorySlot>().getSlot();
                WeaponStats stats = testWeapon.GetComponent<WeaponStats>();
                if (stats.name == name)
                {
                    StartCoroutine(Discard(slot, testWeapon));
                }
            }
        }
    }
    //return the current weapon
    public GameObject getCurrentWeapon()
    {
        return currentWeapon;
    }
    /*
     * discard the weapon
     */
    IEnumerator Discard(GameObject m_slot, GameObject m_testWeapon)
    {
        //set its inventory slot to empty
        m_slot.GetComponent<InventorySlot>().setSlot(null);
        GameObject child = m_slot.transform.GetChild(0).gameObject;
        child.SetActive(false);
        yield return new WaitForSeconds(0.000005f);

        Vector3 position = transform.position + (transform.forward * 7) * -1;
        position.y += 0.5f;

        Quaternion rotation = Quaternion.Euler(0, 0, 90);

        //"Throw" the weapon 
        m_testWeapon.transform.position = position;
        m_testWeapon.transform.rotation = rotation;

        m_testWeapon.transform.GetChild(0).gameObject.SetActive(true);
        Rigidbody rb = m_testWeapon.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        m_testWeapon.SetActive(true);
        m_testWeapon.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionY;
        m_testWeapon.transform.parent = null;
        yield return new WaitForSeconds(2);
        m_testWeapon.tag = "Pickup";
    }

    [PunRPC]
    void PickupRPC(int viewID, string pickupName, int weaponViewID)
    {
        // Check if this pickup is mine and still exists
        if (PhotonNetwork.GetPhotonView(viewID).IsMine && PhotonNetwork.GetPhotonView(weaponViewID) != null)
        {
            //inventory.addToInventory(itemName);
            Debug.Log("I picked up: " + pickupName);
        }
        if (PhotonNetwork.GetPhotonView(weaponViewID).IsMine && PhotonNetwork.GetPhotonView(weaponViewID) != null)
        {
            Debug.Log("I own this and I destroy");
            PhotonNetwork.Destroy(PhotonNetwork.GetPhotonView(weaponViewID).gameObject);
        }
    }
}
