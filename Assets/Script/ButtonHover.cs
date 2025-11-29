using UnityEngine;
using UnityEngine.EventSystems; // Required for detecting hover
using System.Collections;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[Header("References")]
	[Tooltip("The text or object you want to scale up (usually this object)")]
	public RectTransform targetToScale;
	[Tooltip("The underline Image object (must be a child)")]
	public RectTransform underlineImage;

	[Header("Animation Settings")]
	public float scaleAmount = 1.1f; // How big the text pops (1.1 = 110%)
	public float animationSpeed = 10f; // How fast it moves

	[Header("Underline Settings")]
	[Range(0f, 1f)]
	[Tooltip("1 = Full Width, 0.5 = Half Width")]
	public float underlineWidth = 1f; // Controls the final length of the line

	private Vector3 originalScale;
	private Coroutine currentCoroutine;

	void Start()
	{
		// Remember the starting size
		if (targetToScale == null) targetToScale = GetComponent<RectTransform>();
		originalScale = targetToScale.localScale;

		// Hide the underline instantly at start
		if (underlineImage != null)
		{
			underlineImage.localScale = new Vector3(0, 1, 1);
		}
	}

	// Called when mouse enters the button area
	public void OnPointerEnter(PointerEventData eventData)
	{
		// Stop any currently running animation to avoid glitches
		if (currentCoroutine != null) StopCoroutine(currentCoroutine);

		// Start animating TO the hover state
		// We use 'underlineWidth' here to control how long the line gets
		Vector3 targetUnderlineScale = new Vector3(underlineWidth, 1, 1);

		currentCoroutine = StartCoroutine(AnimateButton(Vector3.one * scaleAmount, targetUnderlineScale));
	}

	// Called when mouse leaves the button area
	public void OnPointerExit(PointerEventData eventData)
	{
		if (currentCoroutine != null) StopCoroutine(currentCoroutine);

		// Start animating BACK to normal (Scale Normal, Underline Hidden)
		currentCoroutine = StartCoroutine(AnimateButton(originalScale, new Vector3(0, 1, 1)));
	}

	// The actual animation logic
	IEnumerator AnimateButton(Vector3 targetScale, Vector3 targetUnderlineScale)
	{
		// While we are not close enough to the target values...
		// Check both text scale AND underline scale to ensure both finish
		while (Vector3.Distance(targetToScale.localScale, targetScale) > 0.01f ||
			   (underlineImage != null && Vector3.Distance(underlineImage.localScale, targetUnderlineScale) > 0.01f))
		{
			// Smoothly interpolate the scale (Pop effect)
			targetToScale.localScale = Vector3.Lerp(targetToScale.localScale, targetScale, Time.deltaTime * animationSpeed);

			// Smoothly interpolate the underline (Slide effect)
			if (underlineImage != null)
			{
				underlineImage.localScale = Vector3.Lerp(underlineImage.localScale, targetUnderlineScale, Time.deltaTime * animationSpeed);
			}

			yield return null; // Wait for next frame
		}

		// Snap to exact values at the end to be clean
		targetToScale.localScale = targetScale;
		if (underlineImage != null) underlineImage.localScale = targetUnderlineScale;
	}
}