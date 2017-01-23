using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Audio;

namespace UnityEditor.Audio
{
	internal sealed class AudioMixerGroupController : AudioMixerGroup
	{
		public extern GUID groupID
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int userColorIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AudioMixerController controller
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern AudioMixerGroupController[] children
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AudioMixerEffectController[] effects
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool mute
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool solo
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool bypassEffects
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public AudioMixerGroupController(AudioMixer owner)
		{
			AudioMixerGroupController.Internal_CreateAudioMixerGroupController(this, owner);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateAudioMixerGroupController(AudioMixerGroupController mono, AudioMixer owner);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void PreallocateGUIDs();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern GUID GetGUIDForVolume();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetValueForVolume(AudioMixerController controller, AudioMixerSnapshotController snapshot);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetValueForVolume(AudioMixerController controller, AudioMixerSnapshotController snapshot, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern GUID GetGUIDForPitch();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetValueForPitch(AudioMixerController controller, AudioMixerSnapshotController snapshot);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetValueForPitch(AudioMixerController controller, AudioMixerSnapshotController snapshot, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasDependentMixers();

		public void InsertEffect(AudioMixerEffectController effect, int index)
		{
			List<AudioMixerEffectController> list = new List<AudioMixerEffectController>(this.effects);
			list.Add(null);
			for (int i = list.Count - 1; i > index; i--)
			{
				list[i] = list[i - 1];
			}
			list[index] = effect;
			this.effects = list.ToArray();
		}

		public bool HasAttenuation()
		{
			AudioMixerEffectController[] effects = this.effects;
			bool result;
			for (int i = 0; i < effects.Length; i++)
			{
				AudioMixerEffectController audioMixerEffectController = effects[i];
				if (audioMixerEffectController.IsAttenuation())
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public void DumpHierarchy(string title, int level)
		{
			if (title != "")
			{
				Console.WriteLine(title);
			}
			string str = "";
			int num = level;
			while (num-- > 0)
			{
				str += "  ";
			}
			Console.WriteLine(str + "name=" + base.name);
			str += "  ";
			AudioMixerEffectController[] effects = this.effects;
			for (int i = 0; i < effects.Length; i++)
			{
				AudioMixerEffectController audioMixerEffectController = effects[i];
				Console.WriteLine(str + "effect=" + audioMixerEffectController.ToString());
			}
			AudioMixerGroupController[] children = this.children;
			for (int j = 0; j < children.Length; j++)
			{
				AudioMixerGroupController audioMixerGroupController = children[j];
				audioMixerGroupController.DumpHierarchy("", level + 1);
			}
		}

		public string GetDisplayString()
		{
			return base.name;
		}

		public override string ToString()
		{
			return base.name;
		}
	}
}
