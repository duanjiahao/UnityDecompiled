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
				Vector3 point;
				(Quaternion.Inverse(handleRotation) * quaternion).ToAngleAxis(out angle, out point);
				Undo.RecordObjects(Selection.transforms, "Rotate");
				Transform[] transforms = Selection.transforms;
				for (int i = 0; i < transforms.Length; i++)
				{
					Transform transform = transforms[i];
					if (Tools.pivotMode == PivotMode.Center)
					{
						transform.RotateAround(handlePosition, handleRotation * point, angle);
					}
					else if (TransformManipulator.individualSpace)
					{
						transform.Rotate(transform.rotation * point, angle, Space.World);
					}
					else
					{
						transform.Rotate(handleRotation * point, angle, Space.World);
					}
					transform.SetLocalEulerHint(transform.GetLocalEulerAngles(transform.rotationOrder));
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
