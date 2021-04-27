using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//boss controller - Dvir
public class FirstBossEnemy : Enemy
{




    //references to gameobjects/components
    [HideInInspector] public Animator animator;
    [SerializeField] private AudioClip deathAudio;
    [HideInInspector] public AudioSource audioSource;
    [SerializeField] List<GameObject> bounds;
    [SerializeField] private Transform topRight;
    [SerializeField] private Transform bottomLeft;
    [SerializeField] private GameObject bossUI;
    [SerializeField] private GameObject bossArenaWall;
    [SerializeField] private Image healthBar;
    [SerializeField] private List<ParticleSystem> deathSparks;
    private GameObject player;

    //Gameplay related values
    [SerializeField] private float playerMinRadius;
    [SerializeField] private float playerMaxRadius;
    [SerializeField] private float waitTime = 5f;
    [SerializeField] private int burstChance;
    [SerializeField] private int meleeChance;
    [SerializeField] private int sniperChance;
    [SerializeField] private int sphereChance;
    [SerializeField] private int arcChance;
    [SerializeField] private int gurrantedMelee = 6;
    public float health;
    private float currentTime = 0;
    private int currentGurrante = 0;
    [HideInInspector] public float currentHealth;

    //bools that control states more precisly
    [HideInInspector] public bool isImmortal = true;
    private bool runCoroutine = true;
    private bool finishedCoroutine = false;
    private bool isAttacking = true;
    private bool isDead = false;
    private bool reinforce = true;

    [HideInInspector] public bool isTeleporting = false;
    [HideInInspector] public bool enableTeleport = false;


    //boss states
    public enum State
    {
        Nothing,
        Decide,
        Teleport,
        BurstShotAttack,
        ReinforceAttack,
        MeleeAttack,
        SniperAttack,
        ArcAttack,
        SphereAttack,
        RapidAttack,
        Reinforcements,
        Death
    }
    State currentState;

    //references to scripts
    MeleeAttack melee;
    Teleport teleport;
    ArcAttack arcAttack;
    SniperAttack sniperAttack;
    RapidFire rapidAttack;
    Reinforcement reinforcement;
    SphereAttack sphere;

    // UI Manager
    public GameObject gameCanvas;
    private UIManager uiManager;

    protected override void Start()
    {
        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        melee = GetComponent<MeleeAttack>();
        teleport = GetComponent<Teleport>();
        arcAttack = GetComponent<ArcAttack>();
        sniperAttack = GetComponent<SniperAttack>();
        rapidAttack = GetComponent<RapidFire>();
        reinforcement = GetComponent<Reinforcement>();
        sphere = GetComponent<SphereAttack>();
        currentHealth = health;

        currentState = State.Nothing;
        //calculate the probabilty as a number out of 100
        burstChance = 100 - (100 - burstChance);
        meleeChance = burstChance + meleeChance;
        sniperChance = burstChance + (meleeChance - burstChance) + sniperChance;
        arcChance = burstChance + (meleeChance - burstChance) + (sniperChance - meleeChance) + arcChance;
        currentTime = waitTime;

        uiManager = gameCanvas.GetComponent<UIManager>();
        if (uiManager == null)
            Debug.LogError(this.name + " has invalid UIManager.");
    }

    public void StartBoss()
    {
        StartCoroutine(WarmUp());
    }

    protected override void Update()
    {
        Attack();

    }

