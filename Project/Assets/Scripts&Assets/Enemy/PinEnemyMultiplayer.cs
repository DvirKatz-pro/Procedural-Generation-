using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

// PinEnemyMultiplayer
// Pin enemy for use in multiplayer
// !!! NOT COMPLETE !!!
//
// Written by: Cal
public class PinEnemyMultiplayer : EnemyMultiplayer
{
    #region Variables

    // Enemy
    public static EnemyMType enemyMType = EnemyMType.Close;
    [SerializeField] private GameObject scrapPrefab;
    [SerializeField] private GameObject damageFXPrefab;
    [SerializeField] private GameObject destroyFXPrefab;
    private Dictionary<int, float> spottedPlayers = new Dictionary<int, float>();
    private GameObject focusedPlayer;

    // Stats
    [SerializeField] private int attackStrength;
    [SerializeField] private float maxHealth;
    [SerializeField] private float score = 100;
    private float health;

    // Manager & Components
    EnemyAIManagerMultiplayer manager;
    GameManager gameManager;
    private Rigidbody enemyRB;

    // Navmesh
    private NavMeshAgent navMeshAgent;

    // State
    private EnemyState currentState;
    private EnemyState lastState;
    private bool isAttacking;
    private bool isMoving;

    // Animator and AudioSource / Sounds
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource audioSource;
    [SerializeField] private AudioClip deathAudio;

    #endregion

    #region Main

    // Iniatlizes enemy: registers itself with AI manager, 
    protected void Awake()
    {
        // Register myself with the AI manager
        manager = GameObject.Find("GameManager").GetComponent<EnemyAIManagerMultiplayer>();
        if (manager == null)
            Debug.LogError(this.name + " cannot find the Enemy AI Manager Multiplayer on Game Manager");
        else
            manager.Register(this, enemyMType);

        // Add game manager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager == null)
            Debug.LogError(this.name + " cannot find the Game Manager");
    }

    // Initialize the enemy
    protected override void Start()
    {
        // Get the components
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyRB = GetComponent<Rigidbody>();
        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();

        SetState(EnemyState.Idle);
        health = maxHealth;
        isAttacking = false;
        isMoving = false;
    }

    // The update method
    protected override void Update()
    {
        if (currentState != EnemyState.Dead && PhotonNetwork.IsMasterClient)
        {
            // Update animator based on movement speed
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

            if (currentState != EnemyState.Stunned)
            {
                // Check all players to see if were in range to spot them, if we are and they havent been spotted then spot them
                foreach (int viewID in gameManager.playerPhotonViewIDs)
                {
                    GameObject currentPlayer = PhotonNetwork.GetPhotonView(viewID).gameObject;
                    if (currentPlayer)
                    {
                        float distance = Vector3.Distance(this.transform.position, currentPlayer.transform.position);
                        if (distance < 15f && !manager.GetPlayersSpotted().Contains(viewID))
                        {
                            manager.PlayerSpottedAlert(viewID);
                        }
                    }
                }

                if (focusedPlayer != null)
                {
                    Attack(focusedPlayer);
                }
            }
        }
    }

    // Returns current enemy type
    public override EnemyMType GetEnemyType()
    {
        return enemyMType;
    }

    #endregion

    #region Player Related Functions

    public override float DistanceToPlayer(int viewID)
    {
        return Vector3.Distance(this.transform.position, PhotonNetwork.GetPhotonView(viewID).gameObject.transform.position);
    }

    public override void SetFocusPlayer(int viewID)
    {
        focusedPlayer = PhotonNetwork.GetPhotonView(viewID).gameObject;
    }

    #endregion

    #region Attack and Related Functions

    protected override void Attack(GameObject playerToAttack)
    {
        if (Vector3.Distance(this.transform.position, playerToAttack.transform.position) > 4.0f)
        {
            SetDestination(playerToAttack.transform.position, 3.5f);
            return;
        }

        // Get angle between enemy and player
        Vector3 enemyForward = this.transform.forward;
        Vector3 playerDirection = playerToAttack.transform.position - this.transform.position;
        float angle = Vector3.Angle(playerDirection, enemyForward);

        if (angle > 4.0f)
        {
            RotateTowards(playerToAttack.transform.position);
        }
        else if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Stab Attack");
        }
    }

    // Rotate towards taget
    // Found on the world wide web: https://answers.unity.com/questions/540120/how-do-you-update-navmesh-rotation-after-stopping.html
    private void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - this.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3.5f);
    }

    #endregion

    #region Health and death

    // Damage the enemy
    public override void Damage(int damageAmount)
    {
        // NOT IMPLEMENTED YET
    }

    // Enemy death
    public override void Death()
    {
        // NOT IMPLEMENTED YET
    }

    #endregion

    #region States

    // Set the state
    public override void SetState(EnemyState newState)
    {
        if (currentState != EnemyState.Dead && currentState != newState)
        {
            lastState = currentState;
            currentState = newState;
        }
    }

    // Set the destination
    protected override void SetDestination(Vector3 position, float stoppingDistance)
    {
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.SetDestination(position);
    }

    #endregion

    // End the attack
    private void EndAttack()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            animator.ResetTrigger("Stab Attack");
            isAttacking = false;
        }
    }

    #region AnimationEvents

    // Attack event - Triggers when the attack animation hits its peak
    protected void PinAttackEvent()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Get distance to player
            float distanceToPlayer = Vector3.Distance(this.transform.position, focusedPlayer.transform.position);

            Vector3 enemyForward = this.transform.forward;
            Vector3 playerDirection = focusedPlayer.transform.position - this.transform.position;
            float angle = Vector3.Angle(playerDirection, enemyForward);

            if (angle <= 20.0f && distanceToPlayer <= navMeshAgent.stoppingDistance + 1f)
            {
                // Do damage to the player
                gameManager.GetPlayer().GetComponent<PhotonView>().RPC("HurtPlayerRPC", RpcTarget.All, (float)attackStrength, transform.forward, focusedPlayer.GetComponent<PhotonView>().ViewID);
                Debug.Log("Pin Enemy: I have hurt the player!");
            }
        }
    }

    // Attack end event - Triggers when the attack animation finishes
    protected void PinAttackEndEvent()
    {
        EndAttack();
    }

    // Destory event - Triggers when the death animation finishes
    public void PinDestroyEvent()
    {
        // Instantiate a scrap at the death location (height will be moved to the ground via a script on the scrap) and at a random rotation
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateRoomObject(scrapPrefab.name, this.transform.position, Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
            PhotonNetwork.Destroy(this.gameObject);
        }
        GameObject destoryFX = Instantiate(destroyFXPrefab, this.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
    }

    // Stunned end event - Triggers when the stunned animation finishes playing
    public void PinStunnedEndEvent()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            navMeshAgent.updateRotation = true;
            SetState(lastState);
        }
    }

    #endregion
}
