using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "AI/New Enemy")]
public class EnemyStats : ScriptableObject
{
    public float health = 100f;
    public float speed = 3f;

    [Header("Movement Control")]
    public float stoppingDistance = 2f;
    public float memoryDuration = 3f;

    [Header("Combat Control")]
    public float damage = 10f;
    public float range = 10f;
    public float attackCooldown = 1.5f;
    [Range(0f, 1f)] public float accuracy = 0.75f;

}

