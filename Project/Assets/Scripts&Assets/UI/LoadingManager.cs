using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// LoadingManager
// Manages the loading screen
//
// Written by: Cal
public class LoadingManager : MonoBehaviour
{
    // Variables
    public GameObject loadingScreen;
    public Slider loadingBar;

    // Load the given level
    public void LoadLevel(string level)
    {
        loadingScreen.SetActive(true);
        StartCoroutine(Load(level));
    }

    // Manage the progress bar on the loading screen
    IEnumerator Load(string level)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(level);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress);
            loadingBar.value = progress;
            yield return null;
        }
    }
}
