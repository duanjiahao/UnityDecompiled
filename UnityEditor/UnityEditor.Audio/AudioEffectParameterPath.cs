using System;

namespace UnityEditor.Audio
{
	internal sealed class AudioEffectParameterPath : AudioGroupParameterPath
	{
		public AudioMixerEffectController effect;

		public AudioEffectParameterPath(AudioMixerGroupController group, AudioMixerEffectController effect, GUID parameter) : base(group, parameter)
		{
			this.effect = effect;
		}

		public override string ResolveStringPath(bool getOnlyBasePath)
		{
			string result;
			if (getOnlyBasePath)
			{
				result = base.GetBasePath(this.group.GetDisplayString(), this.effect.effectName);
			}
			else if (this.effect.GetGUIDForMixLevel() == this.parameter)
			{
				result = "Mix Level" + base.GetBasePath(this.group.GetDisplayString(), this.effect.effectName);
			}
			else
			{
				MixerParameterDefinition[] effectParameters = MixerEffectDefinitions.GetEffectParameters(this.effect.effectName);
				for (int i = 0; i < effectParameters.Length; i++)
				{
					GUID gUIDForParameter = this.effect.GetGUIDForParameter(effectParameters[i].name);
					if (gUIDForParameter == this.parameter)
					{
						result = effectParameters[i].name + base.GetBasePath(this.group.GetDisplayString(), this.effect.effectName);
						return result;
					}
				}
				result = "Error finding Parameter path.";
			}
			return result;
		}
	}
}
