using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// PopupManager
// Manages the UI for the popup in the tutorial level
//
// Written by: Cal
public class PopupManager : MonoBehaviour
{
    #region Variables

    public GameObject textObject;
    private TextMeshProUGUI text;
    public bool popupVisible;

    #endregion

    #region Main

    void Awake()
    {
        text = textObject.GetComponent<TextMeshProUGUI>();
        if (text == null)
            Debug.LogError("TextMesh on " + this.gameObject + " is null.");
    }

    void Update()
    {
        if (!popupVisible && Input.GetKeyDown(KeyCode.H))
        {
            ShowPopup();
        }
        else if (popupVisible && Input.GetKeyDown(KeyCode.Escape))
        {
            HidePopup();
        }
    }

    #endregion

    #region Text and Popup

    // Set the text
    public void SetText(string textToDisplay)
    {
        text.SetText(textToDisplay);
    }

    // Show the popup
    public void ShowPopup()
    {
        popupVisible = true;
        LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.5f).setEaseOutSine();
    }

    // Hide the popup
    public void HidePopup()
    {
        LeanTween.scale(this.gameObject, new Vector3(0f, 0f, 0f), 0.5f).setEaseInSine().setOnComplete(DisablePopup);
    }

    // Disable the popup
    public void DisablePopup()
    {
        popupVisible = false;
    }

    #endregion
}
