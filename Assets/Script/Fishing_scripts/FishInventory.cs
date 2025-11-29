using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Fishing/Inventory")]
public class FishInventory : ScriptableObject
{
    public List<FishData> caughtFish = new List<FishData>();

    public void AddFish(FishData fish)
    {
        caughtFish.Add(fish);
    }
}