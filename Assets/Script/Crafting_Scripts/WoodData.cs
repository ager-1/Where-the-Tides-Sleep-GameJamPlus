using UnityEngine;

[CreateAssetMenu(fileName = "New Wood", menuName = "Wood Game/Wood Data")]
public class WoodData : ScriptableObject
{
    public string woodName;
    public Sprite icon;
    public int scoreValue = 10;
}