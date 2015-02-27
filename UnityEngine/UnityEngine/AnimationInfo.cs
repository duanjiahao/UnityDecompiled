using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public struct AnimationInfo
	{
		private int m_ClipInstanceID;
		private float m_Weight;
		public AnimationClip clip
		{
			get
			{
				return (this.m_ClipInstanceID == 0) ? null : AnimationInfo.ClipInstanceToScriptingObject(this.m_ClipInstanceID);
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
