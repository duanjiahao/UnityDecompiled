using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class HumanTrait
	{
		public static extern int MuscleCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string[] MuscleName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int BoneCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string[] BoneName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int RequiredBoneCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int MuscleFromBone(int i, int dofIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int BoneFromMuscle(int i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RequiredBone(int i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasCollider(Avatar avatar, int i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetMuscleDefaultMin(int i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetMuscleDefaultMax(int i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetParentBone(int i);
	}
}
