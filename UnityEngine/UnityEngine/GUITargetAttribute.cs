using System;
using System.Reflection;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Method)]
	public class GUITargetAttribute : Attribute
	{
		internal int displayMask;

		public GUITargetAttribute()
		{
			this.displayMask = -1;
		}

		public GUITargetAttribute(int displayIndex)
		{
			this.displayMask = 1 << displayIndex;
		}

		public GUITargetAttribute(int displayIndex, int displayIndex1)
		{
			this.displayMask = (1 << displayIndex | 1 << displayIndex1);
		}

		public GUITargetAttribute(int displayIndex, int displayIndex1, params int[] displayIndexList)
		{
			this.displayMask = (1 << displayIndex | 1 << displayIndex1);
			for (int i = 0; i < displayIndexList.Length; i++)
			{
				this.displayMask |= 1 << displayIndexList[i];
			}
		}

		[RequiredByNativeCode]
		private static int GetGUITargetAttrValue(Type klass, string methodName)
		{
			MethodInfo method = klass.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			int result;
			if (method != null)
			{
				object[] customAttributes = method.GetCustomAttributes(true);
				if (customAttributes != null)
				{
					for (int i = 0; i < customAttributes.Length; i++)
					{
						if (customAttributes[i].GetType() == typeof(GUITargetAttribute))
						{
							GUITargetAttribute gUITargetAttribute = customAttributes[i] as GUITargetAttribute;
							result = gUITargetAttribute.displayMask;
							return result;
						}
					}
				}
			}
			result = -1;
			return result;
		}
	}
}
