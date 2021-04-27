using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//Based on the pin enemy script made by Cal, changes made Dvir 
public class PinEnemyEndless : Enemy
{
    // Enemy
    public static EnemyType enemyType = EnemyType.Close;
    [SerializeField] private GameObject scrapPrefab;
    [SerializeField] private GameObject damageFXPrefab;
    [SerializeField] private GameObject destroyFXPrefab;

    // Player
    private GameObject player;

    // Stats
    [SerializeField] private int attackStrength;
    [SerializeField] private float maxHealth;
    [SerializeField] private int score;
    private float health;
     

    // Manager & Components
    private Rigidbody enemyRB;

    // Navmesh
    private NavMeshAgent navMeshAgent;

    // State
    private EnemyState currentState;
    private EnemyState lastState;
    private bool isAttacking;
    private bool isMoving;
    private bool playerSpotted;

    // Animator and AudioSource / Sounds
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource audioSource;
    [SerializeField] private AudioClip deathAudio;
    private Score scoreScript;
    private EnemySpawn enemySpawn;
    // Iniatlizes enemy: registers itself with AI manager, 
   

    // Initialize the enemy
    protected override void Start()
    {
        // Get the components
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyRB = GetComponent<Rigidbody>();
        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();
        enemySpawn = GameObject.Find("TerrainGenerator").GetComponent<EnemySpawn>();
        player = GameObject.Find("Player");
        scoreScript = GameObject.Find("Canvas").transform.GetChild(1).gameObject.GetComponent<Score>();
        SetState(EnemyState.Idle);
        health = maxHealth;
        isAttacking = false;
        isMoving = false;
    }

    // Returns current enemy type
    public override EnemyType GetEnemyType()
    {
        return enemyType;
    }

    protected override void Update()
    {
        if (currentState != EnemyState.Dead)
        {
            if (navMeshAgent.velocity.magnitude >= 1.0f && !isMoving)
            {
                animator.SetBool("Dash Forward", true);
                isMoving = true;
            }
            else if (navMeshAgent.velocity.magnitude < 1.0f && isMoving)
            {
                animator.SetBool("Dash Forward", false);
                isMoving = false;
            }

            // Only update if the current enemy is not stunned or dead
            if (currentState != EnemyState.Stunned)
            {
                float distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);

                // Spot the player
                if (!playerSpotted && distanceToPlayer < 15.0f)
                {
                    playerSpotted = true;
                    // Get into formation
                    Vector3 relativeFormationPosition = player.transform.position;
                    RotateTowards(player.transform.position);
                    SetDestination(relativeFormationPosition, 0.0f);
                    
                    SetState(EnemyState.Attack);
                    
                }
                else if(distanceToPlayer > 15.0f)
                {
                    playerSpotted = false;
                    SetState(EnemyState.Idle);
                }

                // Force an attack because the player is so close to me or attack because of state
                if (distanceToPlayer < 5.0 || currentState == EnemyState.Attack)
                {
                    Attack();
                    return;
                }

                // I'm close enough to the enemy I would like to attack
                if (distanceToPlayer <= 15.0f)
                {
                    if (currentState == EnemyState.Attack)
                    {
                        Attack();
                        return;
                    }
                }

               
            }
        }
    }

    //
    //  ACTION RELATED FUNCTIONS
    //
    protected override void SetDestination(Vector3 position, float stoppingDistance)
    {
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.SetDestination(position);
    }

    protected override void Attack()
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) > 4.0f)
        {
            SetDestination(player.transform.position, 3.5f);
            return;
        }

        // Get angle between enemy and player
        Vector3 enemyForward = this.transform.forward;
        Vector3 playerDirection = player.transform.position - this.transform.position;
        float angle = Vector3.Angle(playerDirection, enemyForward);

        if (angle > 4.0f)
        {
            RotateTowards(player.transform.position);
        }
        else if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Stab Attack");
        }
    }

    // Called to end an attack
    private void EndAttack()
    {
        animator.ResetTrigger("Stab Attack");
        isAttacking = false;
    }

    // Rotate towards taget
    // Found on the world wide web: https://answers.unity.com/questions/540120/how-do-you-update-navmesh-rotation-after-stopping.html
    private void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - this.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3.5f);
    }

    //
    //  STATE
    //

    // Set the state of the enemy
    public override void SetState(EnemyState newState)
    {
        if (currentState != EnemyState.Dead && currentState != newState)
        {
            lastState = currentState;
            currentState = newState;
        }
    }

    public override void SetFormationPosition(Vector3 pos)
    {
        
    }

    //
    //  HEALTH AND DEATH
    //
    #region Health&Death
    public override void Damage(int damage)
    {
        if (currentState != EnemyState.Dead)
        {
            navMeshAgent.updateRotation = false;
            Vector3 direction = (player.transform.position - this.transform.position);
            navMeshAgent.velocity = -direction.normalized * 5;

            health -= damage;
            if (health <= 0)
            {
                Death();
            }
            else
            {
                if (isAttacking)
                {
                    EndAttack();
                }
                GameObject damageFX = Instantiate(damageFXPrefab, this.transform.position + new Vector3(0, 2, 0), this.transform.rotation) as GameObject;

                animator.SetTrigger("Take Damage");
                SetState(EnemyState.Stunned);
            }
        }
    }

    public override void Death()
    {
        if (currentState != EnemyState.Dead)
        {
            navMeshAgent.enabled = false;
            SetState(EnemyState.Dead);
            this.enabled = false;
            animator.SetTrigger("Death");
            audioSource.PlayOneShot(deathAudio, 0.3f);
            scoreScript.updateScore(score);
            enemySpawn.removeEnemy(gameObject);
        }
    }
    #endregion

    //
    //  ANIMATION EVENTS
    //
    #region AnimationEvents
    // Attack Event - Player when the attack animation hits its peak
    protected void PinAttackEvent()
    {
        // Get distance to player
        float distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);

        Vector3 enemyForward = this.transform.forward;
        Vector3 playerDirection = player.transform.position - this.transform.position;
        float angle = Vector3.Angle(playerDirection, enemyForward);

        if (angle <= 20.0f && distanceToPlayer <= navMeshAgent.stoppingDistance + 1f)
        {
            // Do damage to the player
            PlayerStatus playerS = player.GetComponent<PlayerStatus>();
            playerS.takeDamage(attackStrength, transform.forward);
        }
    }

    protected void PinAttackEndEvent()
    {
        EndAttack();
    }

    public void PinDestroyEvent()
    {
        // Instantiate a scrap at the death location (height will be moved to the ground via a script on the scrap) and at a random rotation
        Instantiate(scrapPrefab, this.transform.position, Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));

        GameObject destoryFX = Instantiate(destroyFXPrefab, this.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        Destroy(this.gameObject);
    }

    public void PinStunnedEndEvent()
    {
        navMeshAgent.updateRotation = true;
        SetState(lastState);
    }
    #endregion
}

