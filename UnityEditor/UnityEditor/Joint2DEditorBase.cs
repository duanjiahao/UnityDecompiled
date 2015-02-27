using System;
using UnityEngine;
namespace UnityEditor
{
	internal class Joint2DEditorBase : Editor
	{
		public class Styles
		{
			public readonly GUIStyle anchor = "U2D.pivotDot";
			public readonly GUIStyle anchorActive = "U2D.pivotDotActive";
			public readonly GUIStyle connectedAnchor = "U2D.dragDot";
			public readonly GUIStyle connectedAnchorActive = "U2D.dragDotActive";
		}
		protected static Joint2DEditorBase.Styles s_Styles;
		protected bool HandleAnchor(ref Vector3 position, bool isConnectedAnchor)
		{
			if (Joint2DEditorBase.s_Styles == null)
			{
				Joint2DEditorBase.s_Styles = new Joint2DEditorBase.Styles();
			}
			Handles.DrawCapFunction drawFunc = (!isConnectedAnchor) ? new Handles.DrawCapFunction(Joint2DEditorBase.AnchorCap) : new Handles.DrawCapFunction(Joint2DEditorBase.ConnectedAnchorCap);
			int id = this.target.GetInstanceID() + ((!isConnectedAnchor) ? 0 : 1);
			EditorGUI.BeginChangeCheck();
			position = Handles.Slider2D(id, position, Vector3.back, Vector3.right, Vector3.up, 0f, drawFunc, Vector2.zero);
			return EditorGUI.EndChangeCheck();
		}
		public static void AnchorCap(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			if (controlID == GUIUtility.keyboardControl)
			{
				Joint2DEditorBase.DrawCap(controlID, position, Joint2DEditorBase.s_Styles.anchorActive);
			}
			else
			{
				Joint2DEditorBase.DrawCap(controlID, position, Joint2DEditorBase.s_Styles.anchor);
			}
		}
		public static void ConnectedAnchorCap(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			if (controlID == GUIUtility.keyboardControl)
			{
				Joint2DEditorBase.DrawCap(controlID, position, Joint2DEditorBase.s_Styles.connectedAnchorActive);
			}
			else
			{
				Joint2DEditorBase.DrawCap(controlID, position, Joint2DEditorBase.s_Styles.connectedAnchor);
			}
		}
		private static void DrawCap(int controlID, Vector3 position, GUIStyle guiStyle)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			Handles.BeginGUI();
			position = HandleUtility.WorldToGUIPoint(position);
			float fixedWidth = guiStyle.fixedWidth;
			float fixedHeight = guiStyle.fixedHeight;
			Rect position2 = new Rect(position.x - fixedWidth / 2f, position.y - fixedHeight / 2f, fixedWidth, fixedHeight);
			guiStyle.Draw(position2, GUIContent.none, controlID);
			Handles.EndGUI();
		}
		public static void DrawAALine(Vector3 start, Vector3 end)
		{
			Handles.DrawAAPolyLine(new Vector3[]
			{
				start,
				end
			});
		}
		public static void DrawDistanceGizmo(Vector3 anchor, Vector3 connectedAnchor, float distance)
		{
			Vector3 normalized = (anchor - connectedAnchor).normalized;
			Vector3 vector = connectedAnchor + normalized * distance;
			Vector3 vector2 = Vector3.Cross(normalized, Vector3.forward);
			vector2 *= HandleUtility.GetHandleSize(connectedAnchor) * 0.16f;
			Handles.color = Color.green;
			Joint2DEditorBase.DrawAALine(anchor, vector);
			Joint2DEditorBase.DrawAALine(connectedAnchor + vector2, connectedAnchor - vector2);
			Joint2DEditorBase.DrawAALine(vector + vector2, vector - vector2);
		}
		private static Matrix4x4 GetAnchorSpaceMatrix(Transform transform)
		{
			return Matrix4x4.TRS(transform.position, Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z), transform.lossyScale);
		}
		protected static Vector3 TransformPoint(Transform transform, Vector3 position)
		{
			return Joint2DEditorBase.GetAnchorSpaceMatrix(transform).MultiplyPoint(position);
		}
		protected static Vector3 InverseTransformPoint(Transform transform, Vector3 position)
		{
			return Joint2DEditorBase.GetAnchorSpaceMatrix(transform).inverse.MultiplyPoint(position);
		}
		protected static Vector3 SnapToSprite(SpriteRenderer spriteRenderer, Vector3 position, float snapDistance)
		{
			if (spriteRenderer == null)
			{
				return position;
			}
			snapDistance = HandleUtility.GetHandleSize(position) * snapDistance;
			float num = spriteRenderer.sprite.bounds.size.x / 2f;
			float num2 = spriteRenderer.sprite.bounds.size.y / 2f;
			Vector2[] array = new Vector2[]
			{
				new Vector2(-num, -num2),
				new Vector2(0f, -num2),
				new Vector2(num, -num2),
				new Vector2(-num, 0f),
				new Vector2(0f, 0f),
				new Vector2(num, 0f),
				new Vector2(-num, num2),
				new Vector2(0f, num2),
				new Vector2(num, num2)
			};
			Vector2[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Vector2 v = array2[i];
				Vector3 vector = spriteRenderer.transform.TransformPoint(v);
				if (Vector2.Distance(position, vector) <= snapDistance)
				{
					return vector;
				}
			}
			return position;
		}
		protected static Vector3 SnapToPoint(Vector3 position, Vector3 snapPosition, float snapDistance)
		{
			snapDistance = HandleUtility.GetHandleSize(position) * snapDistance;
			return (Vector3.Distance(position, snapPosition) > snapDistance) ? position : snapPosition;
		}
		protected static Vector2 RotateVector2(Vector2 direction, float angle)
		{
			float f = 0.0174532924f * -angle;
			float num = Mathf.Cos(f);
			float num2 = Mathf.Sin(f);
			float x = direction.x * num - direction.y * num2;
			float y = direction.x * num2 + direction.y * num;
			return new Vector2(x, y);
		}
	}
}
