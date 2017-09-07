using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal sealed class DrivenPropertyManager
	{
		[Conditional("UNITY_EDITOR")]
		public static void RegisterProperty(Object driver, Object target, string propertyPath)
		{
			if (driver == null)
			{
				throw new ArgumentNullException("driver");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (propertyPath == null)
			{
				throw new ArgumentNullException("propertyPath");
			}
			DrivenPropertyManager.RegisterPropertyInternal(driver, target, propertyPath);
		}

		[Conditional("UNITY_EDITOR")]
		public static void UnregisterProperty(Object driver, Object target, string propertyPath)
		{
			if (driver == null)
			{
				throw new ArgumentNullException("driver");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (propertyPath == null)
			{
				throw new ArgumentNullException("propertyPath");
			}
			DrivenPropertyManager.UnregisterPropertyInternal(driver, target, propertyPath);
		}

		[Conditional("UNITY_EDITOR")]
		public static void UnregisterProperties(Object driver)
		{
			if (driver == null)
			{
				throw new ArgumentNullException("driver");
			}
			DrivenPropertyManager.UnregisterPropertiesInternal(driver);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterPropertyInternal(Object driver, Object target, string propertyPath);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnregisterPropertyInternal(Object driver, Object target, string propertyPath);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnregisterPropertiesInternal(Object driver);
	}
}
