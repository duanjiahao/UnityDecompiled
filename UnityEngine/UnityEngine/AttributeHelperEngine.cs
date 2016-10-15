using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal class AttributeHelperEngine
	{
		[RequiredByNativeCode]
		private static Type GetParentTypeDisallowingMultipleInclusion(Type type)
		{
			Stack<Type> stack = new Stack<Type>();
			while (type != null && type != typeof(MonoBehaviour))
			{
				stack.Push(type);
				type = type.BaseType;
			}
			while (stack.Count > 0)
			{
				Type type2 = stack.Pop();
				object[] customAttributes = type2.GetCustomAttributes(typeof(DisallowMultipleComponent), false);
				int num = customAttributes.Length;
				if (num != 0)
				{
					return type2;
				}
			}
			return null;
		}

		[RequiredByNativeCode]
		private static Type[] GetRequiredComponents(Type klass)
		{
			List<Type> list = null;
			while (klass != null && klass != typeof(MonoBehaviour))
			{
				RequireComponent[] array = (RequireComponent[])klass.GetCustomAttributes(typeof(RequireComponent), false);
				Type baseType = klass.BaseType;
				RequireComponent[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RequireComponent requireComponent = array2[i];
					if (list == null && array.Length == 1 && baseType == typeof(MonoBehaviour))
					{
						return new Type[]
						{
							requireComponent.m_Type0,
							requireComponent.m_Type1,
							requireComponent.m_Type2
						};
					}
					if (list == null)
					{
						list = new List<Type>();
					}
					if (requireComponent.m_Type0 != null)
					{
						list.Add(requireComponent.m_Type0);
					}
					if (requireComponent.m_Type1 != null)
					{
						list.Add(requireComponent.m_Type1);
					}
					if (requireComponent.m_Type2 != null)
					{
						list.Add(requireComponent.m_Type2);
					}
				}
				klass = baseType;
			}
			if (list == null)
			{
				return null;
			}
			return list.ToArray();
		}

		[RequiredByNativeCode]
		private static bool CheckIsEditorScript(Type klass)
		{
			while (klass != null && klass != typeof(MonoBehaviour))
			{
				object[] customAttributes = klass.GetCustomAttributes(typeof(ExecuteInEditMode), false);
				int num = customAttributes.Length;
				if (num != 0)
				{
					return true;
				}
				klass = klass.BaseType;
			}
			return false;
		}
	}
}
