using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float speedUpKoef = 1f;
    [SerializeField] private float speedAcceleration = 20f;
    [SerializeField] private float crouchSpeedKoef = 0.5f;
    [SerializeField] private float gravityKoef = 3f;
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float minRotationAngle = 0f;
    [SerializeField] private float maxRotationAngle = 90f;
    [SerializeField] private float characterNormalHeight = 2f;
    [SerializeField] private float characterCrouchHeight = 1.25f;
    [SerializeField] private float characterNormalCenterY = 0f;
    [SerializeField] private float characterCrouchCenterY = -0.4f;
    [SerializeField] private float camNormalY = 0.68f;
    [SerializeField] private float camCrouchY = 0.131f;
    [SerializeField] private float distanceOfInteraction = 1f;
    [Header("Objects")]
    [SerializeField] private GameObject cam;

    private InputActionsPlayer inputActions;
    private CharacterController characterController;
    
    private Vector3 curPosition;
    private Vector2 moveInputHorizontal;
    private Vector2 mouseRotation;

    private float currentSpeed;
    private const float gravity = -9.81f;
    private float grav_Velocity;
    private bool isSprinting;
    private bool isCrouch;

    private GameObject UIController;
    private PlayerUI playerUI;
    private bool isInventoryOpen;

    private RaycastHit hit;
    private GameObject lookObject;
    
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new InputActionsPlayer();
        UIController = GameObject.Find("UI Controller");
        playerUI = GetComponent<PlayerUI>();
        
        isInventoryOpen = false;

        HideCursor();
        UIController.GetComponent<Layouts>().OpenLayout(LayoutType.PlayerPanel);
        playerUI.DisableInfoItemText();
        InventoryChangeStatement(isInventoryOpen);

        //input actions Player
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

        inputActions.Player.Crouch.started += Crouch;
        inputActions.Player.Crouch.performed += Crouch;
        inputActions.Player.Crouch.canceled += Crouch;

        inputActions.Player.InventoryOpen.started += InventoryInteraction;

        //input actions UI
        inputActions.UI.InventoryClose.started += InventoryInteraction;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void HideCursor() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ShowCursor() 
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        if (characterController.isGrounded && !isCrouch)
        {
            grav_Velocity += jumpForce;
        }
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        isSprinting = context.started || context.performed;
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        isCrouch = context.started || context.performed;
        Crouching();
    }

    private void Move()
    {
        float targetSpeed;
        if (isCrouch) targetSpeed = speed * crouchSpeedKoef;
        else if (isSprinting) targetSpeed = speed * speedUpKoef;
        else targetSpeed = speed;
        
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

    private void Look() 
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, distanceOfInteraction))
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            //print(hit.collider.gameObject.name);

            if (hit.collider.gameObject.GetComponent<ItemInfoHolder>())
            {
                lookObject = hit.collider.gameObject;
                playerUI.EnableInfoItemText(lookObject.GetComponent<ItemInfoHolder>().itemInfo.name);
            }
            else
            {
                lookObject = null;
                playerUI.DisableInfoItemText();
            }
        }
        else if (playerUI.infoItemTextIsActive) playerUI.DisableInfoItemText();
    }

    private void Crouching()
    {
        if (isCrouch)
        {
            characterController.height = characterCrouchHeight;
            characterController.center = new Vector3(0f, characterCrouchCenterY, 0f);
            cam.transform.localPosition = new Vector3(0f, camCrouchY, 0f);
        }

        else
        {
            characterController.height = characterNormalHeight;
            characterController.center = new Vector3(0f, characterNormalCenterY, 0f);
            cam.transform.localPosition = new Vector3(0f, camNormalY, 0f);
        }
    }
    
    private void InventoryInteraction(InputAction.CallbackContext context) 
    {
        isInventoryOpen = !isInventoryOpen;
        InventoryChangeStatement(isInventoryOpen);
    }

    private void InventoryChangeStatement(bool inventoryStatement) 
    {
        switch (inventoryStatement) 
        {
            case true:
                UIController.GetComponent<Layouts>().OpenLayout(LayoutType.Inventory);
                ShowCursor();
                inputActions.Player.Disable();
                inputActions.UI.Enable();
                break;
            case false:
                UIController.GetComponent<Layouts>().OpenLayout(LayoutType.PlayerPanel);
                HideCursor();
                inputActions.Player.Enable();
                inputActions.UI.Disable();
                break;
        }
    }

    void FixedUpdate()
    {
        Move();
        Look();
        Rotation();
        ApplyGravity();
    }
}
