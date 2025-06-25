using System;

public class ActionParams
{
	private Action<object[]> _action;
	private object[] _args;
	public ActionParams(Action<object[]> action, params object[] args)
	{
		_action = action;
		_args = args;
	}

	public void Run()
	{
		_action(_args);
	}
}