using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FireMode { Hitscan, Projectile }

public enum FireType { SemiAuto, Automatic }
public enum ShotType { Hitscan, Projectile }

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/New Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("General Info")]
    public string weaponName;
    public GameObject weaponPrefab;

    [Header("Firing Settings")]
    public FireType fireType = FireType.SemiAuto;
    public ShotType shotType = ShotType.Hitscan;

    public float fireRate = 5f; // rounds per second
    public float damage = 10f;
    public float range = 100f;

    [Header("Ammo Settings")]
    public int magazineSize = 30;
    public float reloadTime = 1.5f;

    [Header("Projectile Settings (Only if ShotType is Projectile)")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 30f;

    [Header("Hitscan Visuals (Only if ShotType is HitScan")]
    public bool useHitscanVisuals = false;
    public GameObject hitscanEffectPrefab;
    public float hitscanEffectDuration = 0.05f;
}
