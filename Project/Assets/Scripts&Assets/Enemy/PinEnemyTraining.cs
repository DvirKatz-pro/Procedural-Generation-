using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// PinEnemyTraining
// Used as a dummy pin in the training level
//
// Written by: Cal
public class PinEnemyTraining : MonoBehaviour
{
    #region Variables

    // Enemy
    [SerializeField] private GameObject scrapPrefab;
    [SerializeField] private GameObject damageFXPrefab;
    [SerializeField] private GameObject destroyFXPrefab;

    // Player
    private GameObject player;

    // Stats
    [SerializeField] private float maxHealth;
    private float health;
    public bool combatAttackable;

    //Components
    private Rigidbody enemyRB;

    // State
    public bool isDead;

    // Animator and AudioSource / Sounds
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource audioSource;
    [SerializeField] private AudioClip deathAudio;

    #endregion

    #region Main

    // Initialize the enemy
    void Start()
    {
        // Get the components
        enemyRB = GetComponent<Rigidbody>();
        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();
        player = GameObject.Find("Player");

        isDead = false;
        health = maxHealth;
    }

    #endregion

    #region Healt and Death

    // Damage the enemy
    public void Damage(int damage)
    {
        if (!isDead)
        {
            health -= damage;
            if (health <= 0)
            {
                Death();
            }
            else
            {
                GameObject damageFX = Instantiate(damageFXPrefab, this.transform.position + new Vector3(0, 2, 0), this.transform.rotation) as GameObject;
                animator.SetTrigger("Take Damage");
            }
        }
    }

    // Enemy death
    public void Death()
    {
        isDead = true;
        this.enabled = false;
        animator.SetTrigger("Death");
        audioSource.PlayOneShot(deathAudio, 0.3f);
    }

    #endregion

    #region Animation Events

    // Destory event - Triggers when the death animation finishes
    public void PinDestroyEvent()
    {
        // Instantiate a scrap at the death location (height will be moved to the ground via a script on the scrap) and at a random rotation
        Instantiate(scrapPrefab, this.transform.position, Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));

        GameObject destoryFX = Instantiate(destroyFXPrefab, this.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        Destroy(this.gameObject);
    }

    // Stunned end event - Triggers when the stunned animation finishes playing
    public void PinStunnedEndEvent()
    {
        // Do nothing
    }

    #endregion
}
