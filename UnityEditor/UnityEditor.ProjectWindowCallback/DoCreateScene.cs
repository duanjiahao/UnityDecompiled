using System;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityEditor.ProjectWindowCallback
{
	internal class DoCreateScene : EndNameEditAction
	{
		public override void Action(int instanceId, string pathName, string resourceFile)
		{
			bool createDefaultGameObjects = true;
			if (EditorSceneManager.CreateSceneAsset(pathName, createDefaultGameObjects))
			{
				UnityEngine.Object o = AssetDatabase.LoadAssetAtPath(pathName, typeof(SceneAsset));
				ProjectWindowUtil.ShowCreatedAsset(o);
			}
		}
	}
}
