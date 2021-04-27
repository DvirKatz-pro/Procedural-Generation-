using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PopupButton
// Manages the dismiss button on the popup
//
// Written by: Cal
public class PopupButton : MonoBehaviour
{
    #region Variables

    // Misc
    public PopupManager popupManager;

    // Sounds
    private AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    #endregion

    #region Main

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogError("Audio source on " + this.name + " is null.");
    }

    #endregion

    #region Interactions

    public void Hovered()
    {
        audioSource.PlayOneShot(hoverSound, 0.5f);
        LeanTween.scale(this.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
    }

    public void Unhovered()
    {
        LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.2f);
    }

    public void Click()
    {
        if (popupManager.popupVisible)
        {
            audioSource.PlayOneShot(clickSound, 1.0f);
            LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.05f);
            popupManager.HidePopup();
        }
    }

    #endregion
}
