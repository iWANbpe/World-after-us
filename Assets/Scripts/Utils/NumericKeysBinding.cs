using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class NumericKeysBinding
{
	private Dictionary<int, SwitchableFunction> keySwitchableFunctionsDictionary = new();

	public void AddKeyBinding(int keyNumber, Action<InputAction.CallbackContext> basicFunction, FunctionSubscriptionType basicFunctionSubscriptionType, Action<InputAction.CallbackContext> switchFunction, FunctionSubscriptionType switchFunctionSubscriptionType)
	{
		keySwitchableFunctionsDictionary.Add(keyNumber, new SwitchableFunction(basicFunction, basicFunctionSubscriptionType, switchFunction, switchFunctionSubscriptionType));
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
}