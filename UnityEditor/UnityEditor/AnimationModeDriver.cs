using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal sealed class AnimationModeDriver : ScriptableObject
	{
		internal delegate bool IsKeyCallback(UnityEngine.Object target, string propertyPath);

		internal AnimationModeDriver.IsKeyCallback isKeyCallback;

		[UsedByNativeCode]
		internal bool InvokeIsKeyCallback_Internal(UnityEngine.Object target, string propertyPath)
		{
			return this.isKeyCallback != null && this.isKeyCallback(target, propertyPath);
		}
	}
}
