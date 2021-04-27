using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// LobbyButtonManager
// Manages the buttons in the lobby
//
// Written by: Cal
public class LobbyButtonManager : MonoBehaviour
{
    #region Variables

    // Manager
    public GameObject lobbyManagerObject;
    private LobbyManager lobbyManager;

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

        lobbyManager = lobbyManagerObject.GetComponent<LobbyManager>();
        if (lobbyManager == null)
            Debug.LogError("Lobby Manager on " + this.name + " is null.");
    }

    #endregion

    #region Interactions

    // When the button is hovered
    public void Hovered()
    {
        audioSource.PlayOneShot(hoverSound, 0.5f);
        LeanTween.scale(this.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
    }

    // When the button is unhovered
    public void Unhovered()
    {
        LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.2f);
    }

    // When the button is clicked
    public void Click()
    {
        if (clickable)
        {
            clickable = false;
            audioSource.PlayOneShot(clickSound, 1.0f);
            LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.05f);
            switch (this.name)
            {
                // Leave
                case "Leave":
                    lobbyManager.Leave();
                    break;

                // Start
                case "Start":
                    lobbyManager.StartGame();
                    break;

                default:
                    Debug.LogError("Button text is invalid on " + this.name);
                    break;
            }
        }
    }

    #endregion
}
