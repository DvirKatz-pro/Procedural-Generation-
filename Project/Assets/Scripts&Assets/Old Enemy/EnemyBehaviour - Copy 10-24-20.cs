using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// [NO LONGER IN USE]
// EnemyBehaviourCopy1
//
// Written by: Cal
public class EnemyBehaviourCopy1 : MonoBehaviour
{
    // Player transform
    [SerializeField]
    GameObject player;

    // Distances
    [SerializeField]
    private float distanceToAttack;

    [SerializeField]
    private float distanceToChase;

    // Nav mesh agent
    NavMeshAgent navMeshAgent;

    // Animator
    Animator animator;

    // State
    private enum State { Idle, Chasing, Attacking, Dead };
    State currentState;

    // Enemy
    [SerializeField]
    float maxHealth;

    float currentHealth;

    [SerializeField]
    float attackStrength;

    // Audio
    private AudioSource audioSource;
    public AudioClip deathAudio;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != State.Dead)
        {
            Vector3 playerPosition = player.transform.position;
            Vector3 myPosition = this.transform.position;
            float distanceToPlayer = Vector3.Distance(myPosition, playerPosition);

            switch (currentState)
            {
                case State.Idle:
                    if (distanceToPlayer <= distanceToAttack)
                    {
                        Attack();
                    }
                    else if (distanceToPlayer <= distanceToChase)
                    {
                        animator.SetBool("Dash Forward", true);
                        Chase(playerPosition);
                    }
                    break;

                case State.Chasing:
                    if (distanceToPlayer <= distanceToAttack)
                    {
                        Attack();
                    }
                    else if (distanceToPlayer <= distanceToChase)
                    {
                        animator.SetBool("Dash Forward", true);
                        Chase(playerPosition);
                    }
                    else
                    {
                        animator.SetBool("Dash Forward", false);
                        currentState = State.Idle;
                    }
                    break;

                case State.Attacking:
                    break;

                default:
                    Debug.LogError(this.name + " has no state.");
                    break;

            }
        }
    }

    public void Hurt(int damage)
    {
        if (currentState != State.Dead)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                ResetAnimationTriggers();
                animator.SetTrigger("Death");
                currentState = State.Dead;
                audioSource.PlayOneShot(deathAudio, 0.3f);
            }
            else
            {
                ResetAnimationTriggers();
                animator.SetTrigger("Take Damage");
                currentState = State.Idle;
            }
        }
    }

    private void Attack()
    {
        ResetAnimationTriggers();
        currentState = State.Attacking;
        animator.SetTrigger("Stab Attack");
    }

    private void Chase(Vector3 playerPosition)
    {
        currentState = State.Chasing;
        navMeshAgent.SetDestination(playerPosition);
    }

    public void EnemyDestroyEvent()
    {
        Destroy(this.gameObject);
    }

    public void EnemyAttackEvent()
    {
        // Get distance to player
        float distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);

        Vector3 enemyForward = this.transform.forward;
        Vector3 playerDirection = player.transform.position - this.transform.position;
        float angle = Vector3.Angle(playerDirection, enemyForward);

        if (angle <= 20.0f && distanceToPlayer <= 3.0f)
        {
            // Do damage to the player
            PlayerStatus playerS = (PlayerStatus)player.GetComponent(typeof(PlayerStatus));
            playerS.takeDamage(attackStrength, transform.forward);
        }
    }

    public void EnemyIdleEvent()
    {
        currentState = State.Idle;
        animator.ResetTrigger("Stab Attack");
    }

    private void ResetAnimationTriggers()
    {
        animator.ResetTrigger("Stab Attack");
        animator.ResetTrigger("Take Damage");
    }
}
