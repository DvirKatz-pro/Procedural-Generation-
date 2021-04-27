using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Boss arc attack - Dvir
public class ArcAttack : MonoBehaviour
{
    //set refernces to player,line and bullet objects
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject line;
    [SerializeField] private GameObject bullet;

    //set references to audio and animation components 
    [SerializeField] private AudioClip fireSound;
    private AudioSource audioSource;
    private Animator animator;
    private FirstBossEnemy controller;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        controller = GetComponent<FirstBossEnemy>();
    }
    public IEnumerator Attack()
    {
        //set line angles and references 
        List<GameObject> lines = new List<GameObject>();
        this.transform.LookAt(player.transform.position);
        yield return new WaitForSeconds(0.5f);
        float startingAngle = 50.0f;
        float angle = startingAngle;
        Vector3 position;
        animator.SetTrigger("BurstAim/ArcAim");
        //create lines at angles  
        for (int i = 0; i < 10; i++)
        {
            position = transform.position;
            position.y += 1.5f;
            position = position + (Quaternion.Euler(0, angle, 0) * transform.forward) * 27;

            Quaternion rotation = Quaternion.Euler(0, 90 + angle, 0) * transform.rotation;

            GameObject aim = Instantiate(line, position, rotation);
            angle -= (startingAngle * 2) / 8.0f;
            lines.Add(aim);
        }
        yield return new WaitForSeconds(1f);
        //change line color
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.1f);
            foreach (GameObject g in lines)
            {
                g.GetComponent<Renderer>().material.color = Color.yellow;
            }
            yield return new WaitForSeconds(0.1f);
            foreach (GameObject g in lines)
            {
                g.GetComponent<Renderer>().material.color = Color.white;
            }
            yield return new WaitForSeconds(0.1f);
            foreach (GameObject g in lines)
            {
                g.GetComponent<Renderer>().material.color = Color.red;
            }
        }
        //instantiace bullets at the same angles as the lines 
        angle = startingAngle;
        audioSource.PlayOneShot(fireSound, 0.5f);
        animator.SetTrigger("ArcShot");
        for (int i = 0; i < 10; i++)
        {
            position = transform.position;
            position.y += 1.5f;
            position = position + (Quaternion.Euler(0, angle, 0) * transform.forward) * 4;

            Quaternion rotation = Quaternion.Euler(0, angle,0) * transform.rotation;

            GameObject aim = Instantiate(bullet, position, rotation);
            angle -= (startingAngle * 2) / 8.0f;
        }
        yield return new WaitForSeconds(0.1f);
        foreach (GameObject g in lines)
        {
            Destroy(g);
        }
        animator.SetTrigger("Idle");
        controller.setState(FirstBossEnemy.State.Decide);
    }
   
}
