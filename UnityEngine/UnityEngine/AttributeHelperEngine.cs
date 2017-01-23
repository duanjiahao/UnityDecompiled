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
			Type result;
			while (stack.Count > 0)
			{
				Type type2 = stack.Pop();
				object[] customAttributes = type2.GetCustomAttributes(typeof(DisallowMultipleComponent), false);
				int num = customAttributes.Length;
				if (num != 0)
				{
					result = type2;
					return result;
				}
			}
			result = null;
			return result;
		}

		[RequiredByNativeCode]
		private static Type[] GetRequiredComponents(Type klass)
		{
			List<Type> list = null;
			Type[] result;
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
						Type[] array3 = new Type[]
						{
							requireComponent.m_Type0,
							requireComponent.m_Type1,
							requireComponent.m_Type2
						};
						result = array3;
						return result;
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
				result = null;
				return result;
			}
			result = list.ToArray();
			return result;
		}

		[RequiredByNativeCode]
		private static bool CheckIsEditorScript(Type klass)
		{
			bool result;
			while (klass != null && klass != typeof(MonoBehaviour))
			{
				object[] customAttributes = klass.GetCustomAttributes(typeof(ExecuteInEditMode), false);
				int num = customAttributes.Length;
				if (num != 0)
				{
					result = true;
					return result;
				}
				klass = klass.BaseType;
			}
			result = false;
			return result;
		}

		[RequiredByNativeCode]
		private static int GetDefaultExecutionOrderFor(Type klass)
		{
			DefaultExecutionOrder customAttributeOfType = AttributeHelperEngine.GetCustomAttributeOfType<DefaultExecutionOrder>(klass);
			int result;
			if (customAttributeOfType == null)
			{
				result = 0;
			}
			else
			{
				result = customAttributeOfType.order;
			}
			return result;
		}

		private static T GetCustomAttributeOfType<T>(Type klass) where T : Attribute
		{
			Type typeFromHandle = typeof(T);
			object[] customAttributes = klass.GetCustomAttributes(typeFromHandle, true);
			T result;
			if (customAttributes != null && customAttributes.Length != 0)
			{
				result = (T)((object)customAttributes[0]);
			}
			else
			{
				result = (T)((object)null);
			}
			return result;
		}
	}
}
