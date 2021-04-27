using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereAttack : MonoBehaviour
{
    [SerializeField] private List<GameObject> shields;
    public float expansionTime = 0.5f;
    [SerializeField] private float disapationTime = 0.5f;
    public float expansionRate = 0.2f;
    [SerializeField] private float disapationnRate = 0.2f;
    private float currentExplosionTime = 0;
    private float currentDisapationTime = 0;
    private bool isAttacking = false;
    private Vector3 ogScale;
    private FirstBossEnemy controller;

    [SerializeField] private AudioClip sphereExplosion;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        currentExplosionTime = expansionTime;
        currentDisapationTime = disapationTime;
        ogScale = shields[0].transform.localScale;
        controller = GetComponent<FirstBossEnemy>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking)
        {
            foreach (GameObject g in shields)
            {
                if (!g.activeSelf)
                {
                    g.SetActive(true);
                    g.GetComponent<ParticleSystem>().Play();
                    audioSource.PlayOneShot(sphereExplosion, 0.3f);
                    g.GetComponent<SphereDamage>().playerHit = false;
                    StartCoroutine(Wait());
                }             
            }
            /*

            if (currentExplosionTime > 0)
            {

                currentExplosionTime -= Time.deltaTime;
                foreach (GameObject g in shields)
                {
                    if (!g.activeSelf)
                    {
                        g.SetActive(true);
                        g.GetComponent<ParticleSystem>().Play();
                        audioSource.PlayOneShot(sphereExplosion, 0.3f);
                    }
                    g.transform.localScale = new Vector3(g.transform.localScale.x + expansionRate, g.transform.localScale.y + expansionRate, g.transform.localScale.z + expansionRate);
                }
                
            }
            else
            {
                if (currentDisapationTime > 0)
                {
                    currentDisapationTime -= Time.deltaTime;
                    foreach (GameObject g in shields)
                    {
                        g.transform.localScale = new Vector3(g.transform.localScale.x + disapationnRate, g.transform.localScale.y + disapationnRate, g.transform.localScale.z + disapationnRate);
                    }
                }
                else
                {
                    isAttacking = false;
                    currentExplosionTime = expansionTime;
                    currentDisapationTime = disapationTime;
                    foreach (GameObject g in shields)
                    {
                        g.SetActive(false);
                        g.transform.localScale = ogScale;
                        g.GetComponent<ParticleSystem>().Stop();
                    }
                    controller.setState(FirstBossEnemy.State.Decide);
                }
             

            }
            */

        }
       
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        isAttacking = false;
        foreach (GameObject g in shields)
        {
            g.SetActive(false);
           // g.transform.localScale = ogScale;
            g.GetComponent<ParticleSystem>().Stop();
        }
        controller.setState(FirstBossEnemy.State.Decide);
    }
    public void setAttack(bool state)
    {
        isAttacking = state;
    }

}
