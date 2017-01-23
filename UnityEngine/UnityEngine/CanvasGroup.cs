using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class CanvasGroup : Component, ICanvasRaycastFilter
	{
		public extern float alpha
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool interactable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool blocksRaycasts
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool ignoreParentGroups
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return this.blocksRaycasts;
		}
	}
}
