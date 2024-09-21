using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheRefresher : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<Enemy>().GetComponentInChildren<EnemyBall>(true).ComputeValues();
    }
}
