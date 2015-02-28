using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditor.Animations
{
	public sealed class AvatarMask : UnityEngine.Object
	{
		public extern int humanoidBodyPartCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern int transformCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		internal extern bool hasFeetIK
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AvatarMask();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetHumanoidBodyPartActive(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetHumanoidBodyPartActive(int index, bool value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetTransformPath(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTransformPath(int index, string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetTransformActive(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTransformActive(int index, bool value);
		internal void Copy(AvatarMask other)
		{
			for (int i = 0; i < this.humanoidBodyPartCount; i++)
			{
				this.SetHumanoidBodyPartActive(i, other.GetHumanoidBodyPartActive(i));
			}
			this.transformCount = other.transformCount;
			for (int j = 0; j < other.transformCount; j++)
			{
				this.SetTransformPath(j, other.GetTransformPath(j));
				this.SetTransformActive(j, other.GetTransformActive(j));
			}
		}
	}
}
