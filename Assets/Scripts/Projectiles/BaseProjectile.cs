using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class BaseProjectile : MonoBehaviour
{
    protected float damage;
    protected float range;
    protected float speed;
    protected WeaponData sourceWeapon;

    protected Rigidbody rb;

    [Header("Physics")]
    public bool useCustomGravity = false;
    public float gravityMultiplier = 1f;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        Launch();
        Destroy(gameObject, 5f); // Cleanup
    }

    protected virtual void FixedUpdate()
    {
        if (useCustomGravity)
        {
            rb.AddForce(Vector3.down * 9.81f * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    public virtual void Init(WeaponData weaponData)
    {
        damage = weaponData.damage;
        speed = weaponData.projectileSpeed;
        range = weaponData.range;
        sourceWeapon = weaponData;
    }

    protected virtual void Launch()
    {
        rb.velocity = transform.forward * speed;
    }

    protected abstract void OnHit(Collider other);

    protected virtual void OnTriggerEnter(Collider other)
    {
        OnHit(other);
    }
}
