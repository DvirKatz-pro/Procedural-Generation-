using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyChase : MonoBehaviour
{
    public abstract void StartChase();
    public abstract void StopChase();
}
