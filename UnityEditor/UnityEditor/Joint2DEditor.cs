using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Joint2D))]
	internal class Joint2DEditor : Editor
	{
		public class Styles
		{
			public readonly GUIStyle anchor = "U2D.pivotDot";

			public readonly GUIStyle anchorActive = "U2D.pivotDotActive";

			public readonly GUIStyle connectedAnchor = "U2D.dragDot";

			public readonly GUIStyle connectedAnchorActive = "U2D.dragDotActive";
		}

		private SerializedProperty m_BreakForce;

		private SerializedProperty m_BreakTorque;

		protected static Joint2DEditor.Styles s_Styles;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache0;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache1;

		public void OnEnable()
		{
			this.m_BreakForce = base.serializedObject.FindProperty("m_BreakForce");
			this.m_BreakTorque = base.serializedObject.FindProperty("m_BreakTorque");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField(this.m_BreakForce, new GUILayoutOption[0]);
			Type type = base.target.GetType();
			if (type != typeof(DistanceJoint2D) && type != typeof(TargetJoint2D) && type != typeof(SpringJoint2D))
			{
				EditorGUILayout.PropertyField(this.m_BreakTorque, new GUILayoutOption[0]);
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		protected bool HandleAnchor(ref Vector3 position, bool isConnectedAnchor)
		{
			if (Joint2DEditor.s_Styles == null)
			{
				Joint2DEditor.s_Styles = new Joint2DEditor.Styles();
			}
			Handles.CapFunction arg_5A_0;
			if (isConnectedAnchor)
			{
				if (Joint2DEditor.<>f__mg$cache0 == null)
				{
					Joint2DEditor.<>f__mg$cache0 = new Handles.CapFunction(Joint2DEditor.ConnectedAnchorHandleCap);
				}
				arg_5A_0 = Joint2DEditor.<>f__mg$cache0;
			}
			else
			{
				if (Joint2DEditor.<>f__mg$cache1 == null)
				{
					Joint2DEditor.<>f__mg$cache1 = new Handles.CapFunction(Joint2DEditor.AnchorHandleCap);
				}
				arg_5A_0 = Joint2DEditor.<>f__mg$cache1;
			}
			Handles.CapFunction capFunction = arg_5A_0;
			int id = base.target.GetInstanceID() + ((!isConnectedAnchor) ? 0 : 1);
			EditorGUI.BeginChangeCheck();
			position = Handles.Slider2D(id, position, Vector3.back, Vector3.right, Vector3.up, 0f, capFunction, Vector2.zero);
			return EditorGUI.EndChangeCheck();
		}

		public static void AnchorHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (controlID == GUIUtility.keyboardControl)
			{
				Joint2DEditor.HandleCap(controlID, position, Joint2DEditor.s_Styles.anchorActive, eventType);
			}
			else
			{
				Joint2DEditor.HandleCap(controlID, position, Joint2DEditor.s_Styles.anchor, eventType);
			}
		}

		public static void ConnectedAnchorHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (controlID == GUIUtility.keyboardControl)
			{
				Joint2DEditor.HandleCap(controlID, position, Joint2DEditor.s_Styles.connectedAnchorActive, eventType);
			}
			else
			{
				Joint2DEditor.HandleCap(controlID, position, Joint2DEditor.s_Styles.connectedAnchor, eventType);
			}
		}

		private static void HandleCap(int controlID, Vector3 position, GUIStyle guiStyle, EventType eventType)
		{
			if (eventType != EventType.Layout)
			{
				if (eventType == EventType.Repaint)
				{
					Handles.BeginGUI();
					position = HandleUtility.WorldToGUIPoint(position);
					float fixedWidth = guiStyle.fixedWidth;
					float fixedHeight = guiStyle.fixedHeight;
					Rect position2 = new Rect(position.x - fixedWidth / 2f, position.y - fixedHeight / 2f, fixedWidth, fixedHeight);
					guiStyle.Draw(position2, GUIContent.none, controlID);
					Handles.EndGUI();
				}
			}
			else
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangleInternal(position, Quaternion.identity, Vector2.zero));
			}
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
			Joint2DEditor.DrawAALine(anchor, vector);
			Joint2DEditor.DrawAALine(connectedAnchor + vector2, connectedAnchor - vector2);
			Joint2DEditor.DrawAALine(vector + vector2, vector - vector2);
		}

		private static Matrix4x4 GetAnchorSpaceMatrix(Transform transform)
		{
			return Matrix4x4.TRS(transform.position, Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z), transform.lossyScale);
		}

		protected static Vector3 TransformPoint(Transform transform, Vector3 position)
		{
			return Joint2DEditor.GetAnchorSpaceMatrix(transform).MultiplyPoint(position);
		}

		protected static Vector3 InverseTransformPoint(Transform transform, Vector3 position)
		{
			return Joint2DEditor.GetAnchorSpaceMatrix(transform).inverse.MultiplyPoint(position);
		}

		protected static Vector3 SnapToSprite(SpriteRenderer spriteRenderer, Vector3 position, float snapDistance)
		{
			Vector3 result;
			if (spriteRenderer == null)
			{
				result = position;
			}
			else
			{
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
						result = vector;
						return result;
					}
				}
				result = position;
			}
			return result;
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
