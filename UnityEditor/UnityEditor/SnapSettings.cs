using System;
using UnityEngine;

namespace UnityEditor
{
	internal class SnapSettings : EditorWindow
	{
		private class Styles
		{
			public GUIStyle buttonLeft = "ButtonLeft";

			public GUIStyle buttonMid = "ButtonMid";

			public GUIStyle buttonRight = "ButtonRight";

			public GUIContent snapAllAxes = EditorGUIUtility.TextContent("Snap All Axes|Snaps selected objects to the grid");

			public GUIContent snapX = EditorGUIUtility.TextContent("X|Snaps selected objects to the grid on the x axis");

			public GUIContent snapY = EditorGUIUtility.TextContent("Y|Snaps selected objects to the grid on the y axis");

			public GUIContent snapZ = EditorGUIUtility.TextContent("Z|Snaps selected objects to the grid on the z axis");

			public GUIContent moveX = EditorGUIUtility.TextContent("Move X|Grid spacing X");

			public GUIContent moveY = EditorGUIUtility.TextContent("Move Y|Grid spacing Y");

			public GUIContent moveZ = EditorGUIUtility.TextContent("Move Z|Grid spacing Z");

			public GUIContent scale = EditorGUIUtility.TextContent("Scale|Grid spacing for scaling");

			public GUIContent rotation = EditorGUIUtility.TextContent("Rotation|Grid spacing for rotation in degrees");
		}

		private static float s_MoveSnapX;

		private static float s_MoveSnapY;

		private static float s_MoveSnapZ;

		private static float s_ScaleSnap;

		private static float s_RotationSnap;

		private static bool s_Initialized;

		private static SnapSettings.Styles ms_Styles;

		public static Vector3 move
		{
			get
			{
				SnapSettings.Initialize();
				return new Vector3(SnapSettings.s_MoveSnapX, SnapSettings.s_MoveSnapY, SnapSettings.s_MoveSnapZ);
			}
			set
			{
				EditorPrefs.SetFloat("MoveSnapX", value.x);
				SnapSettings.s_MoveSnapX = value.x;
				EditorPrefs.SetFloat("MoveSnapY", value.y);
				SnapSettings.s_MoveSnapY = value.y;
				EditorPrefs.SetFloat("MoveSnapZ", value.z);
				SnapSettings.s_MoveSnapZ = value.z;
			}
		}

		public static float scale
		{
			get
			{
				SnapSettings.Initialize();
				return SnapSettings.s_ScaleSnap;
			}
			set
			{
				EditorPrefs.SetFloat("ScaleSnap", value);
				SnapSettings.s_ScaleSnap = value;
			}
		}

		public static float rotation
		{
			get
			{
				SnapSettings.Initialize();
				return SnapSettings.s_RotationSnap;
			}
			set
			{
				EditorPrefs.SetFloat("RotationSnap", value);
				SnapSettings.s_RotationSnap = value;
			}
		}

		private static void Initialize()
		{
			if (!SnapSettings.s_Initialized)
			{
				SnapSettings.s_MoveSnapX = EditorPrefs.GetFloat("MoveSnapX", 1f);
				SnapSettings.s_MoveSnapY = EditorPrefs.GetFloat("MoveSnapY", 1f);
				SnapSettings.s_MoveSnapZ = EditorPrefs.GetFloat("MoveSnapZ", 1f);
				SnapSettings.s_ScaleSnap = EditorPrefs.GetFloat("ScaleSnap", 0.1f);
				SnapSettings.s_RotationSnap = EditorPrefs.GetFloat("RotationSnap", 15f);
				SnapSettings.s_Initialized = true;
			}
		}

		[MenuItem("Edit/Snap Settings...")]
		private static void ShowSnapSettings()
		{
			EditorWindow.GetWindowWithRect<SnapSettings>(new Rect(100f, 100f, 230f, 130f), true, "Snap settings");
		}

		private void OnGUI()
		{
			if (SnapSettings.ms_Styles == null)
			{
				SnapSettings.ms_Styles = new SnapSettings.Styles();
			}
			GUILayout.Space(5f);
			EditorGUI.BeginChangeCheck();
			Vector3 move = SnapSettings.move;
			move.x = EditorGUILayout.FloatField(SnapSettings.ms_Styles.moveX, move.x, new GUILayoutOption[0]);
			move.y = EditorGUILayout.FloatField(SnapSettings.ms_Styles.moveY, move.y, new GUILayoutOption[0]);
			move.z = EditorGUILayout.FloatField(SnapSettings.ms_Styles.moveZ, move.z, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				if (move.x <= 0f)
				{
					move.x = SnapSettings.move.x;
				}
				if (move.y <= 0f)
				{
					move.y = SnapSettings.move.y;
				}
				if (move.z <= 0f)
				{
					move.z = SnapSettings.move.z;
				}
				SnapSettings.move = move;
			}
			SnapSettings.scale = EditorGUILayout.FloatField(SnapSettings.ms_Styles.scale, SnapSettings.scale, new GUILayoutOption[0]);
			SnapSettings.rotation = EditorGUILayout.FloatField(SnapSettings.ms_Styles.rotation, SnapSettings.rotation, new GUILayoutOption[0]);
			GUILayout.Space(5f);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button(SnapSettings.ms_Styles.snapAllAxes, SnapSettings.ms_Styles.buttonLeft, new GUILayoutOption[0]))
			{
				flag = true;
				flag2 = true;
				flag3 = true;
			}
			if (GUILayout.Button(SnapSettings.ms_Styles.snapX, SnapSettings.ms_Styles.buttonMid, new GUILayoutOption[0]))
			{
				flag = true;
			}
			if (GUILayout.Button(SnapSettings.ms_Styles.snapY, SnapSettings.ms_Styles.buttonMid, new GUILayoutOption[0]))
			{
				flag2 = true;
			}
			if (GUILayout.Button(SnapSettings.ms_Styles.snapZ, SnapSettings.ms_Styles.buttonRight, new GUILayoutOption[0]))
			{
				flag3 = true;
			}
			GUILayout.EndHorizontal();
			if (flag | flag2 | flag3)
			{
				Vector3 vector = new Vector3(1f / SnapSettings.move.x, 1f / SnapSettings.move.y, 1f / SnapSettings.move.z);
				Undo.RecordObjects(Selection.transforms, "Snap " + ((Selection.transforms.Length != 1) ? " selection" : Selection.activeGameObject.name) + " to grid");
				Transform[] transforms = Selection.transforms;
				for (int i = 0; i < transforms.Length; i++)
				{
					Transform transform = transforms[i];
					Vector3 position = transform.position;
					if (flag)
					{
						position.x = Mathf.Round(position.x * vector.x) / vector.x;
					}
					if (flag2)
					{
						position.y = Mathf.Round(position.y * vector.y) / vector.y;
					}
					if (flag3)
					{
						position.z = Mathf.Round(position.z * vector.z) / vector.z;
					}
					transform.position = position;
				}
			}
		}
	}
}
