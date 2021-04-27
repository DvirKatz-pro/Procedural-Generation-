using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * handle sphere damage
 */
public class SphereDamage : MonoBehaviour
{
    public bool playerHit = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnParticleCollision(GameObject other)
    {

        //deal damage to player
        if (other.gameObject.tag == "Player" && !playerHit)
        {
            playerHit = true;
            GameObject player = other.gameObject;
            PlayerStatus damageScript = player.GetComponent<PlayerStatus>();
            damageScript.takeDamage(20, transform.forward, damageScript.unblockable);
        }


    }
}
