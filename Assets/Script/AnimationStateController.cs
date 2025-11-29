using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationStateController : MonoBehaviour
{
	// --- NEW SHIP VARIABLES ---
	[Header("Ship Settings")]
	public Transform shipTransform; // DRAG YOUR SHIP OBJECT HERE IN INSPECTOR
	private Vector3 lastShipPosition;
	// --------------------------

	PlayerInput playerInput;
	Vector2 currentMovementInput;
	Vector3 currentMovement;
	Vector3 currentRunMovement;
	bool isRunPressed;
	bool isMovementPressed;
	CharacterController characterController;
	Animator animator;
	float rotationFactorPerFrame = 15.0f;
	float runMultiplier = 3.0f;
	int isWalkingHash;
	int isRunningHash;

	void Awake()
	{
		playerInput = new PlayerInput();
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();
		isWalkingHash = Animator.StringToHash("isWalking");
		isRunningHash = Animator.StringToHash("isRunning");
		playerInput.CharacterControls.Move.started += onMovementInput;
		playerInput.CharacterControls.Move.canceled += onMovementInput;
		playerInput.CharacterControls.Move.performed += onMovementInput;
		playerInput.CharacterControls.Run.started += onRun;
		playerInput.CharacterControls.Run.canceled += onRun;
		playerInput.CharacterControls.Run.performed += onRun;
	}

	void Start()
	{
		// Initialize ship position to prevent a big jump on frame 1
		if (shipTransform != null)
		{
			lastShipPosition = shipTransform.position;
		}
	}

	void onMovementInput(InputAction.CallbackContext context)
	{
		currentMovementInput = context.ReadValue<Vector2>();
		currentMovement.x = currentMovementInput.x;
		currentMovement.z = currentMovementInput.y;
		currentRunMovement.x = currentMovementInput.x * runMultiplier;
		currentRunMovement.z = currentMovementInput.y * runMultiplier;
		isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
	}

	void onRun(InputAction.CallbackContext context)
	{
		isRunPressed = context.ReadValueAsButton();
	}

	void handleGravity()
	{
		if (characterController.isGrounded)
		{
			float groundedGravity = -.05f;
			currentMovement.y = groundedGravity;
			currentRunMovement.y = groundedGravity;
		}
		else
		{
			float gravity = -9.8f;
			currentMovement.y += gravity;
			currentRunMovement.y += gravity;
		}
	}

	void handleRotation()
	{
		Vector3 positionToLookAt;
		positionToLookAt.x = currentMovement.x;
		positionToLookAt.y = 0.0f;
		positionToLookAt.z = currentMovement.z;
		Quaternion currentRotation = transform.rotation;
		if (isMovementPressed)
		{
			Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
			transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
		}
	}

	void handleAnimation()
	{
		bool isWalking = animator.GetBool(isWalkingHash);
		bool isRunning = animator.GetBool(isRunningHash);
		if (isMovementPressed && !isWalking)
		{
			animator.SetBool(isWalkingHash, true);
		}
		else if (!isMovementPressed && isWalking)
		{
			animator.SetBool(isWalkingHash, false);
		}
		if ((isMovementPressed && isRunPressed) && !isRunning)
		{
			animator.SetBool(isRunningHash, true);
		}
		else if ((!isMovementPressed || !isRunPressed) && isRunning)
		{
			animator.SetBool(isRunningHash, false);
		}
	}

	// Update is called once per frame
	void Update()
	{
		handleRotation();
		handleAnimation();
		handleGravity(); // You forgot to call this in your original script! I added it back.

		// --- CALCULATE SHIP MOVEMENT ---
		Vector3 shipMovement = Vector3.zero;
		if (shipTransform != null)
		{
			shipMovement = shipTransform.position - lastShipPosition;
			lastShipPosition = shipTransform.position;
		}
		// -------------------------------

		if (isRunPressed && isMovementPressed)
		{
			// Add shipMovement to your running movement
			characterController.Move((currentRunMovement * Time.deltaTime) + shipMovement);
		}
		else
		{
			// Add shipMovement to your walking/idle movement
			characterController.Move((currentMovement * Time.deltaTime) + shipMovement);
		}
	}

	void OnEnable()
	{
		playerInput.CharacterControls.Enable();
	}
	void OnDisable()
	{
		playerInput.CharacterControls.Disable();
	}
}