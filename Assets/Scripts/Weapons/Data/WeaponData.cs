using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FireMode { Hitscan, Projectile }

public enum FireType { SemiAuto, Automatic }
public enum ShotType { Hitscan, Projectile }

// Defines the data for weapons, configurable in the Unity Inspector
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/New Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("General Info")]
    public string weaponName;
    public GameObject weaponPrefab;

    [Header("Firing Settings")]
    public FireType fireType = FireType.SemiAuto;     // Semi-auto or automatic
    public ShotType shotType = ShotType.Hitscan;      // Hitscan or projectile

    public float fireRate = 5f;       // Rounds per second
    public float damage = 10f;        // Base damage
    public float range = 100f;        // Maximum range

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
