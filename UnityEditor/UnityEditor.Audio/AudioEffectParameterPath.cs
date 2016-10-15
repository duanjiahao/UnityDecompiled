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
			if (getOnlyBasePath)
			{
				return base.GetBasePath(this.group.GetDisplayString(), this.effect.effectName);
			}
			if (this.effect.GetGUIDForMixLevel() == this.parameter)
			{
				return "Mix Level" + base.GetBasePath(this.group.GetDisplayString(), this.effect.effectName);
			}
			MixerParameterDefinition[] effectParameters = MixerEffectDefinitions.GetEffectParameters(this.effect.effectName);
			for (int i = 0; i < effectParameters.Length; i++)
			{
				GUID gUIDForParameter = this.effect.GetGUIDForParameter(effectParameters[i].name);
				if (gUIDForParameter == this.parameter)
				{
					return effectParameters[i].name + base.GetBasePath(this.group.GetDisplayString(), this.effect.effectName);
				}
			}
			return "Error finding Parameter path.";
		}
	}
}
