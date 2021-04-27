using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//control player blocking - Dvir
public class PlayerBlock : MonoBehaviour
{
    //blocking attributes
    private bool blocking = false;
    [SerializeField] private float blockingAngle = 0.0f;

    private bool specialBlock = false;
    private bool countdown = false;
    private float blockTimer = 0.0f;
    private float blockTimerAmount = 4.0f;


    private CombatController controller;
    private Animator animator;
    private PlayerMovement move;
    private PlayerStatus status;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CombatController>();
        animator = GetComponent<Animator>();
        move = GetComponent<PlayerMovement>();
        status = GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if (countdown)
        {
            specialBlock = true;
            if (Time.time > blockTimer)
            {
                specialBlock = false;
                countdown = false;
            }
        }
    }

    public void handleBlock()
    {
        //switch the state of the player based on key presses
        if (Input.GetMouseButton(1) && !blocking)
        {

            controller.setState(CombatController.State.Blocking);
            move.setMove(false);
            blocking = true;
            animator.SetBool("Blocking", true);
            countdown = true;
            blockTimer = Time.time + blockTimerAmount;



        }
        else if (!Input.GetMouseButton(1) && blocking)
        {
            controller.setState(CombatController.State.Normal);
            blocking = false;
        }
        if (blocking)
        {
            controller.setState(CombatController.State.Blocking);
            move.setMove(false);
            animator.SetBool("Blocking", true);
            move.rotate();
        }
        else
        {
            animator.SetBool("Blocking", false);
        }
    }
    /*
     * check to see if player is blocking
     */
    public bool isBlocking(float angle)
    {
        if (angle > blockingAngle)
        {
            return false;
        }
        if (blocking && specialBlock)
        {
            status.changeAp(20);
            countdown = false;
            specialBlock = false;
            
        }
        return blocking;
    }
}
