using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : BaseProjectile
{
    protected override void OnHit(Collider other)
    {
        if (other.GetComponentInParent<BaseEnemyAI>() is BaseEnemyAI enemy)
        {
            enemy.TakeDamage(damage);
        }

        Debug.Log($"Standard projectile hit {other.name}, damage: {damage}");
        Destroy(gameObject);
    }
}
