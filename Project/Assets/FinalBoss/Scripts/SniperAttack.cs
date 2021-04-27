using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//boss sniper attack - Dvir
public class SniperAttack : MonoBehaviour
{
    //set the references
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject line;
    [SerializeField] private GameObject bullet;

    [SerializeField] private AudioClip fireSound;
    AudioSource audioSource;

    private GameObject lineInstance;
    private bool isTracking = false;
    private Animator animator;

    FirstBossEnemy controller;
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<FirstBossEnemy>();
    }
    void Update()
    {
        //set the rotation and scale of the line instance 
        if (isTracking)
        {
            this.transform.LookAt(player.transform.position);
            Vector3 position = transform.position;
            position = position + transform.forward * 2;
            float scale = Vector3.Distance(position, player.transform.position);
            lineInstance.transform.localScale = new Vector3(scale, lineInstance.transform.localScale.y, lineInstance.transform.localScale.z);
            Vector3 lookDir = player.transform.position - transform.position;

         
            Quaternion newRotation = Quaternion.LookRotation(lookDir);
              

            lineInstance.transform.rotation = newRotation;

            lineInstance.transform.rotation = Quaternion.Euler(0, 90, 0) * lineInstance.transform.rotation;



        }
    }
    //Sniper attack coroutine
    public IEnumerator Attack()
    {
        this.transform.LookAt(player.transform.position);
        yield return new WaitForSeconds(0.5f);

        Vector3 position;
        position = transform.position;
        position.y += 1.5f;
        position = position +  transform.forward * 2;

        animator.SetTrigger("SniperAim");
        //instantiate the line and enable tracking 
        Quaternion rotation = Quaternion.Euler(0, 90, 0) * transform.rotation;       
        lineInstance = Instantiate(line, position, rotation,this.transform);
        isTracking = true;
        GameObject lineInstanceChild = lineInstance.transform.GetChild(0).gameObject;
        yield return new WaitForSeconds(1);
        //change the color of the line instance 
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.1f);
            lineInstanceChild.GetComponent<Renderer>().material.color = Color.yellow;

            yield return new WaitForSeconds(0.1f);
            lineInstanceChild.GetComponent<Renderer>().material.color = Color.white;
            
            yield return new WaitForSeconds(0.1f);
            lineInstanceChild.GetComponent<Renderer>().material.color = Color.red;
            
        }
        audioSource.PlayOneShot(fireSound, 0.3f);

        position = transform.position;
        position.y += 1.5f;
        position = position + transform.forward * 2;
        //intantiate the bullet instance 
        animator.SetTrigger("SniperShot");
        Vector3 playerPosition = player.transform.position;
        playerPosition.y = 1.5f;
        GameObject aim = Instantiate(bullet, position, Quaternion.identity);
        aim.transform.LookAt(playerPosition);
        isTracking = false;
        Destroy(lineInstance);
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Idle");
        controller.setState(FirstBossEnemy.State.Decide);
    }
}
