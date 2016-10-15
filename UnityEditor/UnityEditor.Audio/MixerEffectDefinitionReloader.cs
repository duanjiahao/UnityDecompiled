using System;

namespace UnityEditor.Audio
{
	[InitializeOnLoad]
	internal static class MixerEffectDefinitionReloader
	{
		static MixerEffectDefinitionReloader()
		{
			MixerEffectDefinitions.Refresh();
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(MixerEffectDefinitionReloader.OnProjectChanged));
		}

		private static void OnProjectChanged()
		{
			MixerEffectDefinitions.Refresh();
		}
	}
}
