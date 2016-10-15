using System;

namespace UnityEditor.Audio
{
	internal abstract class AudioParameterPath
	{
		public GUID parameter;

		public abstract string ResolveStringPath(bool getOnlyBasePath);
	}
}
