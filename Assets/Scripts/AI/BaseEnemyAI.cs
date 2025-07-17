using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BaseEnemyAI : MonoBehaviour
{
    public EnemyStats stats;

    protected NavMeshAgent agent;
    protected Transform player;

    private float currentHealth;
    private float memoryTimer;

    protected float lastAttackTime = -Mathf.Infinity;

    private enum AIState { Wandering, Chasing }
    private AIState currentState = AIState.Wandering;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        agent.speed = stats.speed;
        agent.stoppingDistance = stats.stoppingDistance;
        agent.isStopped = false;

        currentHealth = stats.health;
        memoryTimer = 0f;
        currentState = AIState.Wandering;
    }

    private void Update()
    {
        UpdateVision();

        switch (currentState)
        {
            case AIState.Chasing:
                UpdateChaseState();
                break;

            case AIState.Wandering:
                UpdateWanderState();
                break;
        }
    }

    // Update line of sight and detection logic
    protected virtual void UpdateVision()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        bool inRange = distance < stats.range;
        bool inSight = !Physics.Raycast(transform.position, direction.normalized, distance, LayerMask.GetMask("Obstacles"));

        if (inRange && inSight)
        {
            memoryTimer = stats.memoryDuration;
            currentState = AIState.Chasing;
        }
        else
        {
            memoryTimer -= Time.deltaTime;
            if (memoryTimer <= 0f)
            {
                currentState = AIState.Wandering;
            }
        }
    }

    // Wandering: move between random points
    protected virtual void UpdateWanderState()
    {
        if (agent.isStopped)
            agent.isStopped = false;

        if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance || agent.pathStatus != NavMeshPathStatus.PathComplete))
        {
            PickNewWanderDestination();
        }
    }

    private void PickNewWanderDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection.y = 0;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    // Chase the player and attack
    protected virtual void UpdateChaseState()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        agent.SetDestination(player.position);

        if (distanceToPlayer <= stats.stoppingDistance)
        {
            agent.isStopped = true;

            if (Time.time >= lastAttackTime + stats.attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.isStopped = false;
        }
    }

    // Placeholder attack logic
    protected virtual void Attack()
    {
        if (player == null) return;

        Vector3 shootOrigin = transform.position + Vector3.up * 1.5f; // Adjust for enemy height
        bool isAccurateShot = Random.value <= stats.accuracy;
        Vector3 directionToPlayer = (player.position + Vector3.up * 1.5f) - shootOrigin;
        directionToPlayer.Normalize();

        if (!isAccurateShot)
        {
            Debug.Log($"{name} missed due to low accuracy.");
            directionToPlayer = ApplyAimSpread(directionToPlayer, 15f);
        }
        else
        {
            directionToPlayer = ApplyAimSpread(directionToPlayer, 0f);
        }

        // Apply inaccuracy based on accuracy %
        float missAngle = Mathf.Lerp(15f, 0f, stats.accuracy);
        directionToPlayer = ApplyAimSpread(directionToPlayer, missAngle);

        if (Physics.Raycast(shootOrigin, directionToPlayer, out RaycastHit hit, stats.range))
        {
            Debug.DrawRay(shootOrigin, directionToPlayer * stats.range, Color.red, 1f);

            var playerStats = hit.collider.GetComponentInParent<CharacterStats>();
            if (playerStats != null)
            {
                float damage = stats.GetRandomDamage();
                playerStats.TakeDamage(damage);
                Debug.Log($"{name} hit the player for {damage} damage.");
            }
            else
            {
                Debug.Log($"{name} missed or hit something else: {hit.collider.name}");
            }
        }
    }

    protected Vector3 ApplyAimSpread(Vector3 direction, float maxAngleDegrees)
    {
        if (maxAngleDegrees <= 0f) return direction;

        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(-maxAngleDegrees, maxAngleDegrees),
            Random.Range(-maxAngleDegrees, maxAngleDegrees),
            0f // Optional: skip roll (z-axis) for realism
        );

        return randomRotation * direction;
    }

    // Health system
    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} has died.");
        Destroy(gameObject);
    }
}