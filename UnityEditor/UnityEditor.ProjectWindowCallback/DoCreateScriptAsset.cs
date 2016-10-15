using System;
using UnityEngine;

namespace UnityEditor.ProjectWindowCallback
{
	internal class DoCreateScriptAsset : EndNameEditAction
	{
		public override void Action(int instanceId, string pathName, string resourceFile)
		{
			UnityEngine.Object o = ProjectWindowUtil.CreateScriptAssetFromTemplate(pathName, resourceFile);
			ProjectWindowUtil.ShowCreatedAsset(o);
		}
	}
}
