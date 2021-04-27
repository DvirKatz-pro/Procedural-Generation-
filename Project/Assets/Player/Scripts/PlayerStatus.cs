using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

//player status controller - Dvir
// Some contributions by Cal
public class PlayerStatus : MonoBehaviourPunCallbacks
{
    //UI objects we need
    private Image healthBar;
    private Image firstAbilityTier;
    private Image secondAbilityTier;
    private Image thirdAbilityTier;
    private Image hurtImage;
    private Image scrapBar;
    [SerializeField] private ParticleSystem healthParticles;
    [SerializeField] private float health = 100;
    [SerializeField] private float abilityPoints = 100;
    private Color flashColor = Color.red;
    private float flashSpeed = 5;

    private float currentHealth;
    private float changeTime = 0;
    private float healthChangeTime = 50;
    private float setTime = 0;
    private float setAp = 0;
    //a code thats used by other scripts 
    [HideInInspector] public int unblockable = 200;

    private float currentAp;
    private Animator animator;

    // Audio
    private AudioSource audioSource;
    public AudioClip deathAudio;

    bool isDead = false;
    bool isDamaged = false;
    bool gainHealth = false;
    public bool isImmortal = false;

    // UI
    private UIManager uiManagerScript;
    private UIElements uiElements;



    // Start is called before the first frame update
    void Start()
    {
        // UI Components
        uiManagerScript = GameObject.Find("GameCanvas").GetComponent<UIManager>();
        if (uiManagerScript == null)
            Debug.LogError("UIManager to pause menu manager on " + this.name + " is null.");

        uiElements = GameObject.Find("GameCanvas").GetComponent<UIElements>();
        if (uiManagerScript == null)
            Debug.LogError("UIElements to pause menu manager on " + this.name + " is null.");

        healthBar = uiElements.healthBar;
        firstAbilityTier = uiElements.firstAbilityTier;
        secondAbilityTier = uiElements.secondAbilityTier;
        thirdAbilityTier = uiElements.thirdAbilityTier;
        hurtImage = uiElements.hurtImage;
        scrapBar = uiElements.scrapBar;

        // Other

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        firstAbilityTier.color = Color.blue;
        secondAbilityTier.color = Color.blue;

        firstAbilityTier.fillAmount = 0;
        secondAbilityTier.fillAmount = 0;
        thirdAbilityTier.fillAmount = 0;
        scrapBar.fillAmount = 0;
        currentHealth = health;
        changeAp(abilityPoints);
    }

