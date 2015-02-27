using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;
namespace UnityEditorInternal
{
	public sealed class State : UnityEngine.Object
	{
		public extern string uniqueName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern int uniqueNameHash
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern float speed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool mirror
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool iKOnFeet
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern string tag
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern StateMachine stateMachine
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public Vector3 position
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_position(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_position(ref value);
			}
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_position(out Vector3 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_position(ref Vector3 value);
		[ExcludeFromDocs]
		public Motion GetMotion()
		{
			AnimatorControllerLayer layer = null;
			return this.GetMotion(layer);
		}
		public Motion GetMotion([DefaultValue("null")] AnimatorControllerLayer layer)
		{
			return this.GetMotionInternal((layer == null) ? 0 : layer.motionSetIndex);
		}
		[ExcludeFromDocs]
		public void SetAnimationClip(AnimationClip clip)
		{
			AnimatorControllerLayer layer = null;
			this.SetAnimationClip(clip, layer);
		}
		public void SetAnimationClip(AnimationClip clip, [DefaultValue("null")] AnimatorControllerLayer layer)
		{
			this.SetMotionInternal(clip, (layer == null) ? 0 : layer.motionSetIndex);
		}
		[ExcludeFromDocs]
		public BlendTree CreateBlendTree()
		{
			AnimatorControllerLayer layer = null;
			return this.CreateBlendTree(layer);
		}
		public BlendTree CreateBlendTree([DefaultValue("null")] AnimatorControllerLayer layer)
		{
			return this.CreateBlendTreeInternal((layer == null) ? 0 : layer.motionSetIndex);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetMotionInternal(Motion motion, [DefaultValue("0")] int motionSetIndex);
		[ExcludeFromDocs]
		internal void SetMotionInternal(Motion motion)
		{
			int motionSetIndex = 0;
			this.SetMotionInternal(motion, motionSetIndex);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Motion GetMotionInternal([DefaultValue("0")] int motionSetIndex);
		[ExcludeFromDocs]
		internal Motion GetMotionInternal()
		{
			int motionSetIndex = 0;
			return this.GetMotionInternal(motionSetIndex);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern BlendTree CreateBlendTreeInternal([DefaultValue("0")] int motionSetIndex);
		[ExcludeFromDocs]
		internal BlendTree CreateBlendTreeInternal()
		{
			int motionSetIndex = 0;
			return this.CreateBlendTreeInternal(motionSetIndex);
		}
	}
}
