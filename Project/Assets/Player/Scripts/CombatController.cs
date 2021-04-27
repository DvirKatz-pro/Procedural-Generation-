using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//control player combat states - Dvir
public class CombatController : MonoBehaviour
{
    //set the available player attacking states
    public enum State
    {
        Normal,
        Attacking,
        Dashing,
        Blocking,
        SpecialAttack,
        LazerAttack,
        SpinAttack,
        GrenadeThrow
    }
    State currentState;
    //set the references to the scripts that are in control of the states
    private PlayerBasicAttack attack;
    private PlayerMovement move;
    private PlayerDash dash;
    private PlayerBlock block;
    private SplashAttack splash;
    private LaserAttack lazer;
    private SpinAttack spin;
    private Grenade grenade;
    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Normal;
        attack = GetComponent<PlayerBasicAttack>();
        move = GetComponent<PlayerMovement>();
        dash = GetComponent<PlayerDash>();
        block = GetComponent<PlayerBlock>();
        splash = GetComponent<SplashAttack>();
        lazer = GetComponent<LaserAttack>();
        spin = GetComponent<SpinAttack>();
        grenade = GetComponent<Grenade>();
    }
    void FixedUpdate()
    {
        //determine what can be done for each state
        if (currentState == State.Normal)
        {
            if (!move.isMoving())
            {
                move.setMove(true);
                move.setAnimate(true);
            }
            attack.handleAttack();
            dash.handleDash();

        }
        else if (currentState == State.Attacking)
        {
            move.setMove(false);
            move.setAnimate(false);
            attack.handleAttack();

        }
        else if (currentState == State.Dashing)
        {
            move.setMove(false);
            move.setAnimate(false);
            dash.handleDash();

        }
    }
    // Update is called once per frame
    void Update()
    {
        //decide what the player can and cant do based on his current state
        if (currentState == State.Normal)
        {
            if (!move.isMoving())
            {
                move.setMove(true);
                move.setAnimate(true);
                move.setRotate(true);
            }
            block.handleBlock();
            splash.splashAttack();
            lazer.laserAttack();
            spin.spinAttack();
            grenade.handleAttack();

        }
        else if (currentState == State.SpecialAttack)
        {
            splash.splashAttack();
        }
        else if (currentState == State.Blocking)
        {
            move.setMove(false);
            move.setAnimate(false);
            block.handleBlock();


        }
        else if (currentState == State.LazerAttack)
        {           
            lazer.laserAttack();
        }
        else if (currentState == State.SpinAttack)
        {
            spin.spinAttack();
        }
        else if (currentState == State.GrenadeThrow)
        {
            move.setMove(false);
            move.setAnimate(false);
        }
    }



    public void setState(State state)
    {
        currentState = state;
    }
}
