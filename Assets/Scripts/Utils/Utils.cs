using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class utils
{
	private static Dictionary<string, List<object>> interfacesDictionary = new Dictionary<string, List<object>>();

	private static ConcurrentDictionary<string, object> globalCache = new ConcurrentDictionary<string, object>();
	private static ConcurrentDictionary<string, object> cashedFunctions = new ConcurrentDictionary<string, object>();

	private static void AddToInterfaceDictionary(string gameObjName, object interf)
	{
		if (interfacesDictionary.ContainsKey(gameObjName))
		{
			interfacesDictionary[gameObjName].Add(interf);
		}

		else
		{
			List<object> interfList = new();
			interfList.Add(interf);
			interfacesDictionary.Add(gameObjName, interfList);
		}
	}

	private static object GetInterfaceFromDictionary(string gameObjName, Type interfaceType)
	{
		interfacesDictionary.TryGetValue(gameObjName, out List<object> interfList);
		return interfList.Find(interf => interfaceType.IsAssignableFrom(interf.GetType()));
	}

	private static bool InterfaceDictionaryConatinsKey(string gameObjName, Type interfaceType)
	{
		interfacesDictionary.TryGetValue(gameObjName, out List<object> interfList);

		if (interfList != null)
		{
			foreach (object interf in interfList)
			{
				if (interfaceType.IsAssignableFrom(interf.GetType()))
					return true;
			}
		}

		return false;
	}
	public static bool GameObjectUsesInterface(GameObject gameObject, Type interfaceType)
	{
		Func<(GameObject, Type), bool> orgFunc = args => GameObjectUsesInterfaceBase(args.Item1, args.Item2);
		var func = orgFunc.Memoize();
		return func((gameObject, interfaceType));
	}
	private static bool GameObjectUsesInterfaceBase(GameObject gameObject, Type interfaceType)
	{
		if (!interfaceType.IsInterface)
			return false;

		if (InterfaceDictionaryConatinsKey(gameObject.name, interfaceType))
			return true;

		foreach (var mono in gameObject.GetComponents<MonoBehaviour>())
		{
			if (interfaceType.IsAssignableFrom(mono.GetType()))
			{
				AddToInterfaceDictionary(gameObject.name, mono);
				return true;
			}

			var fields = mono.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (var field in fields)
			{
				var val = field.GetValue(mono);

				if (val != null && interfaceType.IsAssignableFrom(val.GetType()))
				{
					AddToInterfaceDictionary(gameObject.name, val);
					return true;
				}
			}
		}

		return false;
	}

	public static object CallFunctionFromInterface(GameObject gameObject, Type interfaceType, string funcName, object[] funcParams)
	{
		object interact = GetInterfaceFromDictionary(gameObject.name, interfaceType);

		if (interact != null)
		{
			MethodInfo method = interact.GetType().GetMethod(funcName);
			return method.Invoke(interact, funcParams);
		}

		return null;
	}

	public static Func<Arg, Ret> Memoize<Arg, Ret>(this Func<Arg, Ret> functor)
	{
		string casheKey = functor.Method.ToString();

		if (cashedFunctions.TryGetValue(casheKey, out var ret))
			return (Func<Arg, Ret>)ret;

		Func<Arg, Ret> wrap = (arg0) =>
		{
			var memoTable = (ConcurrentDictionary<Arg, Ret>)globalCache.GetOrAdd(
				casheKey, _ => new ConcurrentDictionary<Arg, Ret>());

			if (memoTable.TryGetValue(arg0, out var cached))
			{
				return cached;
			}

			var result = functor(arg0);
			memoTable.TryAdd(arg0, result);
			return result;
		};

		cashedFunctions[casheKey] = wrap;
		return wrap;
	}

	public static void ClearFromCashe(string key)
	{
		globalCache.TryRemove(key, out _);
	}

	public static void ClearAllCashe()
	{
		globalCache.Clear();
	}

	#region Wrapper
	public static Action<object[]> Wrap<T1>(Action<T1> func)
	{
		
		return (args) => func((T1)args[0]);
	}
	public static Action<object[]> Wrap<T1, T2>(Action<T1, T2> func)
	{
		return (args) => func((T1)args[0], (T2)args[1]);
	}

	public static Action<object[]> Wrap<T1, T2, T3>(Action<T1, T2, T3> func)
	{
		return (args) => func((T1)args[0], (T2)args[1], (T3)args[2]);
	}
	#endregion
}