using System;
using UnityEngine.InputSystem;

public class SwitchableFunction
{
	private Action<InputAction.CallbackContext> _basicFunc;
	private Action<InputAction.CallbackContext> _switchFunc;
	private Action<InputAction.CallbackContext> _currentFunc;


	private FunctionSubscriptionType _basicFuncSubscriptionType;
	private FunctionSubscriptionType _switchFuncFuncSubscriptionType;

	public SwitchableFunction(Action<InputAction.CallbackContext> basicFunc, FunctionSubscriptionType basicFuncsubscriptionType, Action<InputAction.CallbackContext> switchFunc, FunctionSubscriptionType switchFuncFuncsubscriptionType)
	{
		_basicFunc = basicFunc;
		_switchFunc = switchFunc;
		_basicFuncSubscriptionType = basicFuncsubscriptionType;
		_switchFuncFuncSubscriptionType = switchFuncFuncsubscriptionType;
		_currentFunc = _basicFunc;
	}

	public void SwitchFunction()
	{
		_currentFunc = _currentFunc == _basicFunc ? _switchFunc : _basicFunc;
	}

	public Action<InputAction.CallbackContext> GetCurrentFunction()
	{
		return _currentFunc;
	}

	public FunctionSubscriptionType GetCurrentFunctionSubscriptionType()
	{
		return _currentFunc == _basicFunc ? _basicFuncSubscriptionType : _switchFuncFuncSubscriptionType;
	}

	public bool IsBasicFunction()
	{
		return _currentFunc == _basicFunc;
	}
}

public enum FunctionSubscriptionType
{
	FirstClick,
	AllClicks
}