    protected override void Attack()
    {

        switch (currentState)
        {
            //decide what state to go to based on probability
            case (State.Decide):
                {

                    if (runCoroutine)
                    {
                        StartCoroutine(Pause());
                    }
                    else
                    {
                        //check if we are done pauseing
                        if (finishedCoroutine)
                        {
                            isAttacking = true;
                            //if we have went into x states without going into the melee attack state, then we go into the melee attack state
                            if (currentGurrante >= gurrantedMelee)
                            {
                                currentState = State.MeleeAttack;
                                currentGurrante = 0;
                                runCoroutine = true;
                            }
                            //if the boss health is at half of maximum, activate reinforcement ability
                            else if (currentHealth <= health / 2 && reinforce)
                            {
                                currentState = State.Reinforcements;
                                reinforce = false;
                            }
                            else
                            {
                                int randomNumber = Random.Range(0, 100);
                                if (randomNumber < burstChance)
                                {
                                    currentState = State.BurstShotAttack;
                                    currentGurrante++;
                                }
                                else if (randomNumber > burstChance && randomNumber < meleeChance)
                                {
                                    currentState = State.MeleeAttack;
                                    currentGurrante = 0;
                                }
                                else if (randomNumber > meleeChance && randomNumber < sniperChance)
                                {
                                    currentState = State.SniperAttack;
                                    currentGurrante++;
                                }
                                else if (randomNumber > sniperChance && randomNumber < arcChance)
                                {
                                    currentState = State.ArcAttack;
                                    currentGurrante++;
                                }
                                runCoroutine = true;
                            }
                        }
                        else
                        {
                            //rotate towards the player if we are pausing
                            Vector3 position = player.transform.position;

                            Vector3 targetDirection = this.transform.position - position;
                            targetDirection *= -1;
                            float singleStep = 5 * Time.deltaTime;

                            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

                            transform.rotation = Quaternion.LookRotation(newDirection);
                        }
                    }

                    break;
                }
            case (State.BurstShotAttack):
                {
                    if (isAttacking)
                    {
                        isAttacking = false;
                        StartCoroutine(rapidAttack.Attack());
                    }

                    break;
                }
            case (State.MeleeAttack):
                {
                    if (isAttacking)
                    {
                        isAttacking = false;
                        GetComponent<Collider>().enabled = false;
                        foreach (Transform tr in transform)
                        {
                            if (tr.gameObject.name == "TriggerCollider")
                            {
                                tr.gameObject.SetActive(true);
                            }
                        }
                        StartCoroutine(melee.Attack());
                    }

                    break;
                }
            case (State.SniperAttack):
                {
                    if (isAttacking)
                    {
                        isAttacking = false;
                        StartCoroutine(sniperAttack.Attack());
                    }

                    break;
                }
            case (State.ArcAttack):
                {
                    if (isAttacking)
                    {
                        isAttacking = false;
                        StartCoroutine(arcAttack.Attack());
                    }

                    break;
                }

            case (State.Teleport):
                {

                    break;
                }
            case (State.Death):
                {
                    if (runCoroutine)
                    {
                        StartCoroutine(DeathRoutine());
                    }
                    break;
                }

            case (State.Reinforcements):
                {
                    if (isAttacking)
                    {
                        isAttacking = false;
                        StartCoroutine(reinforcement.Reinforce());
                    }
                    break;
                }
        }



    }

    /*
     * Check if the boss can take damage, and is so, deal it
     * damage - amount of damage to take
     */
    public override void Damage(int damage)
    {
        if (!isImmortal)
        {
            currentHealth = currentHealth - damage;
            healthBar.fillAmount = currentHealth / health;
            if (currentHealth <= 0)
            {
                Death();
            }
        }
    }

    /*
     * check if boss is dead, and set state to death state
     */
    public override void Death()
    {
        if (!isDead)
        {
            isDead = true;
            animator.SetTrigger("Death");
            melee.enabled = false;
            sphere.enabled = false;
            currentState = State.Death;
            runCoroutine = true;
        }


    }
    /*
     * disable everything
     */
    IEnumerator DeathRoutine()
    {
        runCoroutine = false;
        yield return new WaitForSeconds(1);
        int playParticle = Random.Range(0, deathSparks.Count);
        deathSparks[playParticle].Play();
        audioSource.PlayOneShot(deathAudio, 0.2f);
        yield return new WaitForSeconds(0.5f);
        healthBar.gameObject.SetActive(false);
        deathSparks[playParticle].Stop();
        runCoroutine = true;
        uiManager.UpdateState(UIManager.UIState.Win);
    }
    /*
     * set the current state of the boss
     */
    public void setState(State state)
    {
        currentState = state;
    }
    /*
     * make the boss pause for a little bit before doing anything else
     */
    IEnumerator Pause()
    {
        enableTeleport = true;
        isTeleporting = true;
        teleport.teleport(playerMaxRadius, playerMinRadius);
        runCoroutine = false;
        finishedCoroutine = false;
        while (currentTime > 0)
        {
            if (isTeleporting)
            {
                enableTeleport = false;
            }
            else
            {
                if (Vector3.Distance(this.transform.position, player.transform.position) < 3.0f)
                {
                    enableTeleport = true;
                    teleport.teleport(playerMaxRadius, playerMinRadius);
                }
            }

            yield return new WaitForSeconds(Time.deltaTime);
            currentTime -= Time.deltaTime;
        }

        currentTime = waitTime;
        finishedCoroutine = true;
    }
    /*
     * initialize boss before starting
     */
    public IEnumerator WarmUp()
    {
        GameObject.Find("Main Camera").GetComponent<CameraFollow>().setPan(true);
        healthBar.gameObject.SetActive(true);
        bossUI.SetActive(true);
        bossArenaWall.SetActive(true);
        yield return new WaitForSeconds(5);
        currentState = State.Decide;
    }
    
    //not using these methods
    #region Unused

    public override void SetFormationPosition(Vector3 pos)
    {

    }

    public override EnemyType GetEnemyType()
    {

        return EnemyType.Close;

    }

    public override void SetState(EnemyState newState)
    {

    }
    protected override void SetDestination(Vector3 position, float stoppingDistance)
    {
    }
    #endregion
}
