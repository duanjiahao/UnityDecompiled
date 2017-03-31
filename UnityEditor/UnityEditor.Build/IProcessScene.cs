using System;
using UnityEngine.SceneManagement;

namespace UnityEditor.Build
{
	public interface IProcessScene : IOrderedCallback
	{
		void OnProcessScene(Scene scene);
	}
}
