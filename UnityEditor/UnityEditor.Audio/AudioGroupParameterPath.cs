using System;

namespace UnityEditor.Audio
{
	internal class AudioGroupParameterPath : AudioParameterPath
	{
		public AudioMixerGroupController group;

		public AudioGroupParameterPath(AudioMixerGroupController group, GUID parameter)
		{
			this.group = group;
			this.parameter = parameter;
		}

		public override string ResolveStringPath(bool getOnlyBasePath)
		{
			if (getOnlyBasePath)
			{
				return this.GetBasePath(this.group.GetDisplayString(), null);
			}
			if (this.group.GetGUIDForVolume() == this.parameter)
			{
				return "Volume" + this.GetBasePath(this.group.GetDisplayString(), null);
			}
			if (this.group.GetGUIDForPitch() == this.parameter)
			{
				return "Pitch" + this.GetBasePath(this.group.GetDisplayString(), null);
			}
			return "Error finding Parameter path.";
		}

		protected string GetBasePath(string group, string effect)
		{
			string str = " (of " + group;
			if (!string.IsNullOrEmpty(effect))
			{
				str = str + "➔" + effect;
			}
			return str + ")";
		}
	}
}
