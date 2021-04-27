using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//lazer damage - Dvir
public class LazerDamage : MonoBehaviour
{
    private bool canDamage = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator DamagePause()
    {
        yield return new WaitForSeconds(0.5f);
        canDamage = true;
    }
    void OnParticleCollision(GameObject other)
    {

        //deal damage to enemies 
        if (other.gameObject.tag == "Enemy" && canDamage)
        {
            GameObject enemy = other.gameObject;
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            canDamage = false;
            enemyScript.Damage(50);
            StartCoroutine(DamagePause());
        }
           
        
    }
}
