using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneAI : BaseEnemyAI
{
    [Header("Drone Settings")]
    public float hoverHeight = 3f;
    public float wanderRadius = 15f;
    public Transform droneBody;

    private bool isShooting = false;

    private float hoverOffset;

    protected override void Start()
    {
        base.Start();

        hoverOffset = Random.Range(0f, 100f);
    }

    private void LateUpdate()
    {
        if (droneBody != null)
        {
            float bob = Mathf.Sin(Time.time * 2f + hoverOffset) * 0.3f;
            Vector3 targetPos = transform.position + Vector3.up * (hoverHeight + bob);
            droneBody.position = Vector3.Lerp(droneBody.position, targetPos, Time.deltaTime * 5f);
        }
    }

    protected override void UpdateWanderState()
    {
        if (agent.isStopped)
            agent.isStopped = false;

        if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance || agent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathComplete))
        {
            PickNewHoverDestination();
        }
    }

    private void PickNewHoverDestination()
    {
        Vector3 randomOffset = Random.insideUnitSphere * wanderRadius;
        randomOffset.y = Random.Range(hoverHeight - 2f, hoverHeight + 2f); // Add vertical variety
        Vector3 target = transform.position + randomOffset;

        if (UnityEngine.AI.NavMesh.SamplePosition(target, out var hit, wanderRadius, UnityEngine.AI.NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    protected override void UpdateChaseState()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        agent.SetDestination(player.position);

        if (distanceToPlayer <= stats.stoppingDistance)
        {
            agent.isStopped = true;

            if (!isShooting && Time.time >= lastAttackTime + stats.attackCooldown)
            {
                StartCoroutine(FireBurst());
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.isStopped = false;
        }
    }

    private IEnumerator FireBurst()
    {
        isShooting = true;

        FireSingleShot();
        yield return new WaitForSeconds(0.2f); // Delay between shots
        FireSingleShot();

        isShooting = false;
    }

    private void FireSingleShot()
    {
        if (player == null) return;

        Vector3 shootOrigin = transform.position + Vector3.up * 1.5f;
        Vector3 directionToPlayer = (player.position + Vector3.up * 1.5f) - shootOrigin;
        directionToPlayer.Normalize();

        bool isAccurateShot = Random.value <= stats.accuracy;

        if (!isAccurateShot)
        {
            Debug.Log($"{name} missed due to low accuracy.");
            directionToPlayer = ApplyAimSpread(directionToPlayer, 15f);
        }

        Debug.DrawRay(shootOrigin, directionToPlayer * stats.range, Color.cyan, 1f);

        if (Physics.Raycast(shootOrigin, directionToPlayer, out RaycastHit hit, stats.range))
        {
            var playerStats = hit.collider.GetComponentInParent<CharacterStats>();
            if (playerStats != null)
            {
                float damage = stats.GetRandomDamage();
                playerStats.TakeDamage(damage);
                Debug.Log($"{name} hit the player for {damage} damage (Drone Shot).");
            }
        }
    }
}
