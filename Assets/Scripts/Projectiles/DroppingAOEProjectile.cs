using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppingAOEProjectile : BaseProjectile
{
    public float radius = 4f;

    protected override void Start()
    {
        base.Start();
        rb.useGravity = false; // We use custom gravity instead
        useCustomGravity = true; // Enable per-projectile
    }

    protected override void OnHit(Collider other)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var col in hitColliders)
        {
            if (other.GetComponentInParent<BaseEnemyAI>() is BaseEnemyAI enemy)
            {
                enemy.TakeDamage(damage);
            }

            Debug.Log($"Dropping AOE hit {col.name}, damage: {damage}");
        }

        Destroy(gameObject);
    }
}

