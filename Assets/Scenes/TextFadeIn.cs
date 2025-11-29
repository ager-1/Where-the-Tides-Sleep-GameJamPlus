using UnityEngine;
using TMPro; // Required for TextMeshPro
using System.Collections;

public class TextFadeIn : MonoBehaviour
{
	[Header("Settings")]
	public float fadeDuration = 4.0f; // Duration in seconds
	public bool fadeOnStart = true;

	private TextMeshProUGUI tmpText;

	void Awake()
	{
		tmpText = GetComponent<TextMeshProUGUI>();
		if (tmpText == null)
		{
			Debug.LogError("No TextMeshProUGUI component found on this object!");
		}
	}

	void Start()
	{
		if (fadeOnStart && tmpText != null)
		{
			// Ensure alpha is 0 at start
			SetAlpha(0f);
			StartCoroutine(FadeInRoutine());
		}
	}

	public void StartFadeIn()
	{
		if (tmpText != null)
		{
			StopAllCoroutines(); // Stop any existing fade
			StartCoroutine(FadeInRoutine());
		}
	}

	IEnumerator FadeInRoutine()
	{
		float timer = 0f;
		Color originalColor = tmpText.color;

		while (timer < fadeDuration)
		{
			timer += Time.deltaTime;
			// Calculate alpha value between 0 and 1
			float alpha = Mathf.Clamp01(timer / fadeDuration);

			// Update text color
			tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

			yield return null; // Wait for next frame
		}

		// Ensure alpha is exactly 1 at the end
		SetAlpha(1f);
	}

	void SetAlpha(float alpha)
	{
		if (tmpText != null)
		{
			Color c = tmpText.color;
			tmpText.color = new Color(c.r, c.g, c.b, alpha);
		}
	}
}