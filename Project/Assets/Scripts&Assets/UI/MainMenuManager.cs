using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

// MainMenuManager
// Manages the main menu, uses a state system
//
// Written by: Cal
public class MainMenuManager : MonoBehaviour
{
    #region Variables

    // States
    public enum MainMenuState { Main, Play, OnlineType, Host, Join, Options, Quit, Tutorial, Farm, Endless };
    public MainMenuState currentState;

    // Menu objects
    public GameObject[] mainObjects = new GameObject[4];
    public GameObject[] playObjects = new GameObject[4];
    public GameObject[] onlineTypeObjects = new GameObject[3];
    public GameObject[] onlineHostObjects = new GameObject[6];
    public GameObject[] onlineJoinObjects = new GameObject[6];
    public GameObject[] optionsObjects = new GameObject[5];

    // Misc
    private float easeTime = 0.4f;
    public GameObject loadingScreen;
    private LoadingManager loadingManager;
    private OnlineManager onlineManager;

    // Player Prefs
    public GameObject playerFileManagerObject;
    private PlayerFileManager playerFileManager;

    #endregion

    #region Main

    // Start is called before the first frame update
    void Start()
    {
        currentState = MainMenuState.Main;
        StartCoroutine(NextStateIn());

        loadingManager = loadingScreen.GetComponent<LoadingManager>();
        if (loadingManager == null)
            Debug.LogError("Loading manager on " + this.name + " is null.");

        onlineManager = GameObject.Find("OnlineManager").GetComponent<OnlineManager>();
        if (onlineManager == null)
            Debug.LogError("Online manager on " + this.name + " is null.");

        GameObject volumeSlider = GameObject.Find("Volume Slider");
        if (volumeSlider == null)
        {
            Debug.LogError("Volume slider on" + this.name + " is null.");
        }
        else
        {
            volumeSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("volume");
        }

        GameObject graphicsDropdown = GameObject.Find("Graphics Dropdown");
        if (graphicsDropdown == null)
        {
            Debug.LogError("Graphics dropdown on" + this.name + " is null.");
        }
        else
        {
            graphicsDropdown.GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt("graphics");
        }

    }

    #endregion

    #region State

    public void SetMenuState(MainMenuState newState)
    {
        if (currentState != newState)
        {
            StartCoroutine(CurrentStateOut());
            currentState = newState;
            StartCoroutine(NextStateIn());
        }
    }

    // Transitions out of current state
    IEnumerator CurrentStateOut()
    {
        // Animate Current Menu Out
        switch (currentState)
        {
            case MainMenuState.Main:
                foreach (GameObject currentElement in mainObjects)
                {
                    AnimateOut(currentElement);
                    yield return new WaitForSeconds(0.1f);
                }
                break;

            case MainMenuState.Play:
                foreach (GameObject currentElement in playObjects)
                {
                    AnimateOut(currentElement);
                    yield return new WaitForSeconds(0.1f);
                }
                break;

            case MainMenuState.OnlineType:
                foreach (GameObject currentElement in onlineTypeObjects)
                {
                    AnimateOut(currentElement);
                    yield return new WaitForSeconds(0.1f);
                }
                break;

            case MainMenuState.Host:
                foreach (GameObject currentElement in onlineHostObjects)
                {
                    AnimateOut(currentElement);
                    yield return new WaitForSeconds(0.1f);
                }
                break;

            case MainMenuState.Join:
                foreach (GameObject currentElement in onlineJoinObjects)
                {
                    AnimateOut(currentElement);
                    yield return new WaitForSeconds(0.1f);
                }
                break;

            case MainMenuState.Options:
                foreach (GameObject currentElement in optionsObjects)
                {
                    AnimateOut(currentElement);
                    yield return new WaitForSeconds(0.1f);
                }
                break;

            default:
                break;
        }
        yield return null;
    }

    // Animate set of objects out
    private void AnimateOut(GameObject element)
    {
        LeanTween.moveLocalX(element, -2500, easeTime).setEaseInSine();
        StartCoroutine(MakeClicable(element, 0.0f, false));
    }

    // Animate set of objects in
    private void AnimateIn(GameObject element)
    {
        LeanTween.moveLocalX(element, -1250, easeTime).setEaseOutSine();
        StartCoroutine(MakeClicable(element, easeTime, true));
    }

    // Toggle objects clickability
    IEnumerator MakeClicable(GameObject element, float delay, bool click)
    {
        yield return new WaitForSeconds(delay);
        if (element.GetComponent<ButtonManager>() != null)
            element.GetComponent<ButtonManager>().clickable = click;
    }

    // Transitions into next state
    IEnumerator NextStateIn()
    {
        // Animate Current Menu In or Perform That Action
        switch (currentState)
        {
            case MainMenuState.Main:
                foreach (GameObject currentElement in mainObjects)
                {
                    AnimateIn(currentElement);
                    yield return new WaitForSeconds(0.1f);
                }
                break;

            case MainMenuState.Play:
                if (PhotonNetwork.IsConnectedAndReady)
                    onlineManager.Disconnect();
                foreach (GameObject currentElement in playObjects)
                {
                    AnimateIn(currentElement);
                    yield return new WaitForSeconds(0.1f);
                }
                break;

            case MainMenuState.OnlineType:
                if (!PhotonNetwork.IsConnectedAndReady)
                    onlineManager.Connect();
                foreach (GameObject currentElement in onlineTypeObjects)
                {
                    AnimateIn(currentElement);
                    yield return new WaitForSeconds(0.1f);
                }
                break;

            case MainMenuState.Host:
                foreach (GameObject currentElement in onlineHostObjects)
                {
                    AnimateIn(currentElement);
                    yield return new WaitForSeconds(0.1f);
                }
                break;

            case MainMenuState.Join:
                foreach (GameObject currentElement in onlineJoinObjects)
                {
                    AnimateIn(currentElement);
                    yield return new WaitForSeconds(0.1f);
                }
                break;

            case MainMenuState.Options:
                foreach (GameObject currentElement in optionsObjects)
                {
                    AnimateIn(currentElement);
                    yield return new WaitForSeconds(0.1f);
                }
                break;

            case MainMenuState.Tutorial:
                this.gameObject.SetActive(false);
                loadingManager.LoadLevel("Tutorial");
                break;

            case MainMenuState.Farm:
                this.gameObject.SetActive(false);
                loadingManager.LoadLevel("Farm");
                break;

            case MainMenuState.Endless:
                this.gameObject.SetActive(false);
                loadingManager.LoadLevel("EndlessMode");
                break;

            case MainMenuState.Quit:
                Debug.Log("Game has been quit.");
                Application.Quit();
                break;

            default:
                break;
        }
        yield return null;
    }

    #endregion
}
