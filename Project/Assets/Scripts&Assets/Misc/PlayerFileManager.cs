using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// PlayerFileManager
// Manages the options settings set by the user
//
// Written by: Cal
public class PlayerFileManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    // Get stored values
    void Start()
    {
        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("volume"));
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("graphics"));
    }

    // Set volume
    public void setVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("volume", newVolume);
        audioMixer.SetFloat("MasterVolume", newVolume);
    }

    // Set graphics
    public void setGraphics(int qualityLevel)
    {
        PlayerPrefs.SetInt("graphics", qualityLevel);
        QualitySettings.SetQualityLevel(qualityLevel);
    }
}
