using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "WeaponInventory", menuName = "Weapons/Inventory")]
public class WeaponInventory : ScriptableObject
{
    public List<WeaponData> collectedWeapons = new List<WeaponData>();

    public void AddWeapon(WeaponData weapon)
    {
        if (!collectedWeapons.Contains(weapon))
        {
            collectedWeapons.Add(weapon);
            Debug.Log("Weapon Added: " + weapon.weaponName);
        }
    }

    // Check if we already have it
    public bool HasWeapon(WeaponData weapon)
    {
        return collectedWeapons.Contains(weapon);
    }
}