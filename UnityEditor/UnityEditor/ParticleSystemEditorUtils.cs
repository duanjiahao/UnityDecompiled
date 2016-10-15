using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;

namespace UnityEditor
{
	internal sealed class ParticleSystemEditorUtils
	{
		internal static extern float editorSimulationSpeed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern float editorPlaybackTime
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool editorIsScrubbing
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool editorIsPlaying
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool editorIsPaused
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool editorResimulation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool editorUpdateAll
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern ParticleSystem lockedParticleSystem
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string CheckCircularReferences(ParticleSystem subEmitter);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void PerformCompleteResimulation();

		public static ParticleSystem GetRoot(ParticleSystem ps)
		{
			if (ps == null)
			{
				return null;
			}
			Transform transform = ps.transform;
			while (transform.parent && transform.parent.gameObject.GetComponent<ParticleSystem>() != null)
			{
				transform = transform.parent;
			}
			return transform.gameObject.GetComponent<ParticleSystem>();
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StopEffect([DefaultValue("true")] bool stop, [DefaultValue("true")] bool clear);

		[ExcludeFromDocs]
		internal static void StopEffect(bool stop)
		{
			bool clear = true;
			ParticleSystemEditorUtils.StopEffect(stop, clear);
		}

		[ExcludeFromDocs]
		internal static void StopEffect()
		{
			bool clear = true;
			bool stop = true;
			ParticleSystemEditorUtils.StopEffect(stop, clear);
		}
	}
}
