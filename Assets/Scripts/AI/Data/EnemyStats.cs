using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "AI/New Enemy")]
public class EnemyStats : ScriptableObject
{
    public float health = 100f;
    public float speed = 3f;

    [Header("Movement Control")]
    public float stoppingDistance = 8f;
    public float memoryDuration = 3f;

    [Header("Combat Control")]
    public float minDamage = 5f;
    public float maxDamage = 15f;

    public float GetRandomDamage()
    {
        return Random.Range(minDamage, maxDamage);
    }

    public float range = 12f;
    public float attackCooldown = 1.5f;
    [Range(0f, 1f)] public float accuracy = 0.7f;

}

