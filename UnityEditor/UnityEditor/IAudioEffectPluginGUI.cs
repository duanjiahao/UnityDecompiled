using System;

namespace UnityEditor
{
	public abstract class IAudioEffectPluginGUI
	{
		public abstract string Name
		{
			get;
		}

		public abstract string Description
		{
			get;
		}

		public abstract string Vendor
		{
			get;
		}

		public abstract bool OnGUI(IAudioEffectPlugin plugin);
	}
}
