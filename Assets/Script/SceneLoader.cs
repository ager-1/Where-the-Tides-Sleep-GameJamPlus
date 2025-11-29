
using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class SceneLoader : MonoBehaviour
{
    [Header("Settings")]
    // Type the EXACT name of your fishing scene here in the Inspector
    public string sceneName = "FishingScene";

    // Link this function to your Button
    public void LoadTargetScene()
    {
        Debug.Log("Loading Scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    // Optional: A function to quit the game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }
}