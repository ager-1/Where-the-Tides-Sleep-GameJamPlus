using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Needed for New Input System

public class PauseMenu : MonoBehaviour
{
	[Header("UI References")]
	public GameObject pauseMenuUI; // Drag your Pause Panel here

	[Header("Scene Management")]
	public string mainMenuSceneName = "MainMenu";

	// Internal State
	private bool isPaused = false;
	private PlayerInput playerInput;
	private InputAction pauseAction;

	void Awake()
	{
		// Setup New Input System manually without a PlayerInput component
		// This listens for the "Escape" key globally
		playerInput = new PlayerInput();
		pauseAction = new InputAction("Pause", binding: "<Keyboard>/escape");
		pauseAction.performed += ctx => TogglePause();
	}

	void OnEnable()
	{
		pauseAction.Enable();
	}

	void OnDisable()
	{
		pauseAction.Disable();
	}

	public void TogglePause()
	{
		if (isPaused)
		{
			Resume();
		}
		else
		{
			Pause();
		}
	}

	public void Resume()
	{
		pauseMenuUI.SetActive(false);
		Time.timeScale = 1f; // Unfreeze time
		isPaused = false;

		// Lock cursor back for gameplay
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	void Pause()
	{
		pauseMenuUI.SetActive(true);
		Time.timeScale = 0f; // Freeze time
		isPaused = true;

		// Unlock cursor for menu
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void LoadMenu()
	{
		Time.timeScale = 1f; // Always unfreeze before leaving scene
		SceneManager.LoadScene(mainMenuSceneName);
	}

	public void QuitGame()
	{
		Debug.Log("Quitting Game...");
		Application.Quit();
	}
}