using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//boss reinforcement state - Dvir
public class Reinforcement : MonoBehaviour
{
    //set the refernces
    [SerializeField] private int enemyCount;
    [SerializeField] private GameObject Enemy;
    [SerializeField] private GameObject enemySpawn;
    [SerializeField] private List<GameObject> enemySpawns;
    [SerializeField] private List<Vector3> enemySpawnsPositions;
    [SerializeField] private ParticleSystem shield;
    [SerializeField] private GameObject arenaCenter;
    [SerializeField] private float distance;
    private Animator animator;

    FirstBossEnemy controller;

    bool isReinforceing = false;
    void Start()
    {
        controller = GetComponent<FirstBossEnemy>();
        animator = GetComponent<Animator>();
        //calculate the 4 symmetrical positions around the center of the arena
        Vector3 position = arenaCenter.transform.position;
        Vector3 topRight = position;
        topRight.x += distance;
        topRight.z += distance;
        enemySpawnsPositions.Add(topRight);

        Vector3 botRight = position;
        botRight.x -= distance;
        botRight.z += distance;
        enemySpawnsPositions.Add(botRight);

        Vector3 botLeft = position;
        botLeft.x -= distance;
        botLeft.z -= distance;
        enemySpawnsPositions.Add(botLeft);

        Vector3 topLeft = position;
        topLeft.x += distance;
        topLeft.z -= distance;
        enemySpawnsPositions.Add(topLeft);

        foreach (Vector3 v in enemySpawnsPositions)
        {
            GameObject g = Instantiate(enemySpawn, v, Quaternion.identity);
            enemySpawns.Add(g);
        }

    }
    //set the amount of enemies remaining
    public void setEnemyCount(int count)
    {
        enemyCount = count;
    }
    //coroutine to control reinforcement ability
    public IEnumerator Reinforce()
    {
        if (!isReinforceing)
        {
            isReinforceing = true;
            yield return new WaitForSeconds(1);
            //teleport to the center of the arena 
            this.GetComponent<FirstBossEnemy>().enableTeleport = true;
            List<Vector3> positions = new List<Vector3>();
            positions.Add(arenaCenter.transform.position);
            StartCoroutine(GetComponent<Teleport>().TeleportMove(positions));
            while (GetComponent<FirstBossEnemy>().isTeleporting != false)
            {
                yield return new WaitForSeconds(0.001f);
            }
            if (GetComponent<FirstBossEnemy>().isTeleporting == false)
            {
                animator.SetTrigger("Reinforce");
                shield.Play();
                GetComponent<FirstBossEnemy>().enableTeleport = false;
                //play the particle effects at the 4 symmetrical positions 
                foreach (GameObject e in enemySpawns)
                {
                    e.GetComponent<ParticleSystem>().Play();
                }
                yield return new WaitForSeconds(4);
                foreach (GameObject e in enemySpawns)
                {
                    Instantiate(Enemy, e.transform);
                    e.GetComponent<ParticleSystem>().Stop();

                }

            }
            while (enemyCount > 0)
            {
                yield return new WaitForSeconds(0.001f);
            }
            //reset the state
            animator.SetTrigger("Idle");
            shield.Stop();
            GetComponent<FirstBossEnemy>().enableTeleport = true;
            isReinforceing = false;
            controller.setState(FirstBossEnemy.State.Decide);
        }
    }
}
