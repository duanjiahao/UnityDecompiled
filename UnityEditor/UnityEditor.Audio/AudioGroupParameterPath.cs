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
			string result;
			if (getOnlyBasePath)
			{
				result = this.GetBasePath(this.group.GetDisplayString(), null);
			}
			else if (this.group.GetGUIDForVolume() == this.parameter)
			{
				result = "Volume" + this.GetBasePath(this.group.GetDisplayString(), null);
			}
			else if (this.group.GetGUIDForPitch() == this.parameter)
			{
				result = "Pitch" + this.GetBasePath(this.group.GetDisplayString(), null);
			}
			else
			{
				result = "Error finding Parameter path.";
			}
			return result;
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
