using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class NumericKeysBinding
{
	private Dictionary<int, SwitchableFunction> keySwitchableFunctionsDictionary = new();
	private Dictionary<int, ActionParams> keyActivationFunctionsDictionary = new();
	private Dictionary<int, ActionParams> keyDeactivationFunctionsDictionary = new();

	public void AddKeyBinding(int keyNumber, Action<InputAction.CallbackContext> basicFunction, FunctionSubscriptionType basicFunctionSubscriptionType, Action<InputAction.CallbackContext> switchFunction, FunctionSubscriptionType switchFunctionSubscriptionType)
	{
		keySwitchableFunctionsDictionary.Add(keyNumber, new SwitchableFunction(basicFunction, basicFunctionSubscriptionType, switchFunction, switchFunctionSubscriptionType));
	}
	public void AddKeyBinding(int keyNumber, Action<InputAction.CallbackContext> basicFunction, FunctionSubscriptionType basicFunctionSubscriptionType, Action<InputAction.CallbackContext> switchFunction, FunctionSubscriptionType switchFunctionSubscriptionType, ActionParams activationFunc)
	{
		keySwitchableFunctionsDictionary.Add(keyNumber, new SwitchableFunction(basicFunction, basicFunctionSubscriptionType, switchFunction, switchFunctionSubscriptionType));
		keyActivationFunctionsDictionary.Add(keyNumber, activationFunc);
	}
	public void AddKeyBinding(int keyNumber, Action<InputAction.CallbackContext> basicFunction, FunctionSubscriptionType basicFunctionSubscriptionType, Action<InputAction.CallbackContext> switchFunction, FunctionSubscriptionType switchFunctionSubscriptionType, ActionParams activationFunc, ActionParams deactivationFunc)
	{
		keySwitchableFunctionsDictionary.Add(keyNumber, new SwitchableFunction(basicFunction, basicFunctionSubscriptionType, switchFunction, switchFunctionSubscriptionType));
		keyActivationFunctionsDictionary.Add(keyNumber, activationFunc);
		keyDeactivationFunctionsDictionary.Add(keyNumber, deactivationFunc);
	}

	public void KeySwitchFunction(int keyNumber)
	{
		if (keySwitchableFunctionsDictionary.ContainsKey(keyNumber))
			keySwitchableFunctionsDictionary[keyNumber].SwitchFunction();
	}

	public Action<InputAction.CallbackContext> KeyGetCurrentFunction(int keyNumber)
	{
		if (keySwitchableFunctionsDictionary.ContainsKey(keyNumber))
			return keySwitchableFunctionsDictionary[keyNumber].GetCurrentFunction();
		return null;
	}

	public void KeyClearBinding(int keyNumber)
	{
		if (keySwitchableFunctionsDictionary.ContainsKey(keyNumber))
			keySwitchableFunctionsDictionary.Remove(keyNumber);
	}

	public FunctionSubscriptionType KeyFunctionSubscriptionType(int keyNumber)
	{
		if (keySwitchableFunctionsDictionary.ContainsKey(keyNumber))
			return keySwitchableFunctionsDictionary[keyNumber].GetCurrentFunctionSubscriptionType();
		return FunctionSubscriptionType.FirstClick;
	}

	public bool ContainsKeyBinding(int keyNumber)
	{
		if (keySwitchableFunctionsDictionary.ContainsKey(keyNumber))
			return true;
		return false;
	}

	public void RunActivationFunction(int keyNumber)
	{
		keyActivationFunctionsDictionary[keyNumber]?.Run();
	}

	public void RunDeactivationFunction(int keyNumber)
	{
		keyDeactivationFunctionsDictionary[keyNumber]?.Run();
	}

	public bool IsBasicFunction(int keyNumber)
	{
		return keySwitchableFunctionsDictionary[keyNumber].IsBasicFunction();
	}
}