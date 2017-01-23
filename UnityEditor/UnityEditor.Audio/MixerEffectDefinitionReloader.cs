using System;
using System.Runtime.CompilerServices;

namespace UnityEditor.Audio
{
	[InitializeOnLoad]
	internal static class MixerEffectDefinitionReloader
	{
		[CompilerGenerated]
		private static EditorApplication.CallbackFunction <>f__mg$cache0;

		static MixerEffectDefinitionReloader()
		{
			MixerEffectDefinitions.Refresh();
			Delegate arg_28_0 = EditorApplication.projectWindowChanged;
			if (MixerEffectDefinitionReloader.<>f__mg$cache0 == null)
			{
				MixerEffectDefinitionReloader.<>f__mg$cache0 = new EditorApplication.CallbackFunction(MixerEffectDefinitionReloader.OnProjectChanged);
			}
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(arg_28_0, MixerEffectDefinitionReloader.<>f__mg$cache0);
		}

		private static void OnProjectChanged()
		{
			MixerEffectDefinitions.Refresh();
		}
	}
}
