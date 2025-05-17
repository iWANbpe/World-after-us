using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class utils
{
	private static Dictionary<string, object> interfacesDictionary = new Dictionary<string, object>();

	public static bool IsGameObjectUsesInterface(GameObject gameObject, Type interfaceType)
	{
		if (!interfaceType.IsInterface)
			return false;

		if (interfacesDictionary.ContainsKey(gameObject.name))
			return true;

		foreach (var mono in gameObject.GetComponents<MonoBehaviour>())
		{
			if (interfaceType.IsAssignableFrom(mono.GetType()))
			{
				interfacesDictionary.Add(gameObject.name, mono);
				return true;
			}

			var fields = mono.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (var field in fields)
			{
				var val = field.GetValue(mono);

				if (val != null && interfaceType.IsAssignableFrom(val.GetType()))
				{
					interfacesDictionary.Add(gameObject.name, val);
					return true;
				}
			}
		}

		return false;
	}

	public static object CallFunctionFromInterface(GameObject gameObject, string funcName, object[] funcParams)
	{
		interfacesDictionary.TryGetValue(gameObject.name, out object interact);

		if (interact != null)
		{
			MethodInfo method = interact.GetType().GetMethod(funcName);

			if (funcParams == null)
				return method.Invoke(interact, null);

			else
				return method.Invoke(interact, funcParams);
		}

		return null;
	}
}