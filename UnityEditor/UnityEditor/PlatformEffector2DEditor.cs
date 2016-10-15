using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(PlatformEffector2D), true)]
	internal class PlatformEffector2DEditor : Effector2DEditor
	{
		public void OnSceneGUI()
		{
			PlatformEffector2D platformEffector2D = (PlatformEffector2D)this.target;
			if (!platformEffector2D.enabled)
			{
				return;
			}
			if (platformEffector2D.useOneWay)
			{
				PlatformEffector2DEditor.DrawSurfaceArc(platformEffector2D);
			}
			if (!platformEffector2D.useSideBounce || !platformEffector2D.useSideFriction)
			{
				PlatformEffector2DEditor.DrawSideArc(platformEffector2D);
			}
		}

		private static void DrawSurfaceArc(PlatformEffector2D effector)
		{
			float num = 0.0174532924f * (effector.surfaceArc * 0.5f + effector.transform.eulerAngles.z);
			float num2 = Mathf.Clamp(effector.surfaceArc, 0.5f, 360f);
			float num3 = num2 * 0.0174532924f;
			foreach (Collider2D current in from collider in effector.gameObject.GetComponents<Collider2D>()
			where collider.enabled && collider.usedByEffector
			select collider)
			{
				Vector3 center = current.bounds.center;
				float handleSize = HandleUtility.GetHandleSize(center);
				Vector3 vector = new Vector3(-Mathf.Sin(num), Mathf.Cos(num), 0f);
				Vector3 a = new Vector3(-Mathf.Sin(num - num3), Mathf.Cos(num - num3), 0f);
				Handles.color = new Color(0f, 1f, 1f, 0.03f);
				Handles.DrawSolidArc(center, Vector3.back, vector, num2, handleSize);
				Handles.color = new Color(0f, 1f, 1f, 0.7f);
				Handles.DrawWireArc(center, Vector3.back, vector, num2, handleSize);
				Handles.DrawDottedLine(center, center + vector * handleSize, 5f);
				Handles.DrawDottedLine(center, center + a * handleSize, 5f);
			}
		}

		private static void DrawSideArc(PlatformEffector2D effector)
		{
			float num = 0.0174532924f * (effector.sideArc * 0.5f + effector.transform.eulerAngles.z);
			float num2 = Mathf.Clamp(effector.sideArc, 0.5f, 180f);
			float num3 = num2 * 0.0174532924f;
			foreach (Collider2D current in from collider in effector.gameObject.GetComponents<Collider2D>()
			where collider.enabled && collider.usedByEffector
			select collider)
			{
				Vector3 center = current.bounds.center;
				float num4 = HandleUtility.GetHandleSize(center) * 0.8f;
				Vector3 vector = new Vector3(-Mathf.Cos(num), -Mathf.Sin(num), 0f);
				Vector3 a = new Vector3(-Mathf.Cos(num - num3), -Mathf.Sin(num - num3), 0f);
				Vector3 vector2 = new Vector3(Mathf.Cos(num), Mathf.Sin(num), 0f);
				Vector3 a2 = new Vector3(Mathf.Cos(num - num3), Mathf.Sin(num - num3), 0f);
				Handles.color = new Color(0f, 1f, 0.7f, 0.03f);
				Handles.DrawSolidArc(center, Vector3.back, vector, num2, num4);
				Handles.DrawSolidArc(center, Vector3.back, vector2, num2, num4);
				Handles.color = new Color(0f, 1f, 0.7f, 0.7f);
				Handles.DrawWireArc(center, Vector3.back, vector, num2, num4);
				Handles.DrawWireArc(center, Vector3.back, vector2, num2, num4);
				Handles.DrawDottedLine(center + vector * num4, center + vector2 * num4, 5f);
				Handles.DrawDottedLine(center + a * num4, center + a2 * num4, 5f);
			}
		}
	}
}
