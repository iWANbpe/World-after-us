using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float speedUpKoef = 1f;
    [SerializeField] private float speedAcceleration = 20f;
    [SerializeField] private float crouchSpeedKoef = 0.5f;
    [SerializeField] private float gravityKoef = 3f;
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private float jumpForce = 1f;
    [Header("Rotation")]
    [SerializeField] private float minRotationAngle = 0f;
    [SerializeField] private float maxRotationAngle = 90f;
    [Header("Crouch")]
    [SerializeField] private float characterNormalHeight = 2f;
    [SerializeField] private float characterCrouchHeight = 1.25f;
    [SerializeField] private float characterNormalCenterY = 0f;
    [SerializeField] private float characterCrouchCenterY = -0.4f;
    [SerializeField] private float camNormalY = 0.68f;
    [SerializeField] private float camCrouchY = 0.131f;
    [Header("Interaction")]
    [SerializeField] private float distanceOfInteraction = 1f;
    [SerializeField] private LayerMask interectionMask;
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

    private GameObject canvas;
    private GameObject UIController;
    private PlayerUI playerUI;
    private bool isInventoryOpen;

    private RaycastHit hit;
    private LookObject lookObject;
    private bool isGrabbing;
    private GameObject grabPoint;

    public EventSystem eventSystem;
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;

    void Awake()
    {
        inputActions = new InputActionsPlayer();

        canvas = GameObject.Find("Canvas");
        UIController = GameObject.Find("UI Controller");
        eventSystem = GameObject.Find("/EventSystem").GetComponent<EventSystem>();

        characterController = GetComponent<CharacterController>();
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        playerUI = GetComponent<PlayerUI>();
        
        isInventoryOpen = false;
        isGrabbing = false;

        SetCursorActivity(false);
        playerUI.DisableInfoItemText();
        InventoryChangeStatement(isInventoryOpen);
        UIController.GetComponent<Layouts>().OpenLayout(LayoutType.PlayerPanel);

        grabPoint = cam.transform.Find("GrabPoint").gameObject;
        
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

        inputActions.Player.Grab.started += Grab;
        inputActions.Player.Grab.performed += Grab;
        inputActions.Player.Grab.canceled += Grab;

        inputActions.Player.Take.started += TakeItem;

        //input actions UI
        inputActions.UI.InventoryClose.started += InventoryInteraction;

        inputActions.UI.DropItem.started += DropItem;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void SetCursorActivity(bool state) 
    {
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
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

    private void Grab(InputAction.CallbackContext context) 
    {
        if (lookObject != null) 
        {
            if (context.started) 
            {
                isGrabbing = true;
                lookObject.rigidBody.useGravity = false;
                lookObject.DisableCollisionLayer(LayerMask.GetMask("Player"));
            }

            else if (context.performed) 
            {
                lookObject.SetTarget(grabPoint);
            }

            else
            {
                isGrabbing = false;
                lookObject.rigidBody.useGravity = true;
                lookObject.SetTarget(null);
                lookObject.DisableCollisionLayer(LayerMask.GetMask("Nothing"));
            }

        }
    }

    private void TakeItem(InputAction.CallbackContext contex) 
    { 
        if(lookObject != null && lookObject.isIntractable) 
        {
            lookObject.DespawnItem();
            ObjectPooler.Instance.AddInventoryItem(lookObject.itemInfo.inventoryItemInfo, new Vector3(90f, 90f, 0f));
        }
    }

    private void DropItem(InputAction.CallbackContext contex) 
    {
        GameObject invItem = SelectedInventoryItem();

        if (invItem) 
        {
            ObjectPooler.Instance.SpawnItem(invItem.GetComponent<InventoryItem>().invItemInfo.itemInfo, grabPoint.transform.position, Quaternion.identity);
            ObjectPooler.Instance.DeleteInventoryItem(invItem.GetComponent<InventoryItem>().invItemInfo, invItem);
        }
    }

    private GameObject SelectedInventoryItem() 
    {
        List<RaycastResult> resultsList = new List<RaycastResult>();
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        raycaster.Raycast(pointerEventData, resultsList);
        
        foreach (RaycastResult result in resultsList)
        {
            if (result.gameObject.GetComponent<InventoryItem>())
                return result.gameObject;
        }

        return null;
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
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, distanceOfInteraction, interectionMask) && !isGrabbing)
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            //print(hit.collider.gameObject.name);

            if (hit.collider.gameObject.GetComponent<InteractableItem>())
            {
                lookObject = new LookObject(hit.collider.gameObject.GetComponent<InteractableItem>());
                playerUI.EnableInfoItemText(lookObject.name);
            }

            else if (hit.collider.gameObject.GetComponent<NonInteractableItem>()) 
            {
                lookObject = new LookObject(hit.collider.gameObject.GetComponent<NonInteractableItem>());
                if (playerUI.infoItemTextIsActive) playerUI.DisableInfoItemText();
            }

            else if (!isGrabbing)
            {
                lookObject = null;
                playerUI.DisableInfoItemText();
            }
        }

        else if (lookObject != null && !isGrabbing) 
        {
            lookObject = null;
            isGrabbing = false;
            playerUI.DisableInfoItemText();
        } 

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
        if (inventoryStatement) 
        {
            UIController.GetComponent<Layouts>().OpenLayout(LayoutType.Inventory);
            inputActions.Player.Disable();
            inputActions.UI.Enable();
        }

        else 
        {
            UIController.GetComponent<Layouts>().OpenLayout(LayoutType.PlayerPanel);
            inputActions.Player.Enable();
            inputActions.UI.Disable();
        }

        SetCursorActivity(inventoryStatement);
    }

    void FixedUpdate()
    {
        Move();
        Look();
        Rotation();
        ApplyGravity();
    }

    private class LookObject 
    {
        public GameObject gameObject;
        public ItemInfo itemInfo;
        public Rigidbody rigidBody;
        public string name;
        public bool isIntractable;
        
        public LookObject(InteractableItem item) 
        {
            gameObject = item.gameObject;
            itemInfo = item.itemInfo;
            rigidBody = item.itemRigidbody;
            name = item.itemInfo.itemName;
            isIntractable = true;
        }

        public LookObject(NonInteractableItem item)
        {
            gameObject = item.gameObject;
            rigidBody = item.itemRigidbody;
            isIntractable = false;
        }

        public void SetTarget(GameObject target) 
        {
            if (isIntractable) 
            {
                gameObject.GetComponent<InteractableItem>().targetObj = target; 
            }
            else 
            {
                gameObject.GetComponent<NonInteractableItem>().targetObj = target;
            }
        }

        public void DisableCollisionLayer(LayerMask layerMask) 
        {
            if (isIntractable)
            {
                gameObject.GetComponent<InteractableItem>().itemCollider.excludeLayers = layerMask;
            }
            else
            {
                gameObject.GetComponent<NonInteractableItem>().itemCollider.excludeLayers = layerMask;
            }
        }

        public void SpawnItem(Vector3 position, Quaternion rotation) 
        {
            if (gameObject.GetComponent<InteractableItem>()) 
            {
                ObjectPooler.Instance.SpawnItem(itemInfo, position, rotation);
            }
        }

        public void DespawnItem() 
        {
            if (gameObject.GetComponent<InteractableItem>())
            {
                ObjectPooler.Instance.DespawnItem(itemInfo, gameObject);
            }
        }
    }
}
