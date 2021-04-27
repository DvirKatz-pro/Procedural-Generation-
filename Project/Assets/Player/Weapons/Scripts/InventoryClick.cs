using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryClick : MonoBehaviour, IPointerClickHandler
{
    TextPopUp popUp;
    private GameObject player;
    private InventoryOld inventoryInstance;

    public GameManager gameManager;

    public void Start()
    {
        // Game Manager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager == null)
            Debug.LogError("Game Manager on " + this.name + " is null.");

        player = gameManager.GetPlayer();

        popUp = GetComponent<TextPopUp>();
        inventoryInstance = player.GetComponent<InventoryOld>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        int tap = eventData.clickCount;

        if (tap == 2 && eventData.button != PointerEventData.InputButton.Right)
        {
            Debug.Log("Double Click");
            if (inventoryInstance == null)
                Debug.LogError("Inventory instance null");
            if (popUp == null)
                Debug.LogError("Popup null");
            inventoryInstance.switchWeapon(popUp.getName());
        }
        if (tap == 2 && eventData.button == PointerEventData.InputButton.Right)
        {

            Debug.Log("Double Click");
            if (inventoryInstance == null)
                Debug.LogError("Inventory instance null");
            if (popUp == null)
                Debug.LogError("Popup null");
            inventoryInstance.disacrdWeapon(popUp.getName());
        }

    }
}
