using System;
using UnityEngine;

namespace UnityEditor.Experimental.AssetImporters
{
	[CustomEditor(typeof(ScriptedImporter), true)]
	public class ScriptedImporterEditor : AssetImporterEditor
	{
		internal override string targetTitle
		{
			get
			{
				return base.targetTitle + " (" + ObjectNames.NicifyVariableName(base.GetType().Name) + ")";
			}
		}

		public override void OnInspectorGUI()
		{
			SerializedProperty iterator = base.serializedObject.GetIterator();
			bool enterChildren = true;
			while (iterator.NextVisible(enterChildren))
			{
				EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
				enterChildren = false;
			}
			base.ApplyRevertGUI();
		}

		protected override bool OnApplyRevertGUI()
		{
			bool flag = base.OnApplyRevertGUI();
			if (flag)
			{
				ActiveEditorTracker.sharedTracker.ForceRebuild();
			}
			return flag;
		}
	}
}
