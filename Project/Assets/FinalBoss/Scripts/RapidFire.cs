using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//rapid fire attack for boss - Dvir
public class RapidFire : MonoBehaviour
{
    //set refernceses to player,bullet,bullet speed and the particle effects
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpeed = 5;
    [SerializeField] private List<ParticleSystem> gunEffects;
    private bool isAttacking = false;

    //set references to audio components 
    [SerializeField] private AudioClip fireSound;
    private AudioSource audioSource;
    
    //Animator reference 
    private Animator animator;
    //script reference
    FirstBossEnemy controller;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<FirstBossEnemy>();
    }
    void Update()
    {
        //if boss is attacking, continuously look at the player
        if (isAttacking)
        {
            Quaternion q = Quaternion.LookRotation(player.transform.position - transform.position);
            this.transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 1000 * Time.deltaTime);
        }
    }
    /*
     * rapid fire attack on the player
     */
    public IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("BurstAim/ArcAim");
        //play the power up particle effects
        foreach (ParticleSystem p in gunEffects)
        {
            p.Play();
        }
        yield return new WaitForSeconds(1);
        audioSource.PlayOneShot(fireSound, 0.5f);
        yield return new WaitForSeconds(1);
        foreach (ParticleSystem p in gunEffects)
        {
            p.Stop();
        }

        animator.SetTrigger("BurstShot");
        //fire the bullets 
        for (int i = 0; i < 4; i++)
        {
        
            Vector3 position = transform.position;
            position.y += 1.5f;
            position = position + transform.forward * 2;

            
            GameObject aim = Instantiate(bullet, position, Quaternion.identity);
            aim.transform.rotation = Quaternion.LookRotation(transform.forward);
            aim.GetComponent<Bullet>().setSpeed(bulletSpeed);
            yield return new WaitForSeconds(0.3f);
         
        }
        isAttacking = false;
        animator.SetTrigger("Idle");
        controller.setState(FirstBossEnemy.State.Decide);
    }

}