    // Update is called once per frame
    void Update()
    {
        //make sure this dosent trigger again if the player is dead
        if (!isDead)
        {
            if (currentHealth <= 0)
            {
                //disable player controls
                isDead = true;
                audioSource.PlayOneShot(deathAudio, 0.3f);
                GetComponent<CombatController>().enabled = false;
                GetComponent<PlayerMovement>().enabled = false;
                animator.SetTrigger("Death");
                GetComponent<PlayerStatus>().enabled = false;
                hurtImage.color = Color.clear;
                uiManagerScript.UpdateState(UIManager.UIState.Death);

                if (!PhotonNetwork.OfflineMode)
                {
                    this.gameObject.GetComponent<PhotonView>().RPC("PlayerDiedRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID);
                }
            }
            if (isDamaged && !isDead)
            {
                hurtImage.color = flashColor;
            }
            else
            {
                hurtImage.color = Color.Lerp(hurtImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }
            isDamaged = false;
        }
        //set the ability points over time, to accuratly display rate of change
        if (changeTime > 0)
        {
            changeTime -= Time.deltaTime;
            float rateOfChange = (setAp / setTime) * (setTime - changeTime);
            float change = currentAp + rateOfChange;
            gainSpeicalAbility(change);
            if (changeTime <= 0)
            {
                currentAp = 0;
            }
        }

        if (gainHealth)
        {
            //if health is full, stop healing
            if (currentHealth >= health)
            {
                currentHealth = health;
                gainHealth = false;
                healthChangeTime = 50;
                healthParticles.Stop();
            }
            //otherwise gain health for 50 secounds at most (more than enough to get to full hp)
            else
            {
                if (healthChangeTime > 0)
                {
                    if (healthParticles.isStopped)
                    {
                        healthParticles.Play();
                    }
                    healthChangeTime -= Time.deltaTime;
                    currentHealth += 0.5f;
                    healthBar.fillAmount = currentHealth / health;
                }

            }
        }

    }
    /**
     * given a damage amount and a enemy direction, find out if the player is blocking or not, if not then the player takes damage
     */
    public void takeDamage(float damage, Vector3 enemyDirection)
    {
        //the player takes damage unless he is blocking
        float angle = Vector3.Dot(enemyDirection, this.transform.forward);
        if (!GetComponent<PlayerBlock>().isBlocking(angle) && !isImmortal)
        {
            healthBar.fillAmount -= damage / health;
            currentHealth -= damage;
            isDamaged = true;
        }

    }
    //check if the player takes damage
    public void takeDamage(float damage, Vector3 enemyDirection, int mode)
    {
        //check if attack is unblockable
        if (mode == unblockable && !isImmortal)
        {
            healthBar.fillAmount -= damage / health;
            currentHealth -= damage;
            isDamaged = true;

        }
        else
        {
            float angle = Vector3.Dot(enemyDirection, this.transform.forward);
            if (!GetComponent<PlayerBlock>().isBlocking(angle) && !isImmortal)
            {
                healthBar.fillAmount -= damage / health;
                currentHealth -= damage;
                isDamaged = true;
            }
        }
    }
    //set the gain health in update
    public void setGainHealth(bool gain)
    {
        gainHealth = gain;
    }
    //change the amount of an ability bar 
    void gainSpeicalAbility(float m_currentAp)
    {
        float ap = m_currentAp / 100.0f;
        if (ap <= 1)
        {
            firstAbilityTier.fillAmount = ap;
            secondAbilityTier.fillAmount = 0;
            thirdAbilityTier.fillAmount = 0;
        }
        if (ap <= 2 && ap >= 1)
        {
            firstAbilityTier.fillAmount = 1;
            secondAbilityTier.fillAmount = ap - 1;
            thirdAbilityTier.fillAmount = 0;
        }
        if (ap <= 3 && ap >= 2)
        {
            firstAbilityTier.fillAmount = 1;
            secondAbilityTier.fillAmount = 1;
            thirdAbilityTier.fillAmount = ap - 2;
        }




    }

    //change the amount of scrap
    public void changeScrap(int scrap)
    {
        float scrapFactor = scrap / 100.0f;
        scrapBar.fillAmount = scrapFactor;
    }
    //change the amount of ability points 
    public void changeAp(float ap)
    {
        currentAp += ap;
        if (currentAp >= 300)
        {
            currentAp = 300;
        }
        gainSpeicalAbility(currentAp);
    }

    public void changeAp(float ap, float time)
    {
        changeTime = time;
        setTime = time;
        setAp = ap;
    }

    public float getAp()
    {
        return currentAp;
    }

    // A janky method to revive the player, written by Cal
    // Not used anymore
    public void Revive()
    {
        isDead = false;
        GetComponent<CombatController>().enabled = true;
        GetComponent<PlayerMovement>().enabled = true;
        animator.ResetTrigger("Death");
        animator.SetBool("Idle", true);
        GetComponent<PlayerStatus>().enabled = true;
        uiManagerScript.UpdateState(UIManager.UIState.Game);
    }

    // Pun RPCs
    [PunRPC]
    void HurtPlayerRPC(float damage, Vector3 enemyDirection, int viewID)
    {
        // I am the player thats supposed to take damage
        if (GetComponent<PhotonView>().IsMine && GetComponent<PhotonView>().ViewID == viewID)
        {
            takeDamage(damage, enemyDirection);
        }
    }
}
