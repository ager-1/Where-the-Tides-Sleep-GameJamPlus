using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
	[Header("Settings")]
	public float lifeTime = 5f;

	private bool hasHit = false;

	void Start()
	{
		// 1. Destroy self after 'lifeTime' seconds
		Destroy(gameObject, lifeTime);

		// Ensure physics is ON
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = false;
			rb.useGravity = true;
		}

		// 2. IGNORE PLAYER
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
	}

	// TRIGGER EVENT (Spear is a Trigger, so it passes through things)
	void OnTriggerEnter(Collider other)
	{
		// Ignore Player and non-Enemies (Pass through walls/floor)
		if (other.CompareTag("Player")) return;
		if (!other.CompareTag("Enemy")) return;

		if (hasHit) return;
		hasHit = true;

		// --- DEAL DAMAGE TO SHARK ---
		SharkBoss shark = other.GetComponent<SharkBoss>();
		if (shark != null)
		{
			shark.TakeDamage();
		}
		// ---------------------------

		Debug.Log("Spear stabbed Enemy: " + other.name);

		// 3. STOP PHYSICS (Freeze position)
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true;
			rb.useGravity = false;
			rb.linearVelocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}

		// 4. STICK TO TARGET
		transform.SetParent(other.transform);
	}

	// Optional: Keeps the spear pointing forward as it flies (Arc)
	void FixedUpdate()
	{
		if (!hasHit)
		{
			Rigidbody rb = GetComponent<Rigidbody>();
			if (rb != null && rb.linearVelocity.sqrMagnitude > 1f)
			{
				transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
			}
		}
	}
}