using System;
using System.Runtime.CompilerServices;
using UnityEngine.Audio;
using UnityEngine.Scripting;

namespace UnityEditor.Audio
{
	internal sealed class AudioMixerSnapshotController : AudioMixerSnapshot
	{
		public extern GUID snapshotID
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public AudioMixerSnapshotController(AudioMixer owner)
		{
			AudioMixerSnapshotController.Internal_CreateAudioMixerSnapshotController(this, owner);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateAudioMixerSnapshotController(AudioMixerSnapshotController mono, AudioMixer owner);

		public void SetValue(GUID guid, float value)
		{
			AudioMixerSnapshotController.INTERNAL_CALL_SetValue(this, guid, value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetValue(AudioMixerSnapshotController self, GUID guid, float value);

		public bool GetValue(GUID guid, out float value)
		{
			return AudioMixerSnapshotController.INTERNAL_CALL_GetValue(this, guid, out value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetValue(AudioMixerSnapshotController self, GUID guid, out float value);

		public void SetTransitionTypeOverride(GUID guid, ParameterTransitionType type)
		{
			AudioMixerSnapshotController.INTERNAL_CALL_SetTransitionTypeOverride(this, guid, type);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTransitionTypeOverride(AudioMixerSnapshotController self, GUID guid, ParameterTransitionType type);

		public bool GetTransitionTypeOverride(GUID guid, out ParameterTransitionType type)
		{
			return AudioMixerSnapshotController.INTERNAL_CALL_GetTransitionTypeOverride(this, guid, out type);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetTransitionTypeOverride(AudioMixerSnapshotController self, GUID guid, out ParameterTransitionType type);

		public void ClearTransitionTypeOverride(GUID guid)
		{
			AudioMixerSnapshotController.INTERNAL_CALL_ClearTransitionTypeOverride(this, guid);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ClearTransitionTypeOverride(AudioMixerSnapshotController self, GUID guid);
	}
}
