using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor.Audio
{
	internal sealed class AudioMixerEffectController : UnityEngine.Object
	{
		private int m_LastCachedGroupDisplayNameID;

		private string m_DisplayName;

		public extern GUID effectID
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string effectName
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern AudioMixerEffectController sendTarget
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enableWetMix
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool bypass
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public AudioMixerEffectController(string name)
		{
			AudioMixerEffectController.Internal_CreateAudioMixerEffectController(this, name);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateAudioMixerEffectController(AudioMixerEffectController mono, string name);

		public bool IsSend()
		{
			return this.effectName == "Send";
		}

		public bool IsReceive()
		{
			return this.effectName == "Receive";
		}

		public bool IsDuckVolume()
		{
			return this.effectName == "Duck Volume";
		}

		public bool IsAttenuation()
		{
			return this.effectName == "Attenuation";
		}

		public bool DisallowsBypass()
		{
			return this.IsSend() || this.IsReceive() || this.IsDuckVolume() || this.IsAttenuation();
		}

		public void ClearCachedDisplayName()
		{
			this.m_DisplayName = null;
		}

		public string GetDisplayString(Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap)
		{
			AudioMixerGroupController audioMixerGroupController = effectMap[this];
			if (audioMixerGroupController.GetInstanceID() != this.m_LastCachedGroupDisplayNameID || this.m_DisplayName == null)
			{
				this.m_DisplayName = audioMixerGroupController.GetDisplayString() + AudioMixerController.s_GroupEffectDisplaySeperator + AudioMixerController.FixNameForPopupMenu(this.effectName);
				this.m_LastCachedGroupDisplayNameID = audioMixerGroupController.GetInstanceID();
			}
			return this.m_DisplayName;
		}

		public string GetSendTargetDisplayString(Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap)
		{
			return (!(this.sendTarget != null)) ? string.Empty : this.sendTarget.GetDisplayString(effectMap);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void PreallocateGUIDs();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern GUID GetGUIDForMixLevel();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetValueForMixLevel(AudioMixerController controller, AudioMixerSnapshotController snapshot);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetValueForMixLevel(AudioMixerController controller, AudioMixerSnapshotController snapshot, float value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern GUID GetGUIDForParameter(string parameterName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetValueForParameter(AudioMixerController controller, AudioMixerSnapshotController snapshot, string parameterName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetValueForParameter(AudioMixerController controller, AudioMixerSnapshotController snapshot, string parameterName, float value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetFloatBuffer(AudioMixerController controller, string name, out float[] data, int numsamples);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetCPUUsage(AudioMixerController controller);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ContainsParameterGUID(GUID guid);
	}
}
