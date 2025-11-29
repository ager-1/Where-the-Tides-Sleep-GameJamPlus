using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class AnimationStateController : MonoBehaviour
{
	// --- SHIP VARIABLES ---
	[Header("Ship Settings")]
	public Transform shipTransform;
	private Vector3 lastShipPosition;

	// --- MOVEMENT VARIABLES ---
	[Header("Movement Settings")]
	public float rotationSpeed = 5.0f;
	public float speed = 5.0f;
	public float runMultiplier = 3.0f;

	// --- COMBAT VARIABLES (NEW) ---
	[Header("Combat Settings")]
	public Transform handSpawnPoint; // Drag "SpearSpawnPoint" here
	public GameObject spearPrefab;   // Drag "Spear" prefab here
	public float throwForce = 25f;
	public float throwDelay = 0.4f;  // Time before spear appears
	public float recoveryTime = 1.0f; // Time stuck after throwing
	public Vector3 spawnRotationOffset = new Vector3(0, 0, 0); // Fixes rotation

	// Internal States
	private bool movementLocked = false;
	private bool isThrowing = false;

	// References
	PlayerInput playerInput;
	CharacterController characterController;
	Animator animator;
	Transform cameraTransform;

	// Inputs
	Vector2 currentMovementInput;
	Vector3 currentMovement;
	Vector3 currentRunMovement;
	bool isRunPressed;
	bool isMovementPressed;

	// Animator Hashes
	int isWalkingHash;
	int isRunningHash;
	int throwHash; // NEW

	void Awake()
	{
		playerInput = new PlayerInput();
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();

		if (Camera.main != null) cameraTransform = Camera.main.transform;

		// Setup IDs for faster animation
		isWalkingHash = Animator.StringToHash("isWalking");
		isRunningHash = Animator.StringToHash("isRunning");
		throwHash = Animator.StringToHash("throw"); // NEW

		// --- MOVEMENT INPUTS ---
		playerInput.CharacterControls.Move.started += onMovementInput;
		playerInput.CharacterControls.Move.canceled += onMovementInput;
		playerInput.CharacterControls.Move.performed += onMovementInput;
		playerInput.CharacterControls.Run.started += onRun;
		playerInput.CharacterControls.Run.canceled += onRun;
		playerInput.CharacterControls.Run.performed += onRun;

		// --- COMBAT INPUTS (NEW) ---
		playerInput.CharacterControls.Throw.started += OnThrow;
	}

	void Start()
	{
		if (shipTransform != null) lastShipPosition = shipTransform.position;
	}

	// --- INPUT EVENTS ---
	void onMovementInput(InputAction.CallbackContext context)
	{
		currentMovementInput = context.ReadValue<Vector2>();
		if (!movementLocked)
		{
			isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
		}
	}

	void onRun(InputAction.CallbackContext context)
	{
		isRunPressed = context.ReadValueAsButton();
	}

	void OnThrow(InputAction.CallbackContext context)
	{
		// 1. Check if we can throw
		if (isThrowing || movementLocked) return;

		// 2. Lock everything
		isThrowing = true;
		movementLocked = true;

		// 3. Trigger Animation
		animator.SetTrigger(throwHash);

		// 4. Start the physical spawn routine
		StartCoroutine(SpawnSpearRoutine());
	}

	// --- LOGIC LOOP ---
	void Update()
	{
		HandleMovementCalculation();
		handleRotation();
		handleAnimation();
		handleGravity();
		ApplyFinalMovement();
	}

	// --- HELPER FUNCTIONS ---

	void HandleMovementCalculation()
	{
		if (!movementLocked && cameraTransform != null)
		{
			// Move relative to Camera
			Vector3 cameraForward = cameraTransform.forward;
			Vector3 cameraRight = cameraTransform.right;
			cameraForward.y = 0;
			cameraRight.y = 0;
			cameraForward.Normalize();
			cameraRight.Normalize();

			Vector3 moveDirection = (cameraForward * currentMovementInput.y + cameraRight * currentMovementInput.x).normalized;

			currentMovement.x = moveDirection.x * speed;
			currentMovement.z = moveDirection.z * speed;
			currentRunMovement.x = moveDirection.x * speed * runMultiplier;
			currentRunMovement.z = moveDirection.z * speed * runMultiplier;
		}
		else
		{
			// If locked, stop X/Z movement immediately
			currentMovement.x = 0;
			currentMovement.z = 0;
			currentRunMovement.x = 0;
			currentRunMovement.z = 0;
			isMovementPressed = false;
		}
	}

	void handleRotation()
	{
		if (movementLocked) return;

		Vector3 positionToLookAt;
		positionToLookAt.x = currentMovement.x;
		positionToLookAt.y = 0.0f;
		positionToLookAt.z = currentMovement.z;
		Quaternion currentRotation = transform.rotation;

		if (isMovementPressed)
		{
			Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
			transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
		}
	}

	void handleAnimation()
	{
		// If locked (Throwing), force walk/run to false
		if (movementLocked)
		{
			animator.SetBool(isWalkingHash, false);
			animator.SetBool(isRunningHash, false);
			return;
		}

		bool isWalking = animator.GetBool(isWalkingHash);
		bool isRunning = animator.GetBool(isRunningHash);

		if (isMovementPressed && !isWalking) animator.SetBool(isWalkingHash, true);
		else if (!isMovementPressed && isWalking) animator.SetBool(isWalkingHash, false);

		if ((isMovementPressed && isRunPressed) && !isRunning) animator.SetBool(isRunningHash, true);
		else if ((!isMovementPressed || !isRunPressed) && isRunning) animator.SetBool(isRunningHash, false);
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

	void ApplyFinalMovement()
	{
		// Calculate Ship Offset
		Vector3 shipMovement = Vector3.zero;
		if (shipTransform != null)
		{
			shipMovement = shipTransform.position - lastShipPosition;
			lastShipPosition = shipTransform.position;
		}

		// Apply Player Move + Ship Move
		if (isRunPressed && isMovementPressed)
		{
			characterController.Move((currentRunMovement * Time.deltaTime) + shipMovement);
		}
		else
		{
			characterController.Move((currentMovement * Time.deltaTime) + shipMovement);
		}
	}

	// --- COMBAT ROUTINE ---
	IEnumerator SpawnSpearRoutine()
	{
		// Wait for arm to swing forward
		yield return new WaitForSeconds(throwDelay);

		if (spearPrefab != null && handSpawnPoint != null)
		{
			// 1. Spawn
			GameObject currentSpear = Instantiate(spearPrefab, handSpawnPoint.position, transform.rotation);

			// 2. Fix Rotation (Apply your offset here)
			currentSpear.transform.Rotate(spawnRotationOffset);

			// 3. Throw Physics
			Rigidbody rb = currentSpear.GetComponent<Rigidbody>();
			if (rb != null)
			{
				// Throw slightly up and forward relative to player
				Vector3 throwDirection = transform.forward + Vector3.up * 0.1f;
				rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
			}
		}

		// Wait for animation to finish before unlocking
		yield return new WaitForSeconds(recoveryTime);

		// Unlock everything
		movementLocked = false;
		isThrowing = false;
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