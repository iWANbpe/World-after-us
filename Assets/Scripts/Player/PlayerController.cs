using System;
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

	[Header("Objects")]
	[SerializeField] private GameObject _cam;

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
	[HideInInspector] public bool shooting;

	private RaycastHit hit;
	private GameObject _lookObject;
	[HideInInspector] public bool isGrabbing;
	private GameObject grabPoint;

	[HideInInspector] public EventSystem eventSystem;
	private GraphicRaycaster raycaster;
	private PointerEventData pointerEventData;
	private GameObject invItemLookObject;
	[HideInInspector] public GameObject invItemHoldObject;

	[HideInInspector] public PlayerInteraction interaction;
	[HideInInspector] public PlayerUI playerUI;
	public static PlayerController Instance;

	#region Getters and Setters
	public GameObject playerGrabPoint { get { return grabPoint; } }
	public GameObject lookObject { get { return _lookObject; } }
	public GameObject cam { get { return _cam; } }

	public InputActionsPlayer actionsPlayer { get { return inputActions; } }
	#endregion

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
		shooting = false;

		isInventoryOpen = false;
		isGrabbing = false;
		grabPoint = _cam.transform.Find("GrabPoint").gameObject;

		fwfPlayer = new FWF(UnityEngine.Random.Range(minStartFood, maxStartFood), UnityEngine.Random.Range(minStartWater, maxStartWater), UnityEngine.Random.Range(minStartFilter, maxStartFilter));
		ChangeMaxAndMinFWFValues(maxFWFValue, minFWFValue);
	}

	private void Start()
	{
		interaction = PlayerInteraction.Instance;
		playerUI = PlayerUI.Instance;

		SetCursorActivity(false);
		playerUI.DisableInfoItemText();
		playerUI.UpdateFWFBars(fwfPlayer);
		playerUI.SetShootPointer(shooting);

		InventoryChangeStatement(isInventoryOpen);
		Layouts.Instance.OpenLayout(LayoutType.PlayerPanel);

		#region Input Actions Player

		inputActions.Player.Moving.started += HorizontalMoving;
		inputActions.Player.Moving.performed += HorizontalMoving;
		inputActions.Player.Moving.canceled += HorizontalMoving;

		inputActions.Player.MouseRotation.started += InputMouseRotation;
		inputActions.Player.MouseRotation.performed += InputMouseRotation;
		inputActions.Player.MouseRotation.canceled += InputMouseRotation;

		inputActions.Player.Jump.started += Jump;

		inputActions.Player.Sprint.started += Sprint;

		inputActions.Player.Crouch.started += Crouch;
		inputActions.Player.Crouch.performed += Crouch;
		inputActions.Player.Crouch.canceled += Crouch;

		inputActions.Player.InventoryOpen.started += InventoryInteraction;

		inputActions.Player.LeftClick.started += interaction.Grab;
		inputActions.Player.LeftClick.performed += interaction.Grab;
		inputActions.Player.LeftClick.canceled += interaction.Grab;

		inputActions.Player.Throw.started += interaction.Throw;
		inputActions.Player.Throw.performed += interaction.Throw;
		inputActions.Player.Throw.canceled += interaction.Throw;

		inputActions.Player.Interact.started += interaction.Interact;

		inputActions.Player.NumericKeys.started += interaction.NumericKeysPress;
		#endregion
		#region Input Actions UI
		inputActions.UI.InventoryClose.started += InventoryInteraction;

		inputActions.UI.DropItem.started += DropItem;

		inputActions.UI.Click.started += UseItem;

		inputActions.UI.Scroll.started += RotateInvItem;
		#endregion

		interaction.AddNumericKeyBinding(1, interaction.Shoot,
										FunctionSubscriptionType.FirstClick,
										new ActionParams(utils.Wrap<bool>(playerUI.SetShootPointer), true),
										new ActionParams(utils.Wrap<bool>(playerUI.SetShootPointer), false));
	}

	private void SetCursorActivity(bool state)
	{
		Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
		Cursor.visible = state;
	}

	public void SubscribeFunctionToInputAction(Action<InputAction.CallbackContext> function, FunctionSubscriptionType subscriptionType)
	{
		if (subscriptionType == FunctionSubscriptionType.FirstClick)
		{
			inputActions.Player.LeftClick.started += function;
		}

		else
		{
			inputActions.Player.LeftClick.started += function;
			inputActions.Player.LeftClick.performed += function;
			inputActions.Player.LeftClick.canceled += function;
		}
	}

	public void UnsubscribeFunctionFromInputAction(Action<InputAction.CallbackContext> function, FunctionSubscriptionType subscriptionType)
	{
		if (subscriptionType == FunctionSubscriptionType.FirstClick)
		{
			inputActions.Player.LeftClick.started -= function;
		}

		else
		{
			inputActions.Player.LeftClick.started -= function;
			inputActions.Player.LeftClick.performed -= function;
			inputActions.Player.LeftClick.canceled -= function;
		}
	}

	private void ChangeMaxAndMinFWFValues(int maxValue, int minValue)
	{
		foreach (UtilityPoint uPoint in fwfPlayer.utilityPoints)
		{
			uPoint.ChangeMaxValue(maxValue);
			uPoint.ChangeMinValue(minValue);
		}
	}

	public bool IsLookObjectItem()
	{
		if (_lookObject.GetComponent<Item>()) return true;
		else return false;
	}
	private void HorizontalMoving(InputAction.CallbackContext context)
	{
		moveInputHorizontal = context.ReadValue<Vector2>();
		curPosition.x = moveInputHorizontal.x;
		curPosition.z = moveInputHorizontal.y;
		isSprinting = context.canceled ? false : isSprinting;
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
		isSprinting = true;
	}

	private void Crouch(InputAction.CallbackContext context)
	{
		isCrouch = context.started || context.performed;
		Crouching();
	}


	public void AddItemToInventory()
	{
		StartCoroutine(AddItemToInventoryCoroutine());
	}
	private IEnumerator AddItemToInventoryCoroutine()
	{
		GameObject invItem = ObjectPooler.Instance.InitializeInventoryItem(_lookObject.GetComponent<Item>().itemInfo.GetInventoryItemInfo());

		yield return new WaitForEndOfFrame();

		if (InventoryControll.Instance.IsFreeSpaceForItem(invItem, out invItemPosition, out float rotation))
		{
			ObjectPooler.Instance.DespawnItem(_lookObject.GetComponent<Item>().itemInfo, _lookObject.gameObject);
			ObjectPooler.Instance.AddInventoryItem(invItem, invItemPosition, rotation);

			invItem.GetComponent<InventoryItem>().hasPlace = true;
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

	private void UseItem(InputAction.CallbackContext contex)
	{
		if (invItemLookObject)
		{
			if (Time.time - lastClickTime <= playerUI.doubleClickCoolDown)
			{
				fwfPlayer.Add(invItemLookObject.GetComponent<InventoryItem>().invItemInfo.itemInfo.GetItemFWF());
				playerUI.UpdateFWFBars(fwfPlayer);
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
		characterController.Move(transform.TransformDirection(new Vector3(curPosition.x * currentSpeed, curPosition.y, curPosition.z * currentSpeed)) * Time.fixedDeltaTime);
	}

	private void Rotation()
	{
		transform.rotation = Quaternion.Euler(0f, mouseRotation.x, 0f);
		_cam.transform.localRotation = Quaternion.Euler(-Mathf.Clamp(mouseRotation.y, minRotationAngle, maxRotationAngle), 0f, 0f);
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
		if (Physics.Raycast(_cam.transform.position, _cam.transform.TransformDirection(Vector3.forward), out hit, interaction.distance, interaction.mask) && !isGrabbing)
		{
			//Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
			//print(hit.collider.gameObject.name);

			if (hit.collider.gameObject)
			{
				_lookObject = hit.collider.gameObject;

				if (_lookObject.GetComponent<Item>() && utils.GameObjectUsesInterface(_lookObject, typeof(IInteract)))
					playerUI.EnableInfoItemText(_lookObject.GetComponent<Item>().itemInfo.GetLocalizedItemName());

				else if (playerUI.infoItemTextIsActive)
					playerUI.DisableInfoItemText();
			}

			else if (!isGrabbing)
			{
				_lookObject = null;
				playerUI.DisableInfoItemText();
			}
		}

		else if (_lookObject != null && !isGrabbing)
		{
			_lookObject = null;
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
			_cam.transform.localPosition = new Vector3(0f, camCrouchY, 0f);
		}

		else
		{
			characterController.height = characterNormalHeight;
			characterController.center = new Vector3(0f, characterNormalCenterY, 0f);
			_cam.transform.localPosition = new Vector3(0f, camNormalY, 0f);
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