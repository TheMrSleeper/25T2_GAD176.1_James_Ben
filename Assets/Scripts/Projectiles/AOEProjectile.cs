using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEProjectile : BaseProjectile
{
    public float radius = 3f;

    protected override void OnHit(Collider other)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var col in hitColliders)
        {
            // if (col.TryGetComponent<BaseEnemyAI>(out var enemy))
            // {
            //     enemy.TakeDamage(damage);
            // }

            Debug.Log($"AOE hit {col.name} in radius {radius}, damage: {damage}");
        }

        Destroy(gameObject);
    }
}
