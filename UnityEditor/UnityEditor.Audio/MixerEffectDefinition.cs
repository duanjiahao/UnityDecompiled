using System;

namespace UnityEditor.Audio
{
	internal class MixerEffectDefinition
	{
		private readonly string m_EffectName;

		private readonly MixerParameterDefinition[] m_Parameters;

		public string name
		{
			get
			{
				return this.m_EffectName;
			}
		}

		public MixerParameterDefinition[] parameters
		{
			get
			{
				return this.m_Parameters;
			}
		}

		public MixerEffectDefinition(string name, MixerParameterDefinition[] parameters)
		{
			this.m_EffectName = name;
			this.m_Parameters = new MixerParameterDefinition[parameters.Length];
			Array.Copy(parameters, this.m_Parameters, parameters.Length);
		}
	}
}
