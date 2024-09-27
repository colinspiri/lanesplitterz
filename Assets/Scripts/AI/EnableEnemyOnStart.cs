using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnableEnemyOnStart : MonoBehaviour
{
    [SerializeField] private string activeEnemy;

    void Awake()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>(true);

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].gameObject.SetActive(enemies[i].gameObject.name == activeEnemy);
        }
    }
}
