using System;

namespace UnityEditor.Build
{
	public interface IPostprocessBuild : IOrderedCallback
	{
		void OnPostprocessBuild(BuildTarget target, string path);
	}
}
