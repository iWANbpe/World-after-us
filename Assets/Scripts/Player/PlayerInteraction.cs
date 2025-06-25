using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
	[Header("Interection")]
	[SerializeField] private float interactionDistance = 1f;
	[SerializeField] private LayerMask interectionMask;
	[Header("Shooting")]
	[SerializeField] private float distanceOfShoot = 1f;
	[Header("Throwing")]
	[SerializeField] private float throwingStrength = 1f;

	#region Getters and Setters
	public float distance { get { return interactionDistance; } }
	public LayerMask mask { get { return interectionMask; } }
	#endregion

	private NumericKeysBinding numericKeysBinding = new NumericKeysBinding();
	private PlayerController controller;
	public static PlayerInteraction Instance;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		controller = PlayerController.Instance;
	}

	public void Grab(InputAction.CallbackContext context)
	{
		if (controller.lookObject != null && controller.IsLookObjectItem())
		{
			if (context.started)
			{
				controller.isGrabbing = true;
				controller.lookObject.GetComponent<Item>().itemRigidbody.useGravity = false;
				controller.lookObject.GetComponent<Item>().DisableCollisionLayer(LayerMask.GetMask("Player"));
			}
			else if (context.performed)
			{
				controller.lookObject.GetComponent<Item>().SetTarget(controller.playerGrabPoint);
			}
			else
			{
				controller.isGrabbing = false;
				controller.lookObject.GetComponent<Item>().SetTarget(null);
				controller.lookObject.GetComponent<Item>().itemRigidbody.useGravity = true;

				controller.lookObject.GetComponent<Item>().itemRigidbody.velocity = Vector3.zero;
				controller.lookObject.GetComponent<Item>().itemRigidbody.angularVelocity = Vector3.zero;
				controller.lookObject.GetComponent<Item>().DisableCollisionLayer(LayerMask.GetMask("Nothing"));
			}

		}
	}

	public void Shoot(InputAction.CallbackContext context)
	{
		if (Physics.Raycast(controller.cam.transform.position, controller.cam.transform.TransformDirection(Vector3.forward), out RaycastHit shootHit, distanceOfShoot, interectionMask))
		{
			GameObject shootObject = shootHit.collider.gameObject;

			if (utils.GameObjectUsesInterface(shootObject, typeof(IOnShoot)))
				utils.CallFunctionFromInterface(shootObject, typeof(IOnShoot), "OnShoot", null);
		}
	}

	public void Throw(InputAction.CallbackContext contex)
	{
		if (controller.isGrabbing)
		{
			controller.isGrabbing = false;
			controller.lookObject.GetComponent<Item>().itemRigidbody.useGravity = true;
			controller.lookObject.GetComponent<Item>().SetTarget(null);
			controller.lookObject.GetComponent<Item>().DisableCollisionLayer(LayerMask.GetMask("Nothing"));

			controller.lookObject.GetComponent<Item>().itemRigidbody.AddForce((controller.playerGrabPoint.transform.position - controller.cam.transform.position).normalized * 10f * throwingStrength);
		}
	}

	public void Interact(InputAction.CallbackContext contex)
	{
		if (controller.lookObject != null && utils.GameObjectUsesInterface(controller.lookObject, typeof(IInteract)) && (bool)utils.CallFunctionFromInterface(controller.lookObject, typeof(IInteract), "CanInteract", null))
		{
			utils.CallFunctionFromInterface(controller.lookObject, typeof(IInteract), "Interact", null);
		}
	}

	#region Numeric Key Binding
	public void NumericKeysPress(InputAction.CallbackContext contex)
	{
		int keyNumber = int.Parse(contex.control.name);

		if (numericKeysBinding.ContainsKeyBinding(keyNumber))
		{
			if (!numericKeysBinding.IsBasicFunction(keyNumber))
				numericKeysBinding.RunDeactivationFunction(keyNumber);

			controller.UnsubscribeFunctionFromInputAction(numericKeysBinding.KeyGetCurrentFunction(keyNumber), numericKeysBinding.KeyFunctionSubscriptionType(keyNumber));
			numericKeysBinding.KeySwitchFunction(keyNumber);
			controller.SubscribeFunctionToInputAction(numericKeysBinding.KeyGetCurrentFunction(keyNumber), numericKeysBinding.KeyFunctionSubscriptionType(keyNumber));

			if (!numericKeysBinding.IsBasicFunction(keyNumber))
				numericKeysBinding.RunActivationFunction(keyNumber);
		}
	}

	public void AddNumericKeyBinding(int keyNumber, Action<InputAction.CallbackContext> switchFunction, FunctionSubscriptionType switchFunctionSubscriptionType)
	{
		numericKeysBinding.AddKeyBinding(keyNumber, Grab, FunctionSubscriptionType.AllClicks, switchFunction, switchFunctionSubscriptionType);
	}

	public void AddNumericKeyBinding(int keyNumber, Action<InputAction.CallbackContext> switchFunction, FunctionSubscriptionType switchFunctionSubscriptionType, ActionParams activationFunction)
	{
		numericKeysBinding.AddKeyBinding(keyNumber, Grab, FunctionSubscriptionType.AllClicks, switchFunction, switchFunctionSubscriptionType, activationFunction);
	}

	public void AddNumericKeyBinding(int keyNumber, Action<InputAction.CallbackContext> switchFunction, FunctionSubscriptionType switchFunctionSubscriptionType, ActionParams activationFunction, ActionParams deactivationFunction)
	{
		numericKeysBinding.AddKeyBinding(keyNumber, Grab, FunctionSubscriptionType.AllClicks, switchFunction, switchFunctionSubscriptionType, activationFunction, deactivationFunction);
	}

	public bool NumericKeyContainsBinding(int keyNumber)
	{
		return numericKeysBinding.ContainsKeyBinding(keyNumber);
	}

	public Action<InputAction.CallbackContext> NumericKeyGetCurrentFunction(int keyNumber)
	{
		return numericKeysBinding.KeyGetCurrentFunction(keyNumber);
	}

	public void NumericKeySwitchFunction(int keyNumber)
	{
		numericKeysBinding.KeySwitchFunction(keyNumber);
	}
	#endregion
}