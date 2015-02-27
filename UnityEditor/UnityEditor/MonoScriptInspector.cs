using System;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(MonoScript))]
	internal class MonoScriptInspector : TextAssetInspector
	{
		protected override void OnHeaderGUI()
		{
		}
	}
}
