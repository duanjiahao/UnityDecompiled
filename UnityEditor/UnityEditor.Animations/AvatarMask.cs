using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.Animations
{
	public sealed class AvatarMask : UnityEngine.Object
	{
		[Obsolete("AvatarMask.humanoidBodyPartCount is deprecated. Use AvatarMaskBodyPart.LastBodyPart instead.")]
		private int humanoidBodyPartCount
		{
			get
			{
				return 13;
			}
		}

		public extern int transformCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal extern bool hasFeetIK
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AvatarMask();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetHumanoidBodyPartActive(AvatarMaskBodyPart index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetHumanoidBodyPartActive(AvatarMaskBodyPart index, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetTransformPath(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTransformPath(int index, string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetTransformActive(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTransformActive(int index, bool value);

		internal void Copy(AvatarMask other)
		{
			for (AvatarMaskBodyPart avatarMaskBodyPart = AvatarMaskBodyPart.Root; avatarMaskBodyPart < AvatarMaskBodyPart.LastBodyPart; avatarMaskBodyPart++)
			{
				this.SetHumanoidBodyPartActive(avatarMaskBodyPart, other.GetHumanoidBodyPartActive(avatarMaskBodyPart));
			}
			this.transformCount = other.transformCount;
			for (int i = 0; i < other.transformCount; i++)
			{
				this.SetTransformPath(i, other.GetTransformPath(i));
				this.SetTransformActive(i, other.GetTransformActive(i));
			}
		}
	}
}
