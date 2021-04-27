using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// PunisherEnemy
// The base punisher for the farm level
//
// Written by: Cal
public class PunisherEnemy : Enemy
{
    #region Variables

    // Enemy
    public static EnemyType enemyType = EnemyType.Ranged;
    [SerializeField] private GameObject scrapPrefab;
    [SerializeField] private GameObject damageFXPrefab;
    [SerializeField] private GameObject destroyFXPrefab;

    // Player
    private GameObject player;

    // Stats
    [SerializeField] private int attackStrength;
    [SerializeField] private float maxHealth;
    [SerializeField] private float Score = 200;
    private float health;

    // Bullets
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private AudioClip laserShotSound;
    [SerializeField] float bulletSpeed;
    private GameObject bullet;
    private Vector3 shootPosition;
    private Vector3 positionToShoot;
    private int shotCount;
    private BulletCollider bulletCollider;
    private Transform bulletLocation1;
    private Transform bulletLocation2;
    private Transform bulletLocation3;
    private Transform bulletLocation4;
    [SerializeField] private float shotCooldown;
    private float shotTimeSinceLast;

    // Manager & Components
    EnemyAIManager manager;
    private Rigidbody enemyRB;

    // Navmesh
    private NavMeshAgent navMeshAgent;
    private Vector3 formationPosition;

    // State
    private EnemyState currentState;
    private EnemyState lastState;

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
        manager = GameObject.Find("GameManager").GetComponent<EnemyAIManager>();
        if (manager == null)
            Debug.LogError(this.name + " cannot find the EnemyAIManager on GameManager");
        else
            manager.Register(this, enemyType);
    }

    // Initialize the enemy
    protected override void Start()
    {
        // Get the components
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyRB = GetComponent<Rigidbody>();
        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();
        player = GameObject.Find("Player");

        SetState(EnemyState.Idle);
        health = maxHealth;
        shotTimeSinceLast = Time.time;

        // Bullet
        shotCount = 0;
        bulletLocation1 = this.transform.Find("BulletLocation1");
        bulletLocation2 = this.transform.Find("BulletLocation2");
        bulletLocation3 = this.transform.Find("BulletLocation3");
        bulletLocation4 = this.transform.Find("BulletLocation4");
    }

    protected override void Update()
    {
        // If bullet exists, move it
        if (bullet != null)
        {
            bullet.transform.position += positionToShoot * bulletSpeed * Time.deltaTime;
        }

        // Only update if the current enemy is not stunned or dead
        if (currentState != EnemyState.Stunned && currentState != EnemyState.Dead)
        {
            // Force an attack because the player is so close to me or attack because of state
            float distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);
            if (currentState == EnemyState.Attack)
            {
                Attack();
                return;
            }

            // Get into formation
            Vector3 relativeFormationPosition = player.transform.position + formationPosition * 14.0f;
            float distanceToPosition = Vector3.Distance(this.transform.position, relativeFormationPosition);
            if (currentState == EnemyState.Formation)
            {
                if (distanceToPosition <= 4.0f)
                {
                    Attack();
                }
                else
                {
                    SetDestination(relativeFormationPosition, 0.0f);
                }
                return;
            }

            // Idle
        }
    }

    // Returns current enemy type
    public override EnemyType GetEnemyType()
    {
        return enemyType;
    }

    #endregion

    #region Actions

    // Set the destination
    protected override void SetDestination(Vector3 position, float stoppingDistance)
    {
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.SetDestination(position);
    }

    // Attack
    protected override void Attack()
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) < 5.0f)
        {
            Vector3 direction = this.transform.position - player.transform.position;
            direction.Normalize();
            SetDestination(this.transform.position + direction * 5, 0.0f);
            return;
        }

        if (Vector3.Distance(this.transform.position, player.transform.position) > 15.0f)
        {
            SetDestination(player.transform.position, 15.0f);
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
        else if (Time.time - shotTimeSinceLast >= shotCooldown)
        {
            // If a bullet still exists should be well off screen by now so destroy it
            if (bullet != null)
                Destroy(bullet);

            // Cycle through the 4 bullet shot locations
            shotCount = shotCount % 4;
            switch (shotCount)
            {
                case 0:
                    shootPosition = bulletLocation1.position;
                    break;

                case 1:
                    shootPosition = bulletLocation2.position;
                    break;

                case 2:
                    shootPosition = bulletLocation3.position;
                    break;

                case 3:
                    shootPosition = bulletLocation4.position;
                    break;

                default:
                    Debug.LogError("Shot count is not in range for " + this.gameObject.name);
                    break;
            }
            shotCount++;

            // Instantiate a new bullet
            bullet = Instantiate(bulletPrefab, shootPosition, Quaternion.identity);
            bulletCollider = bullet.GetComponent<BulletCollider>();
            bulletCollider.setEnemyAttackScript(this);
            positionToShoot = player.transform.position - bullet.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
            audioSource.PlayOneShot(laserShotSound, 0.3f);

            shotTimeSinceLast = Time.time;
        }
    }

    // Called to end an attack
    private void EndAttack()
    {
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

    #region State

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
        formationPosition = pos;
    }

    #endregion

    #region Health and Death

    // Damage the enemy
    public override void Damage(int damage)
    {
        if (currentState != EnemyState.Dead)
        {
            navMeshAgent.updateRotation = false;
            Vector3 direction = (player.transform.position - this.transform.position);
            navMeshAgent.velocity = -direction.normalized * 3;

            health -= damage;
            if (health <= 0)
            {
                Death();
            }
            else
            {
                GameObject damageFX = Instantiate(damageFXPrefab, this.transform.position + new Vector3(0, 2, 0), this.transform.rotation);
                animator.SetTrigger("Take Damage");
                SetState(EnemyState.Stunned);
            }
        }
    }

    // Enemy death
    public override void Death()
    {
        manager.Unregister(this, enemyType);
        navMeshAgent.enabled = false;
        SetState(EnemyState.Dead);
        animator.SetTrigger("Die");
        audioSource.PlayOneShot(deathAudio, 0.3f);
    }

    #endregion

    #region Animation Events

    // Attack event - Triggers when the shot hits a target
    public void hitPlayer(GameObject gameObjectCollided)
    {
        if (gameObjectCollided.tag.Equals("Player"))
        {
            // Do damage to the player
            PlayerStatus playerS = player.GetComponent<PlayerStatus>();
            playerS.takeDamage(attackStrength, transform.forward);
        }
        else if (gameObjectCollided.tag.Equals("Enemy"))
        {
            // Do damage to the enemy
            Enemy enemyScript = gameObjectCollided.GetComponent<Enemy>();
            enemyScript.Damage(attackStrength);
        }
        Destroy(bullet);
    }

    // Destory event - Triggers when the death animation finishes
    public void PunisherDestroyEvent()
    {
        // Instantiate a scrap at the death location (height will be moved to the ground via a script on the scrap) and at a random rotation
        Instantiate(scrapPrefab, this.transform.position, Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));

        GameObject destoryFX = Instantiate(destroyFXPrefab, this.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        Destroy(this.gameObject);
    }

    // Stunned end event - Triggers when the stunned animation finishes playing
    public void PunisherStunnedEndEvent()
    {
        navMeshAgent.updateRotation = true;
        SetState(lastState);
    }

    #endregion
}
