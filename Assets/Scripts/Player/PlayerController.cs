using UnityEngine;
using System.Collections;
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
    [SerializeField] private float throwingStrength = 1f;
    [SerializeField] private LayerMask interectionMask;
    [Header("Objects")]
    [SerializeField] private GameObject cam;
    [Header("FWF")]
    [SerializeField] private int maxStartFood;
    [SerializeField] private int minStartFood;
    [SerializeField] private int maxStartWater;
    [SerializeField] private int minStartWater;
    [SerializeField] private int maxStartFilter;
    [SerializeField] private int minStartFilter;
    [SerializeField] private int maxFWFvalue;

    private InputActionsPlayer inputActions;
    private CharacterController characterController;

    private FWF fwfPlayer;

    private Vector3 curPosition;
    private Vector2 moveInputHorizontal;
    private Vector2 mouseRotation;
    private Vector2 invItemPosition;

    private float currentSpeed;
    private const float gravity = -9.81f;
    private float grav_Velocity;
    private bool isSprinting;
    private bool isCrouch;

    private GameObject canvas;
    private PlayerUI playerUI;
    private bool isInventoryOpen;

    private RaycastHit hit;
    private Item lookObject;
    private bool isGrabbing;
    private GameObject grabPoint;

    [HideInInspector] public EventSystem eventSystem;
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private GameObject invItemLookObject;

    void Awake()
    {
        canvas = GameObject.Find("Canvas");
        eventSystem = GameObject.Find("/EventSystem").GetComponent<EventSystem>();

        characterController = GetComponent<CharacterController>();
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        playerUI = GetComponent<PlayerUI>();

        inputActions = new InputActionsPlayer();
        pointerEventData = new PointerEventData(eventSystem);

        isInventoryOpen = false;
        isGrabbing = false;
        grabPoint = cam.transform.Find("GrabPoint").gameObject;

        fwfPlayer = new FWF(Random.Range(minStartFood, maxStartFood), Random.Range(minStartWater, maxStartWater), Random.Range(minStartFilter, maxStartFilter), maxFWFvalue);
        
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

        inputActions.Player.Throw.started += Throw;
        inputActions.Player.Throw.performed += Throw;
        inputActions.Player.Throw.canceled += Throw;

        inputActions.Player.Take.started += TakeItem;

        //input actions UI
        inputActions.UI.InventoryClose.started += InventoryInteraction;

        inputActions.UI.DropItem.started += DropItem;
    }

    private void Start()
    {
        SetCursorActivity(false);
        playerUI.DisableInfoItemText();
        playerUI.UpdateFWFBars(fwfPlayer);

        InventoryChangeStatement(isInventoryOpen);
        Layouts.Instance.OpenLayout(LayoutType.PlayerPanel);
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
                lookObject.itemRigidbody.useGravity = false;
                lookObject.DisableCollisionLayer(LayerMask.GetMask("Player"));
            }

            else if (context.performed) 
            {
                lookObject.SetTarget(grabPoint);
            }

            else
            {
                isGrabbing = false;
                lookObject.SetTarget(null);
                lookObject.itemRigidbody.useGravity = true;

                lookObject.itemRigidbody.velocity = Vector3.zero;
                lookObject.itemRigidbody.angularVelocity = Vector3.zero;
                lookObject.DisableCollisionLayer(LayerMask.GetMask("Nothing"));
            }
           
        }
    }

    private void Throw(InputAction.CallbackContext contex) 
    {
        if (isGrabbing) 
        {
            isGrabbing = false;
            lookObject.itemRigidbody.useGravity = true;
            lookObject.SetTarget(null);
            lookObject.DisableCollisionLayer(LayerMask.GetMask("Nothing"));

            lookObject.itemRigidbody.AddForce( (grabPoint.transform.position - cam.transform.position).normalized * 10f * throwingStrength);
        }
    }

    private void TakeItem(InputAction.CallbackContext contex) 
    { 
        if(lookObject != null && lookObject.isIntractable) 
        {
            StartCoroutine(AddItemToInventory());
        }
    }
    private IEnumerator AddItemToInventory()
    {
        GameObject invItem = ObjectPooler.Instance.InitializeInventoryItem(lookObject.itemInfo.GetInventoryItemInfo());

        yield return new WaitForEndOfFrame();
        
        if(InventoryControll.Instance.IsFreeSpaceForItem(invItem, out invItemPosition)) 
        {
            ObjectPooler.Instance.DespawnItem(lookObject.itemInfo, lookObject.gameObject);
            ObjectPooler.Instance.AddInventoryItem(invItem, invItemPosition);
        }
        else 
        {
            playerUI.PlayerPanelMessage(Localization.Instance.GetText("UIStringTable", "notEnoughInventorySpace"));
        }

    }
    private void DropItem(InputAction.CallbackContext contex) 
    {
        if (invItemLookObject) 
        {
            ObjectPooler.Instance.SpawnItem(invItemLookObject.GetComponent<InventoryItem>().invItemInfo.itemInfo, grabPoint.transform.position, Quaternion.identity);
            ObjectPooler.Instance.DeleteInventoryItem(invItemLookObject.GetComponent<InventoryItem>().invItemInfo, invItemLookObject);
        }
    }

    private GameObject SelectedInventoryItem() 
    {
        List<RaycastResult> resultsList = new List<RaycastResult>();
        pointerEventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        raycaster.Raycast(pointerEventData, resultsList);
        
        foreach (RaycastResult result in resultsList)
        {
            if (result.gameObject.GetComponent<InventoryItem>()) 
            {
                playerUI.EnableInfoPanel(result.gameObject.GetComponent<InventoryItem>().invItemInfo);
                return result.gameObject;
            }  
        }

        playerUI.DisableInfoPanel();
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

            if (hit.collider.gameObject.GetComponent<Item>())
            {
                lookObject = hit.collider.gameObject.GetComponent<Item>();

                if (lookObject.isIntractable) 
                    playerUI.EnableInfoItemText(lookObject.itemInfo.GetLocalizedItemName());
                
                else if (playerUI.isActiveAndEnabled) 
                    playerUI.DisableInfoItemText();
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
            Layouts.Instance.OpenLayout(LayoutType.Inventory);
            inputActions.Player.Disable();
            inputActions.UI.Enable();
        }

        else 
        {
            Layouts.Instance.OpenLayout(LayoutType.PlayerPanel);
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

        if (isInventoryOpen) invItemLookObject = SelectedInventoryItem();
    }
}

