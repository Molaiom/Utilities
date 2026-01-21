using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonMovement : MonoBehaviour
{
	[SerializeField] private InputActionAsset InputActions;
	[SerializeField] private float movementSpeed = 10;
	[SerializeField] private float turningSpeed = 10;

	[Header("Camera attributes")]
	[Tooltip("The object the camera will follow and rotate with")]
	[SerializeField] private Transform shoulderTransform;
	[SerializeField] private float cameraSensitivity = 0.2f;
	[SerializeField] private float maxCameraAngle = 70;

	[Header("Jump / ground check attributes")]
	[SerializeField] private float jumpForce = 7;
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
	private float cameraXAngle = 0;
	private float cameraYAngle = 0;
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
		if (moveInputValue.magnitude <= 0)
			return;

		// Moves relative to the shoulder/camera rotation
		Vector3 moveDirection = moveInputValue.y * shoulderTransform.forward + moveInputValue.x * shoulderTransform.right;
		moveDirection.y = 0;
		moveDirection.Normalize();
		characterRigidbody.MovePosition(characterRigidbody.position + (moveDirection * movementSpeed * Time.deltaTime));

		RotateCharacter(ref moveDirection);
	}

	private void RotateCharacter(ref Vector3 rotateDirection)
	{
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
