using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WoodInventory", menuName = "Wood Game/Inventory")]
public class WoodInventory : ScriptableObject
{
    public List<WoodData> collectedWood = new List<WoodData>();

    public void AddWood(WoodData wood)
    {
        collectedWood.Add(wood);
    }
}