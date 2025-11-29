using UnityEngine;
using TMPro;

public class InventoryDisplay : MonoBehaviour
{
    public FishInventory inventory; // Drag your 'MyInventory' asset here
    public TMP_Text displayLabel;   // Drag your Text object here

    void Start()
    {
        UpdateDisplay();
    }

    // Call this if you ever need to refresh it manually
    public void UpdateDisplay()
    {
        if (inventory != null && displayLabel != null)
        {
            displayLabel.text = $"TOTAL CATCH: {inventory.caughtFish.Count}";
        }
    }
}