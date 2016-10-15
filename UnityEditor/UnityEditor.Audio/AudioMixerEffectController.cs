using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.Audio
{
	internal sealed class AudioMixerEffectController : UnityEngine.Object
	{
		private int m_LastCachedGroupDisplayNameID;

		private string m_DisplayName;

		public extern GUID effectID
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string effectName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern AudioMixerEffectController sendTarget
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enableWetMix
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool bypass
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public AudioMixerEffectController(string name)
		{
			AudioMixerEffectController.Internal_CreateAudioMixerEffectController(this, name);
		}

		[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void PreallocateGUIDs();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern GUID GetGUIDForMixLevel();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetValueForMixLevel(AudioMixerController controller, AudioMixerSnapshotController snapshot);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetValueForMixLevel(AudioMixerController controller, AudioMixerSnapshotController snapshot, float value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern GUID GetGUIDForParameter(string parameterName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetValueForParameter(AudioMixerController controller, AudioMixerSnapshotController snapshot, string parameterName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetValueForParameter(AudioMixerController controller, AudioMixerSnapshotController snapshot, string parameterName, float value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetFloatBuffer(AudioMixerController controller, string name, out float[] data, int numsamples);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetCPUUsage(AudioMixerController controller);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ContainsParameterGUID(GUID guid);
	}
}
