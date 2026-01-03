using UnityEngine;

public class CannonAimController : MonoBehaviour
{
    [Header("Assignments")]
    public Transform cannonTransform;
    public Texture2D crosshairTexture;

    [Header("Settings")]
    public float rotationSpeed = 10f;
    public float maxAngle = 30f; // The clamp limit (e.g., 30 degrees left/right)

    // Store the "Zero" direction (where the cannon faced when the game started)
    private Vector3 initialForward;

    void Start()
    {
        // 1. Remember the starting direction so we can clamp relative to it
        if (cannonTransform != null)
        {
            initialForward = cannonTransform.forward;
            initialForward.y = 0; // Flatten it just in case
        }

        // Setup Cursor
        if (crosshairTexture != null)
        {
            Vector2 hotspot = new Vector2(crosshairTexture.width / 2, crosshairTexture.height / 2);
            Cursor.SetCursor(crosshairTexture, hotspot, CursorMode.Auto);
        }
    }

    void Update()
    {
        AimCannonClamped();
    }

    void AimCannonClamped()
    {
        // 1. Math Plane (Infinite floor at cannon height)
        Plane infiniteFloor = new Plane(Vector3.up, cannonTransform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float enter;
        if (infiniteFloor.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            // 2. Get the direction to the mouse
            Vector3 targetDirection = hitPoint - cannonTransform.position;
            targetDirection.y = 0; // Flatten (Y-axis only)

            if (targetDirection.sqrMagnitude > 0.001f)
            {
                // 3. Calculate the angle difference between Start Direction and Mouse Direction
                // SignedAngle gives us -45, +10, -120, etc.
                float angleDifference = Vector3.SignedAngle(initialForward, targetDirection, Vector3.up);

                // 4. Clamp the angle
                float clampedAngle = Mathf.Clamp(angleDifference, -maxAngle, maxAngle);

                // 5. Calculate the final rotation
                // We take the Initial Rotation and rotate it by the Clamped Angle
                Quaternion targetRotation = Quaternion.LookRotation(initialForward) * Quaternion.Euler(0, clampedAngle, 0);

                // 6. Apply Smooth Rotation
                cannonTransform.rotation = Quaternion.Slerp(cannonTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}