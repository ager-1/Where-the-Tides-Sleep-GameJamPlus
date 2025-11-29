using UnityEngine;

public class ShipMovementSync : MonoBehaviour
{
	public Transform shipTransform; // Drag your Ship object here in Inspector
	public CharacterController characterController;

	private Vector3 lastShipPosition;

	void Start()
	{
		if (shipTransform != null) lastShipPosition = shipTransform.position;
	}

	void LateUpdate() // LateUpdate runs AFTER the ship has moved
	{
		if (shipTransform != null)
		{
			// Calculate exactly how far the ship moved this frame
			Vector3 shipMovement = shipTransform.position - lastShipPosition;

			// Force the CharacterController to move that same amount (External Motion)
			// We use MinMoveDistance 0 to ensure even tiny movements are caught
			characterController.Move(shipMovement);

			// Update for next frame
			lastShipPosition = shipTransform.position;
		}
	}
}