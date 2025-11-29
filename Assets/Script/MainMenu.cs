using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq; // Needed for sorting resolutions

public class MainMenu : MonoBehaviour
{
	[Header("UI References")]
	public Slider VolumeSlider;
	public TMP_Dropdown resolutionDropdown;

	private Resolution nativeRes;
	private Resolution mediumRes;

	void Start()
	{
		// 1. Initialize Volume
		if (PlayerPrefs.HasKey("soundVolume")) LoadVolume();
		else { PlayerPrefs.SetFloat("soundVolume", 1); LoadVolume(); }

		// 2. Initialize Modes
		InitializeModes();
	}

	void InitializeModes()
	{
		if (resolutionDropdown == null)
		{
			Debug.LogError("❌ Resolution Dropdown not assigned!");
			return;
		}

		// Get the desktop/monitor resolution (Native)
		nativeRes = Screen.currentResolution;

		// Calculate the "Medium" resolution (Next valid step down)
		mediumRes = FindNextLowerResolution(nativeRes);

		// Clear and Populate Dropdown
		resolutionDropdown.ClearOptions();

		// Create the 3 specific options with dynamic text
		List<string> options = new List<string> {
			$"Fullscreen",
			$"Windowed",
			$"Medium"
		};

		resolutionDropdown.AddOptions(options);

		// Set the dropdown value based on current state
		if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
			resolutionDropdown.value = 0;
		else if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
			resolutionDropdown.value = 1;
		else
			resolutionDropdown.value = 2; // Assume medium/windowed

		resolutionDropdown.RefreshShownValue();
	}

	Resolution FindNextLowerResolution(Resolution current)
	{
		// 1. Get all supported resolutions from hardware
		// 2. Filter to remove duplicates (same size, different Hz)
		// 3. Sort by Width Descending (Big -> Small)
		var allRes = Screen.resolutions
			.Select(r => new { r.width, r.height }) // Select only dimensions
			.Distinct() // Remove duplicates
			.OrderByDescending(r => r.width) // Sort highest to lowest
			.ToList();

		// 4. Loop to find the first one smaller than Native
		foreach (var res in allRes)
		{
			// If this resolution is smaller than our native one
			if (res.width < current.width && res.height < current.height)
			{
				// Found it! Return as our "Medium" option
				Resolution r = new Resolution();
				r.width = res.width;
				r.height = res.height;
				return r;
			}
		}

		// FALLBACK: If we couldn't find a smaller hardware resolution (rare),
		// manually create one that is half size.
		Resolution fallback = new Resolution();
		fallback.width = current.width / 2;
		fallback.height = current.height / 2;
		return fallback;
	}

	public void SetResolution(int index)
	{
		switch (index)
		{
			case 0: // Fullscreen
				Screen.SetResolution(nativeRes.width, nativeRes.height, FullScreenMode.ExclusiveFullScreen);
				Debug.Log($"Applied: Fullscreen {nativeRes.width}x{nativeRes.height}");
				break;

			case 1: // Windowed (Maximized / Borderless)
				Screen.SetResolution(nativeRes.width, nativeRes.height, FullScreenMode.FullScreenWindow);
				Debug.Log($"Applied: Borderless {nativeRes.width}x{nativeRes.height}");
				break;

			case 2: // Medium (Windowed)
				Screen.SetResolution(mediumRes.width, mediumRes.height, FullScreenMode.Windowed);
				Debug.Log($"Applied: Medium Windowed {mediumRes.width}x{mediumRes.height}");
				break;
		}
	}

	// --- BASE LOGIC ---
	public void StartGame() => SceneManager.LoadScene("Prologue");
	public void ExitGame() => Application.Quit();

	// --- VOLUME LOGIC ---
	public void SetVolume() { AudioListener.volume = VolumeSlider.value; SaveVolume(); }
	public void SaveVolume() => PlayerPrefs.SetFloat("soundVolume", VolumeSlider.value);
	public void LoadVolume() { if (VolumeSlider != null) VolumeSlider.value = PlayerPrefs.GetFloat("soundVolume"); }
}