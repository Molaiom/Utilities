using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private float movementSpeed = 10;
	[SerializeField] private float cameraSensitivity = 1;
	[SerializeField] private float cameraMaxAngle = 80;
	[SerializeField] private Transform cameraTransform;
	private float verticalRotation = 0;
	private Rigidbody playerRigidbody;

	void Awake()
	{
		playerRigidbody = GetComponent<Rigidbody>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void FixedUpdate()
	{
		MoveCharacter();
	}

	private void Update()
	{
		RotateCamera();
	}

	private void MoveCharacter()
	{
		Vector3 verticalInput = Input.GetAxis("Vertical") * transform.forward;
		Vector3 horizontalInput = Input.GetAxis("Horizontal") * transform.right;

		Vector3 movementInput = verticalInput + horizontalInput;
		if (movementInput.magnitude > 1)
			movementInput.Normalize();

		playerRigidbody.MovePosition(playerRigidbody.position + (movementInput * movementSpeed * Time.deltaTime));
	}

	private void RotateCamera()
	{
		// HORIZONTAL ROTATION
		float rotateHorizontal = Input.GetAxis("Mouse X") * cameraSensitivity;
		transform.Rotate(0, rotateHorizontal, 0);

		// VERTICAL ROTATION
		verticalRotation -= Input.GetAxis("Mouse Y") * cameraSensitivity;
		verticalRotation = Mathf.Clamp(verticalRotation, -cameraMaxAngle, cameraMaxAngle);
		cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
	}
}
