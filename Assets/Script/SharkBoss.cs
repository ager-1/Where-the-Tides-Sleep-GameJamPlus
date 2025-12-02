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

	private Vector3 startPos;
	private Vector3 targetPosition;

	[Header("Scene Management")]
	public string winSceneName = "WinScene";   // Loads if you kill the shark
	public string loseSceneName = "GameOver";  // Loads if you run out of spears

	void Start()
	{
		currentHealth = maxHealth;
		startPos = transform.position;
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
			// 50% chance X, 50% chance Y
			bool moveX = Random.value > 0.5f;
			float nextX = transform.position.x;
			float nextY = transform.position.y;

			if (moveX)
			{
				nextX = Random.Range(xRange.x, xRange.y);
				nextX = (nextX + startPos.x) / 2f; // Bias towards center
			}
			else
			{
				nextY = Random.Range(yRange.x, yRange.y);
				nextY = (nextY + startPos.y) / 2f; // Bias towards center
			}

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

	// Called internally when Health hits 0
	void WinGame()
	{
		Debug.Log("Shark Defeated! YOU WIN!");

		// Ensure scene name is valid before loading
		if (!string.IsNullOrEmpty(winSceneName))
		{
			SceneManager.LoadScene(winSceneName);
		}
		else
		{
			Debug.LogError("Win Scene Name is empty in SharkBoss Inspector!");
		}

		// Destroy AFTER loading request to ensure code runs
		Destroy(gameObject);
	}

	// Called by AnimationStateController when you run out of spears
	public void TriggerLoseGame()
	{
		Debug.Log("Player ran out of spears. GAME OVER!");

		if (!string.IsNullOrEmpty(loseSceneName))
		{
			SceneManager.LoadScene(loseSceneName);
		}
		else
		{
			Debug.LogError("Lose Scene Name is empty in SharkBoss Inspector!");
		}
	}
}