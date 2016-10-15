using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct AnimatorClipInfo
	{
		private int m_ClipInstanceID;

		private float m_Weight;

		public AnimationClip clip
		{
			get
			{
				return (this.m_ClipInstanceID == 0) ? null : AnimatorClipInfo.ClipInstanceToScriptingObject(this.m_ClipInstanceID);
			}
		}

		public float weight
		{
			get
			{
				return this.m_Weight;
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationClip ClipInstanceToScriptingObject(int instanceID);
	}
}
