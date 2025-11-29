using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishName;
    public GameObject fishModelPrefab; // Your 3D Model
    [Range(0.1f, 10f)] public float movementSpeed = 2f; // Higher = Harder
    [Range(0.1f, 0.5f)] public float zoneSize = 0.25f; // Lower = Harder
}