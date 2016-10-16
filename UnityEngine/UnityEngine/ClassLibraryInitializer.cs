using System;

namespace UnityEngine
{
	internal static class ClassLibraryInitializer
	{
		private static void Init()
		{
			UnityLogWriter.Init();
		}
	}
}
