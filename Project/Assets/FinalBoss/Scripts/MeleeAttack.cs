using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//melee attack for boss - Dvir
public class MeleeAttack : MonoBehaviour
{
    //needed script refernces
    SphereAttack sphere;
    FirstBossEnemy controller;
    
    private Rigidbody rb;

    //gameplay related values
    [SerializeField] private float waitTime;
    [SerializeField] private float healthLimit;
    [SerializeField] private float meleeTime;
    private float currentTime;
    private float currentHealth;

    //needed gameobjects and particle systems
    [SerializeField] private GameObject player;
    [SerializeField] private List<ParticleSystem> jets;
    [SerializeField] private GameObject portal;
    [SerializeField] private GameObject shieldLoss;
    [SerializeField] private GameObject shield;

    //audio clips
    [SerializeField] private AudioClip shieldLossSound;
    [SerializeField] private AudioClip shieldRecharge;
    [SerializeField] private AudioClip teleportSound;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphere = GetComponent<SphereAttack>();
        controller = GetComponent<FirstBossEnemy>();
        audioSource = GetComponent<AudioSource>();
        currentTime = waitTime;
        currentHealth = controller.currentHealth;
    }

    /*
     * teleport to 4 positions around the player and dash at him
    */
   public IEnumerator Attack()
    {
        yield return new WaitForSeconds(1.0f);
        //start at a random position, then teleport clockwise based on that initial position
        int randPosition = Random.Range(0, 3);
        float elapsedTime = 0;
        for (int i = 0; i < 4; i++)
        {
            Vector3 position = player.transform.position;
            position.y += 0.5f;
            Vector3 finalPos = position;
            switch (randPosition)
            {
                case 0:
                    {
                      
                        position.z += 3.5f;
                        finalPos.z -= 3.5f;
                       
                        break;
                    }
                case 1:
                    {

                       
                        position.x += 3.5f;
                        finalPos.x -= 3.5f;
                        break;
                    }
                case 2:
                    {
                        
                        position.z -= 3.5f;
                        finalPos.z += 3.5f;
                      
                        break;
                    }
                case 3:
                    {


                       
                        position.x -= 3.5f;
                        finalPos.x += 3.5f;
                        
                       
                        randPosition = 0;
                        elapsedTime = 0;
                        break;
                    }
            }
            //Instantiae one teleport at current position and another at future position
            Vector3 portalPosition = this.transform.position;
            portalPosition.y += 1.7f;
            portalPosition.x -= 0.5f;
            portalPosition.z -= 0.5f;
            GameObject portalInstance = Instantiate(portal, portalPosition, Quaternion.identity);
            ParticleSystem particle = portalInstance.GetComponent<ParticleSystem>();
            audioSource.PlayOneShot(teleportSound, 0.5f);
            particle.Play();
            yield return new WaitForSeconds(0.1f);
            portalPosition = position;
            portalPosition.y += 1.7f;
            portalPosition.x -= 0.5f;
            portalPosition.z -= 0.5f;
            GameObject otherPortalInstance = Instantiate(portal, portalPosition, Quaternion.identity);
            ParticleSystem otherParticle = otherPortalInstance.GetComponent<ParticleSystem>();
            otherParticle.Play();
            yield return new WaitForSeconds(0.3f);
            
            this.transform.position = position;
            this.transform.LookAt(finalPos);
            
            
            yield return new WaitForSeconds(0.2f);
            particle.Stop();
            otherParticle.Stop();
            Destroy(portalInstance);
            Destroy(otherPortalInstance);

            //dash at player
            foreach (ParticleSystem sys in jets)
            {            
                sys.Play();
            }
            
            while (elapsedTime < meleeTime)
            {
                transform.position = Vector3.Lerp(position, finalPos, (elapsedTime / meleeTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            foreach (ParticleSystem sys in jets)
            {
                sys.Stop();
            }
            yield return new WaitForSeconds(0.2f);
            elapsedTime = 0;
            randPosition++;
        }
        //disable trigger collider, making player/boss collision possible
        GetComponent<Collider>().enabled = true;
        foreach (Transform tr in transform)
        {
            if (tr.gameObject.name == "TriggerCollider")
            {
                tr.gameObject.SetActive(false);
            }
        }
        //make boss mortal, allowing the player to hit him for a certain period of time  
        rb.useGravity = true;
        rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
        shieldLoss.GetComponent<ParticleSystem>().Play();
        audioSource.PlayOneShot(shieldLossSound, 0.5f);
        controller.isImmortal = false;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            //if the player deals too much damage to boss, then the boss should stop waiting
            if (currentHealth - controller.currentHealth > healthLimit)
            {
                currentTime = 0;              
            }
            yield return new WaitForEndOfFrame();
        }
        //regain boss immortality
        currentHealth = controller.currentHealth;
        controller.isImmortal = true;
        if (currentHealth > 0)
        {
            //enable particles to show boss regaining immortality and tell the boss to activate sphere attack
            currentTime = waitTime;          
            shield.GetComponent<ParticleSystem>().Play();
            audioSource.PlayOneShot(shieldRecharge, 0.5f);
            rb.useGravity = false;
            yield return new WaitForSeconds(3);

            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX;
            sphere.setAttack(true);
        }

    }

}