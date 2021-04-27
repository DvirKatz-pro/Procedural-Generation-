using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UIManager
// Manages the games UI
//
// Written by: Cal
public class UIManager : MonoBehaviour
{
    #region Variables

    // UI Manager
    public enum UIState { Game, Inventory, Paused, Death, Win };
    private UIState currentUIState;
    private GameObject player;

    // Game
    [SerializeField] private GameObject gameUI;

    // Inventory 
    [SerializeField] private GameObject inventoryUI;
    private Inventory inventoryManager;

    // Pause
    private PauseMenu pauseMenuManager;

    // Death & Win
    [SerializeField] private GameObject deathUI;
    [SerializeField] private GameObject winUI;

    #endregion

    #region Main

    // Set the player
    public void SetPlayer(GameObject p)
    {
        player = p;
        Initialize();
    }

    // Initialize
    private void Initialize()
    {
        inventoryManager = player.GetComponent<Inventory>();
        if (inventoryManager == null)
            Debug.LogError("Inventory manager on " + this.name + " is null.");
        pauseMenuManager = GetComponent<PauseMenu>();
        if (pauseMenuManager == null)
            Debug.LogError("Pause menu UI on " + this.name + " is null.");

        // Initialize the state and enter the first state
        currentUIState = UIState.Game;
        EnterState();
        this.enabled = true;
    }

    // Update method
    void Update()
    {
        switch (currentUIState)
        {
            case UIState.Game:
                if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.E))
                {
                    UpdateState(UIState.Inventory);
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    UpdateState(UIState.Paused);
                }
                break;

            case UIState.Inventory:
                if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
                {
                    UpdateState(UIState.Game);
                }
                break;

            case UIState.Paused:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    UpdateState(UIState.Game);
                }
                break;

            default:
                break;
        }

    }

    #endregion

    #region State

    public void UpdateState(UIState newState)
    {
        ExitState();
        currentUIState = newState;
        EnterState();
    }

    public void ExitState()
    {
        switch (currentUIState)
        {
            case UIState.Game:
                gameUI.SetActive(false);
                break;

            case UIState.Inventory:
                inventoryUI.SetActive(false);
                //inventoryManager.setActive(false);
                break;

            case UIState.Paused:
                pauseMenuManager.Resume();
                break;

            case UIState.Death:
                deathUI.SetActive(false);
                break;

            case UIState.Win:
                winUI.SetActive(false);
                break;

            default:
                break;
        }
    }

    public void EnterState()
    {
        switch (currentUIState)
        {
            case UIState.Game:
                gameUI.SetActive(true);
                break;

            case UIState.Inventory:
                inventoryUI.SetActive(true);
                break;

            case UIState.Paused:
                pauseMenuManager.Pause();
                break;

            case UIState.Death:
                deathUI.SetActive(true);
                break;

            case UIState.Win:
                winUI.SetActive(true);
                break;

            default:
                break;
        }
    }

    #endregion
}
