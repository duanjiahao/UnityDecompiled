using System;
using UnityEngine;
namespace UnityEditor
{
	internal class RotateTool : ManipulationTool
	{
		private static RotateTool s_Instance;
		public static void OnGUI(SceneView view)
		{
			if (RotateTool.s_Instance == null)
			{
				RotateTool.s_Instance = new RotateTool();
			}
			RotateTool.s_Instance.OnToolGUI(view);
		}
		public override void ToolGUI(SceneView view, Vector3 handlePosition, bool isStatic)
		{
			Quaternion handleRotation = Tools.handleRotation;
			EditorGUI.BeginChangeCheck();
			Quaternion quaternion = Handles.RotationHandle(handleRotation, handlePosition);
			if (EditorGUI.EndChangeCheck() && !isStatic)
			{
				float angle;
				Vector3 vector;
				(Quaternion.Inverse(handleRotation) * quaternion).ToAngleAxis(out angle, out vector);
				vector = handleRotation * vector;
				if (TransformManipulator.individualSpace)
				{
					vector = Quaternion.Inverse(Tools.handleRotation) * vector;
				}
				Undo.RecordObjects(Selection.transforms, "Rotate");
				Transform[] transforms = Selection.transforms;
				for (int i = 0; i < transforms.Length; i++)
				{
					Transform transform = transforms[i];
					Vector3 axis = vector;
					if (TransformManipulator.individualSpace)
					{
						axis = transform.rotation * vector;
					}
					if (Tools.pivotMode == PivotMode.Center)
					{
						transform.RotateAround(handlePosition, axis, angle);
					}
					else
					{
						transform.RotateAround(transform.position, axis, angle);
					}
					if (transform.parent != null)
					{
						transform.SendTransformChangedScale();
					}
				}
				Tools.handleRotation = quaternion;
			}
		}
	}
}
