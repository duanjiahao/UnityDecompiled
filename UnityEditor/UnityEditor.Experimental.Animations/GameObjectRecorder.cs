using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.Experimental.Animations
{
	public class GameObjectRecorder : UnityEngine.Object
	{
		public extern GameObject root
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float currentTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isRecording
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public GameObjectRecorder()
		{
			GameObjectRecorder.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] GameObjectRecorder notSelf);

		public void Bind(EditorCurveBinding binding)
		{
			this.Bind_Injected(ref binding);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BindAll(GameObject target, bool recursive);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BindComponent(GameObject target, Type componentType, bool recursive);

		public void BindComponent<T>(GameObject target, bool recursive) where T : Component
		{
			this.BindComponent(target, typeof(T), recursive);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern EditorCurveBinding[] GetBindings();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void TakeSnapshot(float dt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SaveToClip(AnimationClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetRecording();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Bind_Injected(ref EditorCurveBinding binding);
	}
}
