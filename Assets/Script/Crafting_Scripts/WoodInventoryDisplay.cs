using UnityEngine;
using TMPro;

public class WoodInventoryDisplay : MonoBehaviour
{
    [Header("References")]
    public WoodInventory inventory; // Drag your 'PlayerWoodInventory' asset here
    public TMP_Text displayLabel;   // Drag your Text object here

    void Start()
    {
        UpdateDisplay();
    }

    // Call this if you want to refresh the text manually
    public void UpdateDisplay()
    {
        if (inventory != null && displayLabel != null)
        {
            // Counts how many items are in the list
            displayLabel.text = "TOTAL WOOD: " + inventory.collectedWood.Count;
        }
        else
        {
            displayLabel.text = "TOTAL WOOD: 0";
        }
    }
}