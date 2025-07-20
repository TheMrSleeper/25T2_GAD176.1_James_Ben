using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    [Header("References")]
    public WeaponData weaponData;         // ScriptableObject with config
    public Transform muzzleTransform;     // Where bullets/projectiles originate

    protected float lastFireTime;         // Time of last shot
    protected int currentAmmo;
    protected bool isReloading = false;

    protected virtual void Start()
    {
        currentAmmo = weaponData.magazineSize;
    }

    protected virtual void Update()
    {
        // Skip firing if reloading
        if (isReloading) return;

        // Firing logic based on type
        if (weaponData.fireType == FireType.Automatic && IsFireHeld())
        {
            TryFire();
        }
        else if (weaponData.fireType == FireType.SemiAuto && IsFirePressed())
        {
            TryFire();
        }

        // Manual reload
        if (IsReloadPressed() && currentAmmo < weaponData.magazineSize)
        {
            StartCoroutine(Reload());
        }
    }

    protected virtual bool IsFirePressed() => Input.GetButtonDown("Fire1");
    protected virtual bool IsFireHeld() => Input.GetButton("Fire1");
    protected virtual bool IsReloadPressed() => Input.GetKeyDown(KeyCode.R);

    // Attempts to fire if fire rate and ammo allow it
    protected virtual void TryFire()
    {
        if (Time.time < lastFireTime + (1f / weaponData.fireRate)) return;
        if (currentAmmo <= 0)
        {
            Debug.Log($"{weaponData.weaponName} is out of ammo.");
            return;
        }

        lastFireTime = Time.time;
        currentAmmo--;

        FireWeapon();
    }

    // Executes either hitscan or projectile logic
    protected virtual void FireWeapon()
    {
        if (weaponData.shotType == ShotType.Hitscan)
        {
            Ray ray = new Ray(muzzleTransform.position, muzzleTransform.forward);
            if (Physics.Raycast(muzzleTransform.position, muzzleTransform.forward, out RaycastHit hit, weaponData.range))
            {
                Debug.Log("Hitscan weapon hit: " + hit.collider.name);

                // Check if we hit an enemy
                if (hit.collider.GetComponentInParent<BaseEnemyAI>() is BaseEnemyAI enemy)
                {
                    enemy.TakeDamage(weaponData.damage);
                    Debug.Log($"Dealt {weaponData.damage} hitscan damage to {enemy.name}");
                }
            }
            else
            {
                Debug.Log($"{weaponData.weaponName} missed (hitscan).");
            }
        }
        else if (weaponData.shotType == ShotType.Projectile)
        {
            if (weaponData.projectilePrefab != null)
            {
                GameObject proj = Instantiate(
                    weaponData.projectilePrefab,
                    muzzleTransform.position,
                    muzzleTransform.rotation
                );

                // Prevent projectile hitting its shooter/gun
                if (proj.TryGetComponent<Collider>(out var projCol))
                {
                    Collider[] shooterColliders = GetComponentsInChildren<Collider>();
                    foreach (var col in shooterColliders)
                    {
                        Physics.IgnoreCollision(projCol, col, true);
                    }
                }

                if (proj.TryGetComponent<BaseProjectile>(out var projectile))
                {
                    projectile.Init(weaponData);
                }

                if (proj.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.velocity = muzzleTransform.forward * weaponData.projectileSpeed;                    
                }
            }
            else
            {
                Debug.LogWarning($"{weaponData.weaponName} has no projectile prefab assigned.");
            }
        }
    }

    // Handles reloading with a delay
    protected virtual System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log($"{weaponData.weaponName} is reloading...");

        yield return new WaitForSeconds(weaponData.reloadTime);

        currentAmmo = weaponData.magazineSize;
        isReloading = false;

        Debug.Log($"{weaponData.weaponName} reloaded.");
    }

    public virtual int GetCurrentAmmo() => currentAmmo;
    public virtual int GetMaxAmmo() => weaponData.magazineSize;
}
