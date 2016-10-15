using System;

namespace UnityEditor.Audio
{
	internal struct MixerParameterDefinition
	{
		public string name;

		public string description;

		public string units;

		public float displayScale;

		public float displayExponent;

		public float minRange;

		public float maxRange;

		public float defaultValue;
	}
}
