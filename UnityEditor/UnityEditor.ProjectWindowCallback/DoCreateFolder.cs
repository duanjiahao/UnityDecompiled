using System;
using System.IO;
using UnityEngine;

namespace UnityEditor.ProjectWindowCallback
{
	internal class DoCreateFolder : EndNameEditAction
	{
		public override void Action(int instanceId, string pathName, string resourceFile)
		{
			string guid = AssetDatabase.CreateFolder(Path.GetDirectoryName(pathName), Path.GetFileName(pathName));
			UnityEngine.Object o = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(UnityEngine.Object));
			ProjectWindowUtil.ShowCreatedAsset(o);
		}
	}
}
