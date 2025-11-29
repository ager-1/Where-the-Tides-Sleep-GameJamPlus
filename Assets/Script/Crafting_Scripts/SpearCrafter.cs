using UnityEngine;
using TMPro;
using System.Collections; // Needed for the timer

public class SpearCrafter : MonoBehaviour
{
    [Header("Requirements")]
    public int woodCost = 20;

    [Header("References")]
    public WoodInventory woodInventory;
    public WeaponInventory weaponInventory;
    public WeaponData spearData;

    [Header("Visuals")]
    public GameObject spearVisualModel;
    public TMP_Text statusText;

    void Start()
    {
        // 1. Hide the spear model
        if (spearVisualModel != null) spearVisualModel.SetActive(false);

        // 2. Clear the text so it is invisible on start
        if (statusText != null) statusText.text = "";
    }

    public void TryCraftSpear()
    {
        // Check if we already have it
        if (weaponInventory.HasWeapon(spearData))
        {
            ShowMessage("You already have a Spear!");
            return;
        }

        // Check wood count
        int currentWood = woodInventory.collectedWood.Count;

        if (currentWood >= woodCost)
        {
            CraftSuccess();
        }
        else
        {
            ShowMessage($"Need {woodCost} Wood. You have {currentWood}.");
        }
    }

    void CraftSuccess()
    {
        // Remove Wood
        for (int i = 0; i < woodCost; i++)
        {
            if (woodInventory.collectedWood.Count > 0)
                woodInventory.collectedWood.RemoveAt(0);
        }

        // Add Spear
        weaponInventory.AddWeapon(spearData);

        // Update Visuals
        ShowMessage("SPEAR CRAFTED!");

        if (spearVisualModel != null)
            spearVisualModel.SetActive(true);
    }

    // --- NEW HELPER FUNCTION ---
    void ShowMessage(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;

            // Stop any existing timer to prevent glitches
            StopAllCoroutines();

            // Start a new timer to hide text after 3 seconds
            StartCoroutine(HideTextAfterDelay());
        }
    }

    IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (statusText != null) statusText.text = ""; // Make invisible again
    }
}