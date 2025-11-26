using UnityEngine;

public class SimpleFloater : MonoBehaviour
{
	[Header("Bobbing Settings")]
	[Tooltip("How high and low the object moves (Height in units)")]
	public float amplitude = 0.5f;

	[Tooltip("How fast the object bobs up and down")]
	public float frequency = 1f;

	[Header("Rotation Settings")]
	[Tooltip("Degrees of rotation for the subtle wobble")]
	public float rotationStrength = 5f;

	[Tooltip("How fast the wobble is")]
	public float rotationSpeed = 0.5f;

	// Internal variables to store starting position
	private Vector3 startPos;
	private Vector3 startRot;
	private float randomOffset;

	void Start()
	{
		// Remember where the object started so we can bob relative to that spot
		startPos = transform.position;
		startRot = transform.eulerAngles;

		// Add a random offset so multiple objects don't bob exactly in sync
		randomOffset = Random.Range(0f, 5f);
	}

	void Update()
	{
		// 1. Position Bobbing (Y-axis)
		// We use Mathf.Sin to get a value between -1 and 1 over time
		float newY = startPos.y + Mathf.Sin((Time.time + randomOffset) * frequency) * amplitude;

		// Apply the new position
		transform.position = new Vector3(startPos.x, newY, startPos.z);

		// 2. Rotation Wobble (Optional but looks nice)
		// We use two different Sine waves for X and Z to make it feel organic
		float wobbleX = Mathf.Sin(Time.time * rotationSpeed + randomOffset) * rotationStrength;
		float wobbleZ = Mathf.Cos(Time.time * rotationSpeed * 0.8f + randomOffset) * rotationStrength;

		transform.rotation = Quaternion.Euler(startRot.x + wobbleX, startRot.y, startRot.z + wobbleZ);
	}
}