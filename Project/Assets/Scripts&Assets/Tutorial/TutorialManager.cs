using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// TutorialManager
// Manages the tutorial in the tutorial level
//
// Written by: Cal
public class TutorialManager : MonoBehaviour
{
    #region Variables

    // Player
    [SerializeField] private GameObject player;
    private Inventory inventory;
    private PlayerStatus playerStatus;

    // Tutorial Manager
    private int tutorialArea;
    private bool startedTutorial;
    private bool currentObjectiveCompleted;
    public GameObject[] areaTriggers = new GameObject[5];

    // Enemies and targets
    public GameObject attackEnemy;
    public GameObject specialAttackEnemy;
    public GameObject target1, target2, target3;

    // UI
    public GameObject popup;
    private PopupManager popupManager;
    public GameObject objective;
    private ObjectiveManager objectiveManager;

    // Text
    private string tutorialSpawnText = "Welcome to the tutorial bunker.\nIn each room you have a task to complete which teaches you how to play the game. Your current task is shown on the bottom of your screen, and it must be completed before you can advance to the next room.\nYou can press 'H' on your keyboard at any time during the tutorial for more information how to complete your task.";
    private string[] popupText = new string[5];
    private string[] objectiveText = new string[5];

    #endregion

    #region Main

    // Start is called before the first frame update
    void Start()
    {
        // Player
        inventory = player.GetComponent<Inventory>();
        if (inventory == null)
            Debug.LogError("Inventory from player on " + this.name + " is null.");

        playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus == null)
            Debug.LogError("PlayerStatus from player on " + this.name + " is null.");

        // Popup text
        popupText[0] = "The player will move in the direction that you mouse is positioned within the world.\nYou can then press 'W' to move forward (towards the mouse cursor) and 'S' to move backwards (away from the mouse cursor).";
        popupText[1] = "You can press 'SPACE' to dash towards your mouse cursors position within the world.\nThis is useful to quickly get in and out of combat scenarios.";
        popupText[2] = "You can pick up weapons from around the map by walking over them. Weapons can then be accessed via the inventory by pressing 'I' or 'E'.\nYou can see weapon stats from this menu, and equip a new one by pressing 'Equip'.\nYou can close the inventory by pressing 'I', 'E', or 'ESC'.";
        popupText[3] = "You can attack an enemy using your weapon by clicking 'LEFT CLICK'.\nAdditionally, you can click and hold 'RIGHT CLICK' to block an incoming attack and prevent damage.\nSpecial Attacks are achieved by using AP and by clicking 'LEFT SHIFT' and 'LEFT CLICK'.";
        popupText[4] = "When an enemy dies they drop what is called scrap.\nAfter you collect enough scrap, you can throw a grenade by pressing 'G'.\nThe grenade will be thrown to the exact position of your cursor, or as close as possible to it, if your cursor is out of range.\nThe grenade will explode automatically, damaging any nearby enemies.";

        // Popup Manager
        popupManager = popup.GetComponent<PopupManager>();
        if (popupManager == null)
            Debug.LogError("Popup Manager on " + this.name + " is null.");

        popupManager.SetText(tutorialSpawnText);
        popupManager.ShowPopup();

        // Objective text
        objectiveText[0] = "Current Objective: Move (W) & (S)";
        objectiveText[1] = "Current Objective: Dash (SPACE)";
        objectiveText[2] = "Current Objective: Switch Weapons (I) & Kill Enemy (LMB)";
        objectiveText[3] = "Current Objective: Kill Enemy with Special Attack (SHIFT + LMB)";
        objectiveText[4] = "Current Objective: Destory a target using a Grenade (G)";

        // Objective Manager
        objectiveManager = objective.GetComponent<ObjectiveManager>();
        if (objectiveManager == null)
            Debug.LogError("Objective Manager on " + this.name + " is null.");

        // Tutorial area
        tutorialArea = 0;
    }

    // Update checks to see if current tutorial area task has been completed
    void Update()
    {
        // Used to start the tutorial

        if (startedTutorial && !currentObjectiveCompleted)
        {
            switch (tutorialArea)
            {
                case 0:
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
                        ObjectiveComplete();
                    break;
                case 1:
                    if (Input.GetKeyDown(KeyCode.Space))
                        ObjectiveComplete();
                    break;
                case 2:
                    if (inventory.GetCurrentWeapon().GetWeaponName().Equals("Axe") && attackEnemy.GetComponent<PinEnemyTraining>().isDead)
                        ObjectiveComplete();
                    break;
                case 3:
                    if (specialAttackEnemy.GetComponent<PinEnemyTraining>().isDead)
                        ObjectiveComplete();
                    break;
                case 4:
                    if (target1.GetComponent<TrainingTarget>().isDead || target2.GetComponent<TrainingTarget>().isDead || target3.GetComponent<TrainingTarget>().isDead)
                        ObjectiveComplete();
                    break;
                default:
                    Debug.LogError("Tutorial area index out of bounds");
                    break;
            }
        }
        else if (!startedTutorial && !popupManager.popupVisible)
        {
            startedTutorial = true;
            IntializeTutorialArea();
        }
    }

    #endregion

    #region Objects and Areas

    // When an objective is completed
    private void ObjectiveComplete()
    {
        currentObjectiveCompleted = true;
        objectiveManager.ObjectiveSuccess();
        TutorialAreaTrigger trigger = areaTriggers[tutorialArea].GetComponent<TutorialAreaTrigger>();
        if (trigger == null)
            Debug.LogError("Tutorial area trigger on " + this.name + " is null.");
        trigger.Advance();
        playerStatus.changeAp(200);
    }

    // Change combat area
    public void NextTutorialArea()
    {
        tutorialArea++;
        if (tutorialArea == areaTriggers.Length)
        {
            SceneManager.LoadScene("Farm");
        }
        IntializeTutorialArea();
    }

    // Initialize next tutorial area
    public void IntializeTutorialArea()
    {
        currentObjectiveCompleted = false;

        objectiveManager.QueueText(objectiveText[tutorialArea]);

        popupManager.SetText(popupText[tutorialArea]);
    }

    #endregion
}
