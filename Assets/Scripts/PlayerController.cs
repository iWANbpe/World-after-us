using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float speedUpKoef = 1f;
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

    private float gravity = -9.81f;
    private float velocity;
    
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new InputActionsPlayer();

        inputActions.Player.Moving.started += HorizontalMoving;
        inputActions.Player.Moving.performed += HorizontalMoving;
        inputActions.Player.Moving.canceled += HorizontalMoving;

        inputActions.Player.MouseRotation.started += InputMouseRotation;
        inputActions.Player.MouseRotation.performed += InputMouseRotation;
        inputActions.Player.MouseRotation.canceled += InputMouseRotation;
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

    private void Move()
    {

        characterController.Move(curPosition * speed * Time.fixedDeltaTime);
    }

    private void Rotation()
    {
        transform.rotation = Quaternion.Euler(0f, mouseRotation.x, 0f);
        cam.transform.localRotation = Quaternion.Euler( -Mathf.Clamp(mouseRotation.y, 0f, 90f), 0f, 0f);
    }

    void FixedUpdate()
    {
        Move();
        Rotation();
    }
}
