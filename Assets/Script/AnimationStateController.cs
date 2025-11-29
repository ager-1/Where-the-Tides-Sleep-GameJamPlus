using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationStateController : MonoBehaviour
{
	[Header("Ship Settings")]
	public Transform shipTransform;
	private Vector3 lastShipPosition;

	[Header("Movement Settings")]
	public float rotationSpeed = 1.0f; 
	public float speed = 1f;         
	public float runMultiplier = 1f;

	// References
	PlayerInput playerInput;
	CharacterController characterController;
	Animator animator;
	Transform cameraTransform; 

	// Input Variables
	Vector2 currentMovementInput;
	Vector3 currentMovement;
	Vector3 currentRunMovement;
	bool isRunPressed;
	bool isMovementPressed;

	// Animation Hashes
	int isWalkingHash;
	int isRunningHash;

	void Awake()
	{
		playerInput = new PlayerInput();
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();

		if (Camera.main != null)
		{
			cameraTransform = Camera.main.transform;
		}
		else
		{
			Debug.LogError("No Main Camera found! Tag your camera as 'MainCamera'.");
		}

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
		if (shipTransform != null) lastShipPosition = shipTransform.position;
	}

	void onMovementInput(InputAction.CallbackContext context)
	{

		currentMovementInput = context.ReadValue<Vector2>();
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

			transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
		}
	}

	void handleAnimation()
	{
		bool isWalking = animator.GetBool(isWalkingHash);
		bool isRunning = animator.GetBool(isRunningHash);

		if (isMovementPressed && !isWalking) animator.SetBool(isWalkingHash, true);
		else if (!isMovementPressed && isWalking) animator.SetBool(isWalkingHash, false);

		if ((isMovementPressed && isRunPressed) && !isRunning) animator.SetBool(isRunningHash, true);
		else if ((!isMovementPressed || !isRunPressed) && isRunning) animator.SetBool(isRunningHash, false);
	}

	void Update()
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
		// ------------------------------------------------

		handleRotation();
		handleAnimation();
		handleGravity();

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

	void OnEnable()
	{
		playerInput.CharacterControls.Enable();
	}
	void OnDisable()
	{
		playerInput.CharacterControls.Disable();
	}
}