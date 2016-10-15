using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ScaleTool : ManipulationTool
	{
		private static ScaleTool s_Instance;

		private static Vector3 s_CurrentScale = Vector3.one;

		public static void OnGUI(SceneView view)
		{
			if (ScaleTool.s_Instance == null)
			{
				ScaleTool.s_Instance = new ScaleTool();
			}
			ScaleTool.s_Instance.OnToolGUI(view);
		}

		public override void ToolGUI(SceneView view, Vector3 handlePosition, bool isStatic)
		{
			Quaternion quaternion = (Selection.transforms.Length <= 1) ? Tools.handleLocalRotation : Tools.handleRotation;
			TransformManipulator.DebugAlignment(quaternion);
			if (Event.current.type == EventType.MouseDown)
			{
				ScaleTool.s_CurrentScale = Vector3.one;
			}
			EditorGUI.BeginChangeCheck();
			TransformManipulator.BeginManipulationHandling(true);
			ScaleTool.s_CurrentScale = Handles.ScaleHandle(ScaleTool.s_CurrentScale, handlePosition, quaternion, HandleUtility.GetHandleSize(handlePosition));
			TransformManipulator.EndManipulationHandling();
			if (EditorGUI.EndChangeCheck() && !isStatic)
			{
				TransformManipulator.SetScaleDelta(ScaleTool.s_CurrentScale, quaternion);
			}
		}
	}
}
