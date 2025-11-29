using UnityEngine;
using UnityEngine.SceneManagement; // Needed for loading scenes

public class SkyboxTimeController : MonoBehaviour
{
	[Header("Skybox Settings")]
	public Material morningSkybox;
	public Material nightSkybox;

	[Header("Fog Settings")]
	[Tooltip("Click the color box in the Inspector to set the Morning fog color manually")]
	public Color morningFogColor = Color.gray;

	[Tooltip("Click the color box in the Inspector to set the Night fog color manually")]
	public Color nightFogColor = Color.black;

	[Header("Light Settings")]
	[Tooltip("Drag your Directional Light (Sun) here from the Hierarchy")]
	public Light directionalLight;
	[Tooltip("Brightness of the sun during the morning phase")]
	public float morningLightIntensity = 1.0f;
	[Tooltip("Brightness of the sun during the night phase")]
	public float nightLightIntensity = 0.2f;

	[Header("Timing Settings (in seconds)")]
	public float morningDuration = 180f; // 3 Minutes
	public float nightDuration = 120f;   // 2 Minutes

	[Header("Scene Management")]
	public string nextSceneName; // Type the EXACT name of your next scene here

	private float timer;
	private bool hasSwitchedToNight = false;

	void Start()
	{
		// 1. Apply Morning Settings immediately on Start
		if (morningSkybox != null)
		{
			RenderSettings.skybox = morningSkybox;
			RenderSettings.fogColor = morningFogColor; // Applies the color you picked in Inspector
			DynamicGI.UpdateEnvironment();
		}

		// Apply Morning Light Intensity
		if (directionalLight != null)
		{
			directionalLight.intensity = morningLightIntensity;
		}

		timer = 0f;
	}

	void Update()
	{
		timer += Time.deltaTime;

		// CHECK 1: Is Morning over? (Switch to Night)
		if (!hasSwitchedToNight && timer >= morningDuration)
		{
			SwitchToNight();
		}

		// CHECK 2: Is Night over? (Load Next Scene)
		if (timer >= (morningDuration + nightDuration))
		{
			LoadNextScene();
		}
	}

	void SwitchToNight()
	{
		hasSwitchedToNight = true;

		if (nightSkybox != null)
		{
			RenderSettings.skybox = nightSkybox;
			RenderSettings.fogColor = nightFogColor; // Applies the color you picked in Inspector
			DynamicGI.UpdateEnvironment();
		}
		else
		{
			Debug.LogWarning("Night Skybox is missing in the Inspector!");
		}

		// Apply Night Light Intensity
		if (directionalLight != null)
		{
			directionalLight.intensity = nightLightIntensity;
		}
	}

	void LoadNextScene()
	{
		if (!string.IsNullOrEmpty(nextSceneName))
		{
			SceneManager.LoadScene(nextSceneName);
		}
		else
		{
			Debug.LogError("Please type the Next Scene Name in the Inspector!");
		}
	}
}