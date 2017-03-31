using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal sealed class ParticleSystemEditorUtils
	{
		internal static extern float editorSimulationSpeed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern float editorPlaybackTime
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool editorIsScrubbing
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool editorIsPlaying
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool editorIsPaused
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool editorResimulation
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern ParticleSystem lockedParticleSystem
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string CheckCircularReferences(ParticleSystem subEmitter);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void PerformCompleteResimulation();

		public static ParticleSystem GetRoot(ParticleSystem ps)
		{
			ParticleSystem result;
			if (ps == null)
			{
				result = null;
			}
			else
			{
				Transform transform = ps.transform;
				while (transform.parent && transform.parent.gameObject.GetComponent<ParticleSystem>() != null)
				{
					transform = transform.parent;
				}
				result = transform.gameObject.GetComponent<ParticleSystem>();
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
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
