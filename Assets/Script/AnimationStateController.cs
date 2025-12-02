using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement; // Needed for Game Over

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

	// --- COMBAT VARIABLES ---
	[Header("Combat Settings")]
	public Transform handSpawnPoint;
	public GameObject spearPrefab;
	public float throwSpeed = 40f;   // Increased from 20 to 40 for more speed
	public float throwDelay = 0.4f;
	public float recoveryTime = 1.0f;

	[Header("Rotation Fix")]
	[Tooltip("If spear stands up, try (90, 0, 0). If backwards, try (0, 180, 0)")]
	public Vector3 spawnRotationOffset = new Vector3(90, 0, 0);

	// --- NEW: BOSS BATTLE SETTINGS ---
	[Header("Boss Battle Settings")]
	public int maxSpears = 10;
	private int spearsThrown = 0;
	public string gameOverSceneName = "GameOver"; // Name of your lose scene
												  // ---------------------------------

	// Internal States
	[HideInInspector] public bool movementLocked = false;
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
	int throwHash;

	void Awake()
	{
		playerInput = new PlayerInput();
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();

		if (Camera.main != null) cameraTransform = Camera.main.transform;

		isWalkingHash = Animator.StringToHash("isWalking");
		isRunningHash = Animator.StringToHash("isRunning");
		throwHash = Animator.StringToHash("throw");

		playerInput.CharacterControls.Move.started += onMovementInput;
		playerInput.CharacterControls.Move.canceled += onMovementInput;
		playerInput.CharacterControls.Move.performed += onMovementInput;
		playerInput.CharacterControls.Run.started += onRun;
		playerInput.CharacterControls.Run.canceled += onRun;
		playerInput.CharacterControls.Run.performed += onRun;
		playerInput.CharacterControls.Throw.started += OnThrow;
	}

	void Start()
	{
		if (shipTransform != null) lastShipPosition = shipTransform.position;
		Debug.Log("Spears Remaining: " + maxSpears);
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
		// 1. Check Ammo
		if (spearsThrown >= maxSpears)
		{
			Debug.Log("Out of Spears!");
			return;
		}

		if (isThrowing || movementLocked) return;

		// 2. Increment Counter
		spearsThrown++;
		Debug.Log($"Throwing Spear {spearsThrown}/{maxSpears}");

		// 3. Start Throw Logic
		isThrowing = true;
		movementLocked = true;
		animator.SetTrigger(throwHash);
		StartCoroutine(SpawnSpearRoutine());

		// 4. Check for Game Over Condition (If this was the last spear)
		if (spearsThrown >= maxSpears)
		{
			StartCoroutine(CheckGameOverSequence());
		}
	}

	// New Coroutine to handle Game Over delay
	IEnumerator CheckGameOverSequence()
	{
		Debug.Log("Last spear thrown! Waiting for impact...");
		// Wait 5.5 seconds (slightly longer than spear lifetime) to see if it hits
		yield return new WaitForSeconds(5.5f);

		// Check if Shark still exists
		GameObject shark = GameObject.FindGameObjectWithTag("Enemy");
		if (shark != null)
		{
			Debug.Log("GAME OVER: You ran out of spears and the Shark is still alive.");
			SceneManager.LoadScene("GameOver");

			// Uncomment this when you are ready to use the scene
			// SceneManager.LoadScene(gameOverSceneName); 
		}
		else
		{
			Debug.Log("You used all spears, but the Shark is dead. You Win!");
		}
	}

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
		Vector3 shipMovement = Vector3.zero;
		if (shipTransform != null)
		{
			shipMovement = shipTransform.position - lastShipPosition;
			lastShipPosition = shipTransform.position;
		}

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
		yield return new WaitForSeconds(throwDelay);

		if (spearPrefab != null && handSpawnPoint != null)
		{
			Vector3 throwDirection = transform.forward;

			// Spawn looking forward
			GameObject currentSpear = Instantiate(spearPrefab, handSpawnPoint.position, Quaternion.LookRotation(throwDirection));

			// Fix Rotation
			currentSpear.transform.Rotate(spawnRotationOffset);

			// Throw Physics
			Rigidbody rb = currentSpear.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.isKinematic = false;

				// Increased Momentum: Doubled the speed variable above and slightly increased the arc
				// This ensures it travels further and faster
				Vector3 finalVelocity = (throwDirection + Vector3.up * 0.08f).normalized * throwSpeed;
				rb.linearVelocity = finalVelocity;
			}
		}
		else
		{
			Debug.LogError("❌ ERROR: Hand Spawn Point or Spear Prefab is missing in AnimationStateController!");
		}

		yield return new WaitForSeconds(recoveryTime);

		movementLocked = false;
		isThrowing = false;
	}

	void OnEnable() { playerInput.CharacterControls.Enable(); }
	void OnDisable() { playerInput.CharacterControls.Disable(); }
}