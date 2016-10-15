using System;
using UnityEngine;

namespace UnityEditor.ProjectWindowCallback
{
	internal class DoCreatePrefab : EndNameEditAction
	{
		public override void Action(int instanceId, string pathName, string resourceFile)
		{
			UnityEngine.Object o = PrefabUtility.CreateEmptyPrefab(pathName);
			ProjectWindowUtil.ShowCreatedAsset(o);
		}
	}
}
