using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneAutoLoader : MonoBehaviour
{
    [Header("Settings")]
    public float delayTime = 5f; 

    [Header("Scene Navigation")]
    [Tooltip("Scene to load if you have 9 or more fish")]
    public string goodSceneName; 
    
    [Tooltip("Scene to load if you have LESS than 9 fish")]
    public string badSceneName;

    [Header("Inventory Reference")]
    public FishInventory playerInventory; // Drag your 'MyInventory' asset here

    void Start()
    {
        StartCoroutine(CheckAndLoadRoutine());
    }

    IEnumerator CheckAndLoadRoutine()
    {
        Debug.Log($"Waiting {delayTime} seconds...");
        yield return new WaitForSeconds(delayTime);

        if (playerInventory != null)
        {
            // Check the list count
            // NOTE: Ensure your list variable in FishInventory.cs is named 'caughtFish' and is public!
            int fishCount = playerInventory.caughtFish.Count;

            Debug.Log($"Fish Count: {fishCount}");

            if (fishCount < 9)
            {
                Debug.Log("Not enough fish (< 9). Loading Bad Scene.");
                LoadScene(badSceneName);
            }
            else
            {
                Debug.Log("Enough fish (9+). Loading Good Scene.");
                LoadScene(goodSceneName);
            }
        }
        else
        {
            Debug.LogError("CRITICAL: 'MyInventory' is not assigned in the Inspector! Loading Default (Good) Scene.");
            LoadScene(goodSceneName);
        }
    }

    void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene Name field is empty in Inspector!");
        }
    }
}