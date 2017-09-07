using System;

namespace UnityEditor.Build
{
	public interface IActiveBuildTargetChanged : IOrderedCallback
	{
		void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget);
	}
}
