using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
	[Header("Settings")]
	public float lifeTime = 5f;

	private bool hasHit = false;

	void Start()
	{
		Destroy(gameObject, lifeTime);

		// 1. FIND PLAYER TO IGNORE
		// This ensures the spear doesn't shove the player sideways when spawning
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			Collider playerCollider = player.GetComponent<Collider>();
			Collider myCollider = GetComponent<Collider>();

			if (playerCollider != null && myCollider != null)
			{
				Physics.IgnoreCollision(playerCollider, myCollider);
			}
		}
		else
		{
			Debug.LogError("Spear could not find object with tag 'Player'! Did you forget to tag your character?");
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		// 2. DEBUG WHAT WE HIT
		// If the spear stops instantly, check the Console to see what it hit
		Debug.Log("Spear hit object: " + collision.gameObject.name);

		if (hasHit) return;
		if (collision.gameObject.CompareTag("Player")) return;

		hasHit = true;

		// 3. STOP PHYSICS
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true;
			rb.linearVelocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}

		// 4. STICK TO TARGET
		transform.SetParent(collision.transform);
	}
}