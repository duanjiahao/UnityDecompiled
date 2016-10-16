using System;

namespace UnityEditor
{
	public abstract class IAudioEffectPlugin
	{
		public abstract bool SetFloatParameter(string name, float value);

		public abstract bool GetFloatParameter(string name, out float value);

		public abstract bool GetFloatParameterInfo(string name, out float minRange, out float maxRange, out float defaultValue);

		public abstract bool GetFloatBuffer(string name, out float[] data, int numsamples);

		public abstract int GetSampleRate();

		public abstract bool IsPluginEditableAndEnabled();
	}
}
