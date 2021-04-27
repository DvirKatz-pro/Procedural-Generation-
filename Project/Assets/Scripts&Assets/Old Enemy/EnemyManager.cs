using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// [NO LONGER IN USE]
// EnemyManager
//
// Written by: Cal
public class EnemyManager : MonoBehaviour
{
    // Enemy type ( Pin, Punisher )
    [SerializeField] string enemyType;

    // Player
    [SerializeField] public GameObject player;

    // Animator and AudioSource
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource audioSource;

    // Associated scripts
    [HideInInspector] public EnemyAttack eAttack;
    EnemyChase eChase;
    EnemyDeath eDeath;
    EnemyHealth eHealth;

    // State
    private enum State { Idle, Chasing, Attacking, Stunned, Dead };
    State currentState;

    // Chasing & Config
    [SerializeField] private float distanceToAttack;
    [SerializeField] private float distanceToChase;
    [SerializeField] float attackCooldown;
    float lastAttack;
    bool canAttack;


    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();

        // Set the associated scripts based on enemy type
        switch (enemyType.ToUpper())
        {
            case "PIN":
                eAttack = this.GetComponent<PinAttack>();
                eChase = this.GetComponent<PinChase>();
                eDeath = this.GetComponent<PinDeath>();
                eHealth = this.GetComponent<EnemyHealth>();
                break;

            case "PUNISHER":
                eAttack = this.GetComponent<PunisherAttack>();
                eChase = this.GetComponent<PunisherChase>();
                eDeath = this.GetComponent<PunisherDeath>();
                eHealth = this.GetComponent<EnemyHealth>();
                break;

            default:
                Debug.LogError(this.name + " has no or invalid enemy type.");
                break;
        }

        // Starting state
        canAttack = true;
        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 myPosition = this.transform.position;
        float distanceToPlayer = Vector3.Distance(myPosition, playerPosition);

        if (!canAttack && Time.time - lastAttack >= attackCooldown)
        {
            canAttack = true;
        }

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer <= distanceToAttack && canAttack) // Should attack
                {
                    currentState = State.Attacking;
                    canAttack = false;
                    lastAttack = Time.time;
                    eAttack.Attack();
                }
                else if (distanceToPlayer <= distanceToChase && distanceToPlayer > distanceToAttack) // Should chase
                {
                    currentState = State.Chasing;
                    eChase.StartChase();
                }
                break;

            case State.Chasing:
                if (distanceToPlayer <= distanceToAttack && canAttack) // Should attack
                {
                    currentState = State.Attacking;
                    canAttack = false;
                    lastAttack = Time.time;
                    eChase.StopChase();
                    eAttack.Attack();
                }
                else if (distanceToPlayer > distanceToChase && distanceToPlayer > distanceToAttack) // Should stop chasing
                {
                    currentState = State.Idle;
                    eChase.StopChase();
                }
                break;

            case State.Attacking:
                break;

            case State.Stunned:
                break;

            default:
                Debug.LogError(this.name + " has no or invalid state.");
                break;

        }
    }

    // Set the state of the enemy
    public void SetState(string newState)
    {
        switch (newState.ToUpper())
        {
            case "IDLE":
            case "IDLING":
                currentState = State.Idle;
                break;

            case "CHASE":
            case "CHASING":
                currentState = State.Chasing;
                break;

            case "ATTACK":
            case "ATTACKING":
                currentState = State.Attacking;
                break;

            case "DEAD":
            case "DYING":
                currentState = State.Dead;
                break;

            case "STUNNED":
                currentState = State.Stunned;
                break;

            default:
                Debug.LogError("Invalid state was given to " + this.name + ".");
                break;
        }
    }

    public bool isEnemyAttacking()
    {
        if (currentState == State.Attacking)
            return true;
        else
            return false;
    }

    // Kill the enemy
    public void kill()
    {
        eDeath.Death();
        enabled = false;
    }
}
