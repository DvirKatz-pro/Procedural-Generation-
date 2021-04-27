using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ObjectiveManager
// Manages the UI for the objectives in the tutorial level
//
// Written by: Cal
public class ObjectiveManager : MonoBehaviour
{
    #region Variables

    public GameObject textObject;
    private TextMeshProUGUI text;
    private Queue<string> textQueue = new Queue<string>();
    public bool objectiveVisible;
    Image panel;
    private Color defaultColor;
    private Color successColor = new Color(0.203f, 0.698f, 0.309f, 0.9f);

    #endregion

    #region Main

    // Initialize
    void Awake()
    {
        text = textObject.GetComponent<TextMeshProUGUI>();
        if (text == null)
            Debug.LogError("TextMesh on " + this.gameObject + " is null.");

        panel = this.GetComponent<Image>();
        if (panel == null)
            Debug.LogError("Panel on " + this.gameObject + " is null.");
        defaultColor = panel.color;
    }

    #endregion

    #region Text

    // Set the text
    public void QueueText(string textToQueue)
    {
        textQueue.Enqueue(textToQueue);
        if (!objectiveVisible)
        {
            ShowObjective();
        }
    }

    #endregion

    #region Objective

    // Show the objective
    public void ShowObjective()
    {
        text.SetText(textQueue.Dequeue());
        objectiveVisible = true;
        panel.color = defaultColor;
        LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.5f).setEaseOutSine();
    }

    // Objective success
    public void ObjectiveSuccess()
    {
        panel.color = successColor;
        LeanTween.scale(this.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 1f).setEaseInOutSine().setOnComplete(HideObjective);
    }

    // Hide the objective
    public void HideObjective()
    {
        LeanTween.scale(this.gameObject, new Vector3(0f, 0f, 0f), 0.5f).setEaseInSine().setOnComplete(FinishObjective);
    }

    // Disable the objective
    public void FinishObjective()
    {
        if (textQueue.Count > 0)
        {
            ShowObjective();
        }
        else
        {
            objectiveVisible = false;
        }
    }

    #endregion
}
