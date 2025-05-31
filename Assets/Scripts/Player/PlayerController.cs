using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
	[SerializeField] private int maxFWFValue;
	[SerializeField] private int minFWFValue;
	[SerializeField] private int maxStartFood;
	[SerializeField] private int minStartFood;
	[SerializeField] private int maxStartWater;
	[SerializeField] private int minStartWater;
	[SerializeField] private int maxStartFilter;
	[SerializeField] private int minStartFilter;

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
	private bool isInventoryOpen;
	private float lastClickTime;

	private RaycastHit hit;
	private GameObject lookObject;
	private bool isGrabbing;
	private GameObject grabPoint;
	public GameObject playerGrabPoint { get { return grabPoint; } }

	[HideInInspector] public EventSystem eventSystem;
	private GraphicRaycaster raycaster;
	private PointerEventData pointerEventData;
	private GameObject invItemLookObject;
	public GameObject invItemHoldObject;

	public static PlayerController Instance;
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance == this)
		{
			Destroy(gameObject);
		}

		canvas = GameObject.Find("Canvas");
		eventSystem = GameObject.Find("/EventSystem").GetComponent<EventSystem>();

		characterController = GetComponent<CharacterController>();
		raycaster = canvas.GetComponent<GraphicRaycaster>();

		inputActions = new InputActionsPlayer();
		pointerEventData = new PointerEventData(eventSystem);

		isInventoryOpen = false;
		isGrabbing = false;
		grabPoint = cam.transform.Find("GrabPoint").gameObject;

		fwfPlayer = new FWF(Random.Range(minStartFood, maxStartFood), Random.Range(minStartWater, maxStartWater), Random.Range(minStartFilter, maxStartFilter));
		ChangeMaxAndMinFWFValues(maxFWFValue, minFWFValue);

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

		inputActions.Player.Interact.started += Interact;

		//input actions UI
		inputActions.UI.InventoryClose.started += InventoryInteraction;

		inputActions.UI.DropItem.started += DropItem;

		inputActions.UI.Click.started += UseItem;

		inputActions.UI.Scroll.started += RotateInvItem;
	}

	private void Start()
	{
		SetCursorActivity(false);
		PlayerUI.Instance.DisableInfoItemText();
		PlayerUI.Instance.UpdateFWFBars(fwfPlayer);

		InventoryChangeStatement(isInventoryOpen);
		Layouts.Instance.OpenLayout(LayoutType.PlayerPanel);
	}

	private void SetCursorActivity(bool state)
	{
		Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
		Cursor.visible = state;
	}

	private void ChangeMaxAndMinFWFValues(int maxValue, int minValue)
	{
		foreach (UtilityPoint uPoint in fwfPlayer.utilityPoints)
		{
			uPoint.ChangeMaxValue(maxValue);
			uPoint.ChangeMinValue(minValue);
		}
	}

	private bool IsLookObjectItem()
	{
		if (lookObject.GetComponent<Item>()) return true;
		else return false;
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
		if (lookObject != null && IsLookObjectItem())
		{
			if (context.started)
			{
				isGrabbing = true;
				lookObject.GetComponent<Item>().itemRigidbody.useGravity = false;
				lookObject.GetComponent<Item>().DisableCollisionLayer(LayerMask.GetMask("Player"));
			}

			else if (context.performed)
			{
				lookObject.GetComponent<Item>().SetTarget(grabPoint);
			}

			else
			{
				isGrabbing = false;
				lookObject.GetComponent<Item>().SetTarget(null);
				lookObject.GetComponent<Item>().itemRigidbody.useGravity = true;

				lookObject.GetComponent<Item>().itemRigidbody.velocity = Vector3.zero;
				lookObject.GetComponent<Item>().itemRigidbody.angularVelocity = Vector3.zero;
				lookObject.GetComponent<Item>().DisableCollisionLayer(LayerMask.GetMask("Nothing"));
			}

		}
	}

	private void Throw(InputAction.CallbackContext contex)
	{
		if (isGrabbing)
		{
			isGrabbing = false;
			lookObject.GetComponent<Item>().itemRigidbody.useGravity = true;
			lookObject.GetComponent<Item>().SetTarget(null);
			lookObject.GetComponent<Item>().DisableCollisionLayer(LayerMask.GetMask("Nothing"));

			lookObject.GetComponent<Item>().itemRigidbody.AddForce((grabPoint.transform.position - cam.transform.position).normalized * 10f * throwingStrength);
		}
	}

	private void Interact(InputAction.CallbackContext contex)
	{
		if (lookObject != null && utils.GameObjectUsesInterface(lookObject, typeof(IInteract)) && (bool)utils.CallFunctionFromInterface(lookObject, typeof(IInteract), "CanInteract", null))
		{
			utils.CallFunctionFromInterface(lookObject, typeof(IInteract), "Interact", null);
		}
	}

	public void AddItemToInventory()
	{
		StartCoroutine(AddItemToInventoryCoroutine());
	}
	private IEnumerator AddItemToInventoryCoroutine()
	{
		GameObject invItem = ObjectPooler.Instance.InitializeInventoryItem(lookObject.GetComponent<Item>().itemInfo.GetInventoryItemInfo());

		yield return new WaitForEndOfFrame();

		if (InventoryControll.Instance.IsFreeSpaceForItem(invItem, out invItemPosition, out float rotation))
		{
			ObjectPooler.Instance.DespawnItem(lookObject.GetComponent<Item>().itemInfo, lookObject.gameObject);
			ObjectPooler.Instance.AddInventoryItem(invItem, invItemPosition, rotation);

			invItem.GetComponent<InventoryItem>().hasPlace = true;
		}
		else
		{
			PlayerUI.Instance.PlayerPanelMessage(Localization.Instance.GetText("UIStringTable", "notEnoughInventorySpace"));
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
				PlayerUI.Instance.EnableInfoPanel(result.gameObject.GetComponent<InventoryItem>().invItemInfo);
				return result.gameObject;
			}
		}

		PlayerUI.Instance.DisableInfoPanel();
		return null;
	}

	private void UseItem(InputAction.CallbackContext contex)
	{
		if (invItemLookObject)
		{
			if (Time.time - lastClickTime <= PlayerUI.Instance.doubleClickCoolDown)
			{
				fwfPlayer.Add(invItemLookObject.GetComponent<InventoryItem>().invItemInfo.itemInfo.GetItemFWF());
				PlayerUI.Instance.UpdateFWFBars(fwfPlayer);
				ObjectPooler.Instance.DeleteInventoryItem(invItemLookObject.GetComponent<InventoryItem>().invItemInfo, invItemLookObject);
			}
			else
			{
				lastClickTime = Time.time;
			}
		}
	}

	private void RotateInvItem(InputAction.CallbackContext context)
	{
		float mouseDelta = context.ReadValue<Vector2>().y;
		mouseDelta = Mathf.Clamp(mouseDelta, -1f, 1f);

		if (invItemHoldObject)
		{
			invItemHoldObject.GetComponent<InventoryItem>().Rotate(90f * mouseDelta);
		}
	}

	private void Move()
	{
		float targetSpeed;
		if (isCrouch) targetSpeed = speed * crouchSpeedKoef;
		else if (isSprinting) targetSpeed = speed * speedUpKoef;
		else targetSpeed = speed;

		currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedAcceleration * Time.fixedDeltaTime);
		characterController.Move(new Vector3(curPosition.x * currentSpeed, curPosition.y, curPosition.z * currentSpeed) * Time.fixedDeltaTime);
	}

	private void Rotation()
	{
		transform.rotation = Quaternion.Euler(0f, mouseRotation.x, 0f);
		cam.transform.localRotation = Quaternion.Euler(-Mathf.Clamp(mouseRotation.y, minRotationAngle, maxRotationAngle), 0f, 0f);
	}

	private void ApplyGravity()
	{
		if (characterController.isGrounded && grav_Velocity < 0f)
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

			if (hit.collider.gameObject)
			{
				lookObject = hit.collider.gameObject;

				if (lookObject.GetComponent<Item>() && utils.GameObjectUsesInterface(lookObject, typeof(IInteract)))
					PlayerUI.Instance.EnableInfoItemText(lookObject.GetComponent<Item>().itemInfo.GetLocalizedItemName());

				else if (PlayerUI.Instance.infoItemTextIsActive)
					PlayerUI.Instance.DisableInfoItemText();
			}

			else if (!isGrabbing)
			{
				lookObject = null;
				PlayerUI.Instance.DisableInfoItemText();
			}
		}

		else if (lookObject != null && !isGrabbing)
		{
			lookObject = null;
			isGrabbing = false;
			PlayerUI.Instance.DisableInfoItemText();
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

	public void InventoryChangeStatement(bool inventoryStatement)
	{
		isInventoryOpen = inventoryStatement;

		if (inventoryStatement)
		{
			Layouts.Instance.OpenLayout(LayoutType.Inventory);
			inputActions.Player.Disable();
			inputActions.UI.Enable();
			SetCursorActivity(inventoryStatement);
		}

		else
		{
			if (InventoryControll.Instance.AllItemsHavePlace())
			{
				Layouts.Instance.OpenLayout(LayoutType.PlayerPanel);
				inputActions.Player.Enable();
				inputActions.UI.Disable();
				SetCursorActivity(inventoryStatement);
				return;
			}

			else if (!InventoryControll.Instance.WarningPanelActivity())
			{
				InventoryControll.Instance.WarningPanelSetActivity(true);
			}

			isInventoryOpen = !isInventoryOpen;
		}
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