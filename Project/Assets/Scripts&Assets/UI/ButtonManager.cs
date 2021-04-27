using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ButtonManager
// Manages the buttons on the main menu
//
// Written by: Cal
public class ButtonManager : MonoBehaviour
{
    #region Variables

    // Manager
    public GameObject mainMenu;
    private MainMenuManager mainMenuManager;

    // Sounds
    private AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    // Misc
    public bool clickable;

    #endregion

    #region Main

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogError("Audio source on " + this.name + " is null.");

        mainMenuManager = mainMenu.GetComponent<MainMenuManager>();
        if (mainMenuManager == null)
            Debug.LogError("Reference to main menu manager on " + this.name + " is null.");
    }

    #endregion

    #region Interactions

    // When button is hovered
    public void Hovered()
    {
        audioSource.PlayOneShot(hoverSound, 0.5f);
        LeanTween.scale(this.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.05f);
    }

    // When button is unhovered
    public void Unhovered()
    {
        LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.05f);
    }

    // When button is clicked
    public void Click()
    {
        if (clickable)
        {
            audioSource.PlayOneShot(clickSound, 1.0f);
            LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.025f);
            switch (this.name)
            {
                // MAIN

                case "Text Play":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.Play);
                    break;

                case "Text Online":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.OnlineType);
                    break;

                case "Text Options":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.Options);
                    break;

                case "Text Quit":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.Quit);
                    break;

                // PLAY

                case "Text Play Back":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.Main);
                    break;

                case "Text Tutorial":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.Tutorial);
                    break;

                case "Text Farm":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.Farm);
                    break;

                case "Text Endless":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.Endless);
                    break;

                // ONLINE TYPE

                case "Text Online Back":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.Main);
                    break;

                case "Text Host":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.Host);
                    break;

                case "Text Join":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.Join);
                    break;

                // HOST

                case "Text Host Back":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.OnlineType);
                    break;

                // JOIN

                case "Text Join Back":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.OnlineType);
                    break;

                // OPTIONS

                case "Text Options Back":
                    mainMenuManager.SetMenuState(MainMenuManager.MainMenuState.Main);
                    break;


                default:
                    Debug.LogError("Button text is invalid on " + this.name);
                    break;
            }
        }
    }

    #endregion
}
