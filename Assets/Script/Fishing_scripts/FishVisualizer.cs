using UnityEngine;
using System.Collections;

public class FishVisualizer : MonoBehaviour
{
    public Transform spawnPoint;
    public FishingMinigame gameScript;

    public void ShowFish()
    {
        if (gameScript.currentFish.fishModelPrefab == null) return;

        // Spawn the fish
        GameObject model = Instantiate(gameScript.currentFish.fishModelPrefab, spawnPoint.position, spawnPoint.rotation);
        model.transform.SetParent(spawnPoint);

        // Destroy it after 4 seconds
        Destroy(model, 4f);
    }
}