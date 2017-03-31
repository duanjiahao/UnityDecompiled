using System;

namespace UnityEditor.Build
{
	public interface IPreprocessBuild : IOrderedCallback
	{
		void OnPreprocessBuild(BuildTarget target, string path);
	}
}
