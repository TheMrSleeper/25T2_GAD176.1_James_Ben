using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public Transform weaponHolder;
    public float pickupRange = 3f;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.G;
    public KeyCode swapKey = KeyCode.Q;

    private BaseWeapon weaponSlot1;
    private BaseWeapon weaponSlot2;
    private BaseWeapon currentWeapon;

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickupWeapon();
        }

        if (Input.GetKeyDown(dropKey))
        {
            DropCurrentWeapon();
        }

        if (Input.GetKeyDown(swapKey))
        {
            SwapWeapon();
        }
    }

    void TryPickupWeapon()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (var hit in hits)
        {
            BaseWeapon weapon = hit.GetComponent<BaseWeapon>();
            if (weapon != null && weapon.transform.parent == null)
            {
                PickupWeapon(weapon);
                return;
            }
        }

        Debug.Log("No weapon nearby to pick up.");
    }

    void PickupWeapon(BaseWeapon weapon)
    {
        if (weaponSlot1 == null)
        {
            weaponSlot1 = weapon;
            EquipWeapon(weaponSlot1);
        }
        else if (weaponSlot2 == null)
        {
            weaponSlot2 = weapon;
            if (currentWeapon == null) EquipWeapon(weaponSlot2);
        }
        else
        {
            Debug.Log("Both weapon slots full.");
            return;
        }

        // Parent to player
        weapon.transform.SetParent(weaponHolder);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        // Disable physics
        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Collider col = weapon.GetComponent<Collider>();
        if (col) col.enabled = false;

        // Disable weapon unless it's currently equipped
        if (weapon != currentWeapon)
        {
            weapon.gameObject.SetActive(false);
        }

        Debug.Log($"Picked up {weapon.weaponData.weaponName}");
    }

    void EquipWeapon(BaseWeapon weaponToEquip)
    {
        // Loop through all weapons in the holder
        foreach (Transform child in weaponHolder)
        {
            BaseWeapon wpn = child.GetComponent<BaseWeapon>();
            if (wpn != null)
            {
                wpn.enabled = false;
                wpn.gameObject.SetActive(false); // Hide weapon visuals
            }
        }

        currentWeapon = weaponToEquip;

        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(true); // Show selected weapon
            currentWeapon.enabled = true;
        }
    }

    void DropCurrentWeapon()
    {
        if (currentWeapon == null) return;

        currentWeapon.transform.SetParent(null);

        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;

        Collider col = currentWeapon.GetComponent<Collider>();
        if (col) col.enabled = true;

        // Clear the appropriate slot
        if (currentWeapon == weaponSlot1)
        {
            weaponSlot1 = null;
            currentWeapon = null;

            // Auto-switch to slot 2 if available
            if (weaponSlot2 != null)
            {
                EquipWeapon(weaponSlot2);
            }
        }
        else if (currentWeapon == weaponSlot2)
        {
            weaponSlot2 = null;
            currentWeapon = null;

            // Auto-switch to slot 1 if available
            if (weaponSlot1 != null)
            {
                EquipWeapon(weaponSlot1);
            }
        }

        Debug.Log("Dropped current weapon.");
    }

    void SwapWeapon()
    {
        if (weaponSlot1 == null || weaponSlot2 == null) return;
        if (currentWeapon == weaponSlot1)
        {
            EquipWeapon(weaponSlot2);
        }
        else if (currentWeapon == weaponSlot2)
        {
            EquipWeapon(weaponSlot1);
        }
    }

    public BaseWeapon GetCurrentWeapon() => currentWeapon;
}
