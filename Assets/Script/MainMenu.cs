using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MainMenu : MonoBehaviour
{
	[Header("UI References")]
	public Slider VolumeSlider;
	public TMP_Dropdown resolutionDropdown;

	private Resolution maxRes;
	private Resolution mediumRes;

	void Start()
	{
		if (PlayerPrefs.HasKey("soundVolume")) LoadVolume();
		else { PlayerPrefs.SetFloat("soundVolume", 1); LoadVolume(); }

		InitializeModes();
	}

	void InitializeModes()
	{
		if (resolutionDropdown == null)
		{
			Debug.LogError("❌ Resolution Dropdown not assigned!");
			return;
		}

		// 1. FIND HIGHEST RESOLUTION
		Resolution[] allResolutions = Screen.resolutions;
		maxRes = allResolutions.OrderByDescending(x => x.width).First();

		// 2. FIND MEDIUM RESOLUTION
		mediumRes = FindNextLowerResolution(maxRes, allResolutions);

		// 3. POPULATE DROPDOWN
		resolutionDropdown.ClearOptions();
		List<string> options = new List<string> {
			"Fullscreen",       // 0: True Fullscreen (No borders)
            "Windowed",         // 1: Standard Window with X button (Maximized)
            "Medium"            // 2: Smaller Standard Window
        };
		resolutionDropdown.AddOptions(options);

		// 4. DETECT CURRENT STATE
		if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
			resolutionDropdown.value = 0;
		else if (Screen.fullScreenMode == FullScreenMode.Windowed && Screen.width > mediumRes.width)
			resolutionDropdown.value = 1; // Assuming large window is "Windowed"
		else
			resolutionDropdown.value = 2; // Assuming smaller window is "Medium"

		resolutionDropdown.RefreshShownValue();
	}

	Resolution FindNextLowerResolution(Resolution target, Resolution[] allRes)
	{
		var sorted = allRes
			.Select(r => new { r.width, r.height })
			.Distinct()
			.OrderByDescending(r => r.width)
			.ToList();

		foreach (var res in sorted)
		{
			if (res.width < target.width && res.height < target.height)
			{
				Resolution r = new Resolution();
				r.width = res.width;
				r.height = res.height;
				return r;
			}
		}

		Resolution fallback = new Resolution();
		fallback.width = target.width / 2;
		fallback.height = target.height / 2;
		return fallback;
	}

	public void SetResolution(int index)
	{
		switch (index)
		{
			case 0: // Fullscreen (Exclusive)
					// This removes all borders and takes over the screen
				Screen.SetResolution(maxRes.width, maxRes.height, FullScreenMode.ExclusiveFullScreen);
				Debug.Log("Applied: Exclusive Fullscreen");
				break;

			case 1: // Windowed (Maximized with Borders)
					// We use 'Windowed' mode here so the Title Bar (X button) remains visible.
					// Note: Sometimes we subtract a tiny bit of height to ensure the bar doesn't go offscreen
				Screen.SetResolution(maxRes.width, maxRes.height - 20, FullScreenMode.Windowed);
				Debug.Log("Applied: Standard Windowed (Maximized)");
				break;

			case 2: // Medium (Smaller Window)
				Screen.SetResolution(mediumRes.width, mediumRes.height, FullScreenMode.Windowed);
				Debug.Log("Applied: Medium Windowed");
				break;
		}
	}

	// --- BASE LOGIC ---
	public void StartGame() => SceneManager.LoadScene("MenuToPrologueTransition");
	public void ExitGame() => Application.Quit();

	// --- VOLUME LOGIC ---
	public void SetVolume() { AudioListener.volume = VolumeSlider.value; SaveVolume(); }
	public void SaveVolume() => PlayerPrefs.SetFloat("soundVolume", VolumeSlider.value);
	public void LoadVolume() { if (VolumeSlider != null) VolumeSlider.value = PlayerPrefs.GetFloat("soundVolume"); }
}