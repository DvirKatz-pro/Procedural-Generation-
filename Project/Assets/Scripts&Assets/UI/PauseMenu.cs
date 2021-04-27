using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// PauseMenu
// Manages the pause menu
//
// Written by: Cal
public class PauseMenu : MonoBehaviour
{
    #region Variables

    // Menu objects
    public GameObject[] menuObjects = new GameObject[3];

    // Misc
    public static bool gamePaused;
    public GameObject pauseMenu;
    private Image pauseMenuPanel;
    private float easeTime = 0.5f;

    #endregion

    #region Main

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuPanel = pauseMenu.GetComponent<Image>();
        if (pauseMenuPanel == null)
            Debug.LogError("Pause menu panel on " + this.name + " is null.");

        pauseMenu.SetActive(false); // Ensure pause menu is closed on start
        gamePaused = false;
    }

    #endregion

    #region Pause / Unpause

    // Resumes game
    public void Resume()
    {
        ResetMenu();
        pauseMenu.SetActive(false);
        gamePaused = false;
    }

    // Pauses game
    public void Pause()
    {
        pauseMenu.SetActive(true);
        //pauseMenuUIimg.color = Color.Lerp(outColor, inColor, 1f * Time.deltaTime);
        gamePaused = true;
        StartCoroutine(AnimateMenuIn());
    }

    #endregion

    #region Animations and Menu Settings

    // Animate the menu items in
    IEnumerator AnimateMenuIn()
    {
        // Animate Current Menu In or Perform That Action
        foreach (GameObject currentElement in menuObjects)
        {
            AnimateIn(currentElement);
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    // Animate set of objects in
    private void AnimateIn(GameObject element)
    {
        LeanTween.moveLocalX(element, -1250, easeTime).setEaseOutSine();
        StartCoroutine(MakeClicable(element, easeTime));
    }

    // Transitions out of current state
    private void ResetMenu()
    {
        // Animate Current Menu Out
        foreach (GameObject currentElement in menuObjects)
        {
            currentElement.transform.localPosition = new Vector3(-2500, currentElement.transform.localPosition.y, currentElement.transform.localPosition.z);
            currentElement.GetComponent<PauseButtonManager>().clickable = false;
        }
    }

    // Toggle objects clickability
    IEnumerator MakeClicable(GameObject element, float delay)
    {
        yield return new WaitForSeconds(delay);
        element.GetComponent<PauseButtonManager>().clickable = true;
    }

    #endregion
}
