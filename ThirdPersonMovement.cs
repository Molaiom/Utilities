using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonMovement : MonoBehaviour
{
	[SerializeField] private InputActionAsset InputActions;
	[SerializeField] private float movementSpeed = 15f;
	[SerializeField] private float turningSpeed = 10f;

	[Header("Camera attributes")]
	[Tooltip("The object the camera will follow and rotate with")]
	[SerializeField] private Transform shoulderTransform;
	[SerializeField] private float cameraSensitivity = 0.2f;
	[SerializeField] private float maxCameraAngle = 70f;

	[Header("Jump / ground check attributes")]
	[SerializeField] private float jumpForce = 7f;
	[Tooltip("How fast will this character fall")]
	[SerializeField] private float smoothFallForce = 20f;
	[SerializeField] private float groundCheckOffset = 0.65f;
	[SerializeField] private float groundCheckRadius = 0.45f;
	[SerializeField] private LayerMask groundCheckLayers;

	private InputAction moveInputAction;
	private InputAction lookInputAction;
	private InputAction jumpInputAction;
	private Vector2 moveInputValue;
	private Vector2 lookInputValue;

	private Rigidbody characterRigidbody;
	private float cameraXAngle = 0f;
	private float cameraYAngle = 0f;
	private bool isGrounded;

	private void Awake()
	{
		characterRigidbody = GetComponent<Rigidbody>();
		moveInputAction = InputSystem.actions.FindAction("Move");
		lookInputAction = InputSystem.actions.FindAction("Look");
		jumpInputAction = InputSystem.actions.FindAction("Jump");
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void OnEnable()
	{
		InputActions.FindActionMap("Player").Enable();
	}

	private void OnDisable()
	{
		InputActions.FindActionMap("Player").Disable();
	}

	private void FixedUpdate()
	{
		MoveCharacter();
		GroundCheck();
		SmoothFall();
	}

	private void Update()
	{
		moveInputValue = moveInputAction.ReadValue<Vector2>();
		lookInputValue = lookInputAction.ReadValue<Vector2>();
		if (jumpInputAction.WasPressedThisFrame())
			Jump();
	}

	private void LateUpdate()
	{
		RotateCamera();

	}

	private void MoveCharacter()
	{	
		Vector3 moveDirection = Vector3.zero;

		// Move the player relative to the should/camera
		if (moveInputValue.magnitude > 0)
		{
			moveDirection = (moveInputValue.y * transform.forward + moveInputValue.x * transform.right).normalized;
			RotateCharacter(moveDirection);
		}
		// Reduce the player speed to prevent "sliding"
		else if (moveInputValue.magnitude <= 0 && isGrounded)
			moveDirection = -characterRigidbody.linearVelocity;

		
		moveDirection.y = 0;
		characterRigidbody.AddForce(moveDirection * movementSpeed * Time.deltaTime, ForceMode.VelocityChange);
	}

	private void RotateCharacter(Vector3 rotateDirection)
	{
		rotateDirection.y = 0;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotateDirection, Vector3.up), turningSpeed * Time.deltaTime);
	}

	private void RotateCamera()
	{
		// Intended to be used with a third person follow cinemachine camera
		cameraYAngle += lookInputValue.x * cameraSensitivity;
		cameraXAngle -= lookInputValue.y * cameraSensitivity;
		cameraXAngle = Mathf.Clamp(cameraXAngle, -maxCameraAngle, maxCameraAngle);

		// The camera will follow the shoulderTransform's rotation
		shoulderTransform.rotation = Quaternion.Euler(cameraXAngle, cameraYAngle, 0);
	}

	private void GroundCheck()
	{
		Vector3 groundCheckPosition = new(transform.position.x, transform.position.y - groundCheckOffset, transform.position.z);

		if (Physics.CheckSphere(groundCheckPosition, groundCheckRadius, groundCheckLayers, QueryTriggerInteraction.Ignore))
			isGrounded = true;
		else
			isGrounded = false;
	}

	private void Jump()
	{
		if (isGrounded)
			characterRigidbody.AddForceAtPosition(new Vector3(0, jumpForce, 0), Vector3.up, ForceMode.Impulse);
	}

	private void SmoothFall()
	{
		// Makes the character fall faster so it doesn't feel "floaty" when jumping
		if (!isGrounded && characterRigidbody.linearVelocity.y < 0)
			characterRigidbody.AddForce(Vector3.down * smoothFallForce, ForceMode.Acceleration);
	}

	#region Gizmos
	private void OnDrawGizmosSelected()
	{
		Vector3 groundCheckPosition = new(transform.position.x, transform.position.y - groundCheckOffset, transform.position.z);
		Gizmos.color = Color.azure;
		Gizmos.DrawWireSphere(groundCheckPosition, groundCheckRadius);
	}

	#endregion
}


