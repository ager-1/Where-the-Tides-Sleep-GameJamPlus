using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SharkBoss : MonoBehaviour
{
	[Header("Boss Stats")]
	public int maxHealth = 7;
	private int currentHealth;

	[Header("Movement Settings")]
	public float moveSpeed = 5f;
	public float changePositionInterval = 3f;

	public Vector2 xRange = new Vector2(-60, 60);
	public Vector2 yRange = new Vector2(-18, 5);

	// Original starting position (Center)
	private Vector3 startPos;
	private Vector3 targetPosition;

	[Header("Scene Management")]
	public string winSceneName = "WinScene";

	void Start()
	{
		currentHealth = maxHealth;
		startPos = transform.position; // Memorize center (e.g. -12.92, 4.24, -834.1)
		targetPosition = transform.position;

		StartCoroutine(MoveRoutine());
	}

	void Update()
	{
		transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
	}

	IEnumerator MoveRoutine()
	{
		while (currentHealth > 0)
		{
			// Decision: 50% chance to move X, 50% chance to move Y
			bool moveX = Random.value > 0.5f;

			// Decision 2: Should we return to center?
			// If we are far from startPos, increase chance to pick a center-biased coordinate
			float nextX = transform.position.x;
			float nextY = transform.position.y;

			if (moveX)
			{
				// Pick random X within range
				nextX = Random.Range(xRange.x, xRange.y);

				// Bias: If too far left/right, pull back slightly towards center
				nextX = (nextX + startPos.x) / 2f;
			}
			else
			{
				// Pick random Y within range
				nextY = Random.Range(yRange.x, yRange.y);

				// Bias: Pull back towards center
				nextY = (nextY + startPos.y) / 2f;
			}

			// Set target (Z is always fixed to start Z)
			targetPosition = new Vector3(nextX, nextY, startPos.z);

			yield return new WaitForSeconds(changePositionInterval);
		}
	}

	public void TakeDamage()
	{
		currentHealth--;
		Debug.Log($"Shark Hit! HP: {currentHealth}/{maxHealth}");

		if (currentHealth <= 0)
		{
			WinGame();
		}
	}

	void WinGame()
	{
		Debug.Log("Shark Defeated! YOU WIN!");
		Destroy(gameObject);
		SceneManager.LoadScene(winSceneName);
	}
}