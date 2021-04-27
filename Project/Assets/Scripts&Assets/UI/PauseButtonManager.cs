using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

// PauseButtonManager
// Manages the buttons in the pause menu
//
// Written by: Cal
public class PauseButtonManager : MonoBehaviourPunCallbacks
{
    #region Variables

    // Manager
    public GameObject pauseMenu;
    private PauseMenu pauseMenuManager;
    //public GameObject levelManager;
    //private LevelManager levelManagerScript;
    private UIManager uiManager;
    private GameManager gameManager;

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

        pauseMenuManager = pauseMenu.GetComponent<PauseMenu>();
        if (pauseMenuManager == null)
            Debug.LogError("Reference to pause menu manager on " + this.name + " is null.");

        uiManager = GetComponentInParent<UIManager>();
        if (uiManager == null)
            Debug.LogError("UI Manager on " + this.name + " is null.");

        GameObject gm = GameObject.Find("GameManager");
        if (gm == null)
            Debug.LogError("Can't find game object GameManager on " + this.name + " is null.");

        gameManager = gm.GetComponent<GameManager>();
        if (gameManager == null)
            Debug.LogError("Game Manager on " + this.name + " is null.");
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
            audioSource.PlayOneShot(clickSound, 1.0f);
            LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.05f);
            switch (this.name)
            {
                // Resume
                case "Text Resume":
                    uiManager.UpdateState(UIManager.UIState.Game);
                    break;

                // Restart Level
                case "Text Restart":
                case "Death Text Retry":
                case "Win Text Play Again":
                    if (PhotonNetwork.OfflineMode)
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                    else if (PhotonNetwork.IsMasterClient)
                    {
                        gameManager.ReloadLevel();
                    }
                    break;

                // Quit Level
                case "Text Quit":
                case "Death Text Quit":
                case "Win Text Quit":
                    if (PhotonNetwork.OfflineMode || PhotonNetwork.CurrentRoom == null)
                    {
                        SceneManager.LoadScene(0);
                    }
                    else
                    {
                        gameManager.Leave();
                    }
                    break;

                default:
                    Debug.LogError("Button text is invalid on " + this.name);
                    break;
            }
        }
    }

    #endregion
}
