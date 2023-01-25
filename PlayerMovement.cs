using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float cameraMaxAngle = 80;
    [SerializeField] private Transform cameraTransform;
    private float verticalRotation = 0;
    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
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
        float movement1 = Input.GetAxis("Horizontal");
        float movement2 = Input.GetAxis("Vertical");

        Vector3 Move = new(movement1, 0, movement2);
        Move = transform.rotation * Move;
        characterController.SimpleMove(Move * movementSpeed);
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
