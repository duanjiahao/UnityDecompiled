using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class AssemblyValidation
{
	private static Dictionary<RuntimePlatform, List<Type>> _rulesByPlatform;

	[CompilerGenerated]
	private static Func<Type, bool> <>f__mg$cache0;

	public static ValidationResult Validate(RuntimePlatform platform, IEnumerable<string> userAssemblies, params object[] options)
	{
		AssemblyValidation.WarmUpRulesCache();
		string[] array = (userAssemblies as string[]) ?? userAssemblies.ToArray<string>();
		ValidationResult result;
		if (array.Length != 0)
		{
			foreach (IValidationRule current in AssemblyValidation.ValidationRulesFor(platform, options))
			{
				ValidationResult validationResult = current.Validate(array, options);
				if (!validationResult.Success)
				{
					result = validationResult;
					return result;
				}
			}
		}
		result = new ValidationResult
		{
			Success = true
		};
		return result;
	}

	private static void WarmUpRulesCache()
	{
		if (AssemblyValidation._rulesByPlatform == null)
		{
			AssemblyValidation._rulesByPlatform = new Dictionary<RuntimePlatform, List<Type>>();
			Assembly assembly = typeof(AssemblyValidation).Assembly;
			IEnumerable<Type> arg_4E_0 = assembly.GetTypes();
			if (AssemblyValidation.<>f__mg$cache0 == null)
			{
				AssemblyValidation.<>f__mg$cache0 = new Func<Type, bool>(AssemblyValidation.IsValidationRule);
			}
			foreach (Type current in arg_4E_0.Where(AssemblyValidation.<>f__mg$cache0))
			{
				AssemblyValidation.RegisterValidationRule(current);
			}
		}
	}

	private static bool IsValidationRule(Type type)
	{
		return AssemblyValidation.ValidationRuleAttributesFor(type).Any<AssemblyValidationRule>();
	}

	private static IEnumerable<IValidationRule> ValidationRulesFor(RuntimePlatform platform, params object[] options)
	{
		return from t in AssemblyValidation.ValidationRuleTypesFor(platform)
		select AssemblyValidation.CreateValidationRuleWithOptions(t, options) into v
		where v != null
		select v;
	}

	[DebuggerHidden]
	private static IEnumerable<Type> ValidationRuleTypesFor(RuntimePlatform platform)
	{
		AssemblyValidation.<ValidationRuleTypesFor>c__Iterator0 <ValidationRuleTypesFor>c__Iterator = new AssemblyValidation.<ValidationRuleTypesFor>c__Iterator0();
		<ValidationRuleTypesFor>c__Iterator.platform = platform;
		AssemblyValidation.<ValidationRuleTypesFor>c__Iterator0 expr_0E = <ValidationRuleTypesFor>c__Iterator;
		expr_0E.$PC = -2;
		return expr_0E;
	}

	private static IValidationRule CreateValidationRuleWithOptions(Type type, params object[] options)
	{
		List<object> list = new List<object>(options);
		object[] array;
		ConstructorInfo constructorInfo;
		while (true)
		{
			array = list.ToArray();
			constructorInfo = AssemblyValidation.ConstructorFor(type, array);
			if (constructorInfo != null)
			{
				break;
			}
			if (list.Count == 0)
			{
				goto Block_2;
			}
			list.RemoveAt(list.Count - 1);
		}
		IValidationRule result = (IValidationRule)constructorInfo.Invoke(array);
		return result;
		Block_2:
		result = null;
		return result;
	}

	private static ConstructorInfo ConstructorFor(Type type, IEnumerable<object> options)
	{
		Type[] types = (from o in options
		select o.GetType()).ToArray<Type>();
		return type.GetConstructor(types);
	}

	internal static void RegisterValidationRule(Type type)
	{
		foreach (AssemblyValidationRule current in AssemblyValidation.ValidationRuleAttributesFor(type))
		{
			AssemblyValidation.RegisterValidationRuleForPlatform(current.Platform, type);
		}
	}

	internal static void RegisterValidationRuleForPlatform(RuntimePlatform platform, Type type)
	{
		if (!AssemblyValidation._rulesByPlatform.ContainsKey(platform))
		{
			AssemblyValidation._rulesByPlatform[platform] = new List<Type>();
		}
		if (AssemblyValidation._rulesByPlatform[platform].IndexOf(type) == -1)
		{
			AssemblyValidation._rulesByPlatform[platform].Add(type);
		}
		AssemblyValidation._rulesByPlatform[platform].Sort((Type a, Type b) => AssemblyValidation.CompareValidationRulesByPriority(a, b, platform));
	}

	internal static int CompareValidationRulesByPriority(Type a, Type b, RuntimePlatform platform)
	{
		int num = AssemblyValidation.PriorityFor(a, platform);
		int num2 = AssemblyValidation.PriorityFor(b, platform);
		int result;
		if (num == num2)
		{
			result = 0;
		}
		else
		{
			result = ((num >= num2) ? 1 : -1);
		}
		return result;
	}

	private static int PriorityFor(Type type, RuntimePlatform platform)
	{
		return (from attr in AssemblyValidation.ValidationRuleAttributesFor(type)
		where attr.Platform == platform
		select attr.Priority).FirstOrDefault<int>();
	}

	private static IEnumerable<AssemblyValidationRule> ValidationRuleAttributesFor(Type type)
	{
		return type.GetCustomAttributes(true).OfType<AssemblyValidationRule>();
	}
}
