using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float speedUpKoef = 1f;
    [SerializeField] private float speedAcceleration = 20f;
    [SerializeField] private float gravityKoef = 3f;
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float minRotationAngle = 0f;
    [SerializeField] private float maxRotationAngle = 90f;
    [Header("Objects")]
    [SerializeField] private GameObject cam;

    private InputActionsPlayer inputActions;
    private CharacterController characterController;
    
    private Vector3 curPosition;
    private Vector2 moveInputHorizontal;
    private Vector2 mouseRotation;

    private float currentSpeed;
    private float gravity = -9.81f;
    private float grav_Velocity;
    private bool isSprinting;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new InputActionsPlayer();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inputActions.Player.Moving.started += HorizontalMoving;
        inputActions.Player.Moving.performed += HorizontalMoving;
        inputActions.Player.Moving.canceled += HorizontalMoving;

        inputActions.Player.MouseRotation.started += InputMouseRotation;
        inputActions.Player.MouseRotation.performed += InputMouseRotation;
        inputActions.Player.MouseRotation.canceled += InputMouseRotation;

        inputActions.Player.Jump.started += Jump;

        inputActions.Player.Sprint.started += Sprint;
        inputActions.Player.Sprint.performed += Sprint;
        inputActions.Player.Sprint.canceled += Sprint;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void HorizontalMoving(InputAction.CallbackContext context)
    {
        moveInputHorizontal = context.ReadValue<Vector2>();
        curPosition.x = moveInputHorizontal.x;
        curPosition.z = moveInputHorizontal.y;
        curPosition = transform.TransformDirection(curPosition);
    }

    private void InputMouseRotation(InputAction.CallbackContext context)
    {
        mouseRotation += context.ReadValue<Vector2>() * mouseSensitivity;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded)
        {
            grav_Velocity += jumpForce;
        }
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        isSprinting = context.started || context.performed;
    }

    private void Move()
    {
        float targetSpeed = isSprinting ? speed * speedUpKoef : speed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedAcceleration * Time.fixedDeltaTime);

        characterController.Move( new Vector3(curPosition.x * currentSpeed, curPosition.y, curPosition.z * currentSpeed) * Time.fixedDeltaTime);
    }

    private void Rotation()
    {
        transform.rotation = Quaternion.Euler(0f, mouseRotation.x, 0f);
        cam.transform.localRotation = Quaternion.Euler( -Mathf.Clamp(mouseRotation.y, minRotationAngle, maxRotationAngle), 0f, 0f);
    }

    private void ApplyGravity()
    {
        if(characterController.isGrounded && grav_Velocity < 0f)
        {
            grav_Velocity = -1f;
        }

        else
        {
            grav_Velocity += gravity * gravityKoef * Time.fixedDeltaTime;
        }

        curPosition.y = grav_Velocity;
    }

    void FixedUpdate()
    {
        Move();
        Rotation();
        ApplyGravity();
    }
}
