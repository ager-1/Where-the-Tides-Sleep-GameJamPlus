using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public Slider VolumeSlider;
	void Start()
	{
		// Volume 
		if (PlayerPrefs.HasKey("soundVolume"))
		{
			LoadVolume();
		}
		else
		{
			PlayerPrefs.SetFloat("soundVolume", 1);
			LoadVolume();
		}
	}
	//Base Menu Code
	public void StartGame()
	{
		SceneManager.LoadScene("Prologue");
	}
	public void ExitGame()
	{
		Application.Quit();
	}

	//Volume Code
	public void SetVolume()
	{
		AudioListener.volume = VolumeSlider.value;
		SaveVolume();
	}
	public void SaveVolume()
	{
		PlayerPrefs.SetFloat("soundVolume", VolumeSlider.value);
	}
	public void LoadVolume()
	{
		VolumeSlider.value = PlayerPrefs.GetFloat("soundVolume");
	}

}
