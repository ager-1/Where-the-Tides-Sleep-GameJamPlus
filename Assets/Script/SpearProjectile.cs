using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
	public float damage = 10f;
	public float lifeTime = 5f; // Disappear after 5 seconds

	void Start()
	{
		Destroy(gameObject, lifeTime); // Auto-delete to save memory
	}

	void OnCollisionEnter(Collision collision)
	{
		// Optional: Stick to the object it hit?
		// For now, let's just stop physics so it doesn't bounce weirdly
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true; // Stop moving
			rb.linearVelocity = Vector3.zero; // Unity 6 syntax (use .velocity in older versions)

			// Stick to the target
			transform.SetParent(collision.transform);
		}
	}
}