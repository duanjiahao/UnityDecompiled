using System;
using UnityEngine;

namespace UnityEditor
{
	internal class MoveTool : ManipulationTool
	{
		private static MoveTool s_Instance;

		public static void OnGUI(SceneView view)
		{
			if (MoveTool.s_Instance == null)
			{
				MoveTool.s_Instance = new MoveTool();
			}
			MoveTool.s_Instance.OnToolGUI(view);
		}

		public override void ToolGUI(SceneView view, Vector3 handlePosition, bool isStatic)
		{
			TransformManipulator.BeginManipulationHandling(false);
			EditorGUI.BeginChangeCheck();
			Vector3 a = Handles.PositionHandle(handlePosition, Tools.handleRotation);
			if (EditorGUI.EndChangeCheck() && !isStatic)
			{
				Vector3 positionDelta = a - TransformManipulator.mouseDownHandlePosition;
				ManipulationToolUtility.SetMinDragDifferenceForPos(handlePosition);
				TransformManipulator.SetPositionDelta(positionDelta);
			}
			TransformManipulator.EndManipulationHandling();
		}
	}
}
