using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    [Header("References")]
    public WeaponData weaponData;
    public Transform muzzleTransform;

    protected float lastFireTime;
    protected int currentAmmo;
    protected bool isReloading = false;

    protected virtual void Start()
    {
        currentAmmo = weaponData.magazineSize;
    }

    protected virtual void Update()
    {
        if (isReloading) return;

        if (weaponData.fireType == FireType.Automatic && IsFireHeld())
        {
            TryFire();
        }
        else if (weaponData.fireType == FireType.SemiAuto && IsFirePressed())
        {
            TryFire();
        }

        if (IsReloadPressed() && currentAmmo < weaponData.magazineSize)
        {
            StartCoroutine(Reload());
        }
    }

    protected virtual bool IsFirePressed() => Input.GetButtonDown("Fire1");
    protected virtual bool IsFireHeld() => Input.GetButton("Fire1");
    protected virtual bool IsReloadPressed() => Input.GetKeyDown(KeyCode.R);

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

    protected virtual void FireWeapon()
    {
        if (weaponData.shotType == ShotType.Hitscan)
        {
            Ray ray = new Ray(muzzleTransform.position, muzzleTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, weaponData.range))
            {
                Debug.Log($"{weaponData.weaponName} hitscan hit: {hit.collider.name}");

                // Apply damage logic here
                // hit.collider.GetComponent<Health>()?.TakeDamage(weaponData.damage);
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
