using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class AvatarControl
	{
		private class Styles
		{
			public GUIContent[] Silhouettes = new GUIContent[]
			{
				EditorGUIUtility.IconContent("AvatarInspector/BodySilhouette"),
				EditorGUIUtility.IconContent("AvatarInspector/HeadZoomSilhouette"),
				EditorGUIUtility.IconContent("AvatarInspector/LeftHandZoomSilhouette"),
				EditorGUIUtility.IconContent("AvatarInspector/RightHandZoomSilhouette")
			};

			public GUIContent[,] BodyPart;

			public Styles()
			{
				GUIContent[,] expr_49 = new GUIContent[4, 9];
				expr_49[0, 1] = EditorGUIUtility.IconContent("AvatarInspector/Torso");
				expr_49[0, 2] = EditorGUIUtility.IconContent("AvatarInspector/Head");
				expr_49[0, 3] = EditorGUIUtility.IconContent("AvatarInspector/LeftArm");
				expr_49[0, 4] = EditorGUIUtility.IconContent("AvatarInspector/LeftFingers");
				expr_49[0, 5] = EditorGUIUtility.IconContent("AvatarInspector/RightArm");
				expr_49[0, 6] = EditorGUIUtility.IconContent("AvatarInspector/RightFingers");
				expr_49[0, 7] = EditorGUIUtility.IconContent("AvatarInspector/LeftLeg");
				expr_49[0, 8] = EditorGUIUtility.IconContent("AvatarInspector/RightLeg");
				expr_49[1, 2] = EditorGUIUtility.IconContent("AvatarInspector/HeadZoom");
				expr_49[2, 4] = EditorGUIUtility.IconContent("AvatarInspector/LeftHandZoom");
				expr_49[3, 6] = EditorGUIUtility.IconContent("AvatarInspector/RightHandZoom");
				this.BodyPart = expr_49;
				base..ctor();
			}
		}

		public enum BodyPartColor
		{
			Off,
			Green,
			Red,
			IKGreen = 4,
			IKRed = 8
		}

		public delegate AvatarControl.BodyPartColor BodyPartFeedback(BodyPart bodyPart);

		private static AvatarControl.Styles s_Styles;

		private static Vector2[,] s_BonePositions;

		private static AvatarControl.Styles styles
		{
			get
			{
				if (AvatarControl.s_Styles == null)
				{
					AvatarControl.s_Styles = new AvatarControl.Styles();
				}
				return AvatarControl.s_Styles;
			}
		}

		static AvatarControl()
		{
			AvatarControl.s_BonePositions = new Vector2[4, HumanTrait.BoneCount];
			int num = 0;
			AvatarControl.s_BonePositions[num, 0] = new Vector2(0f, 0.08f);
			AvatarControl.s_BonePositions[num, 1] = new Vector2(0.16f, 0.01f);
			AvatarControl.s_BonePositions[num, 2] = new Vector2(-0.16f, 0.01f);
			AvatarControl.s_BonePositions[num, 3] = new Vector2(0.21f, -0.4f);
			AvatarControl.s_BonePositions[num, 4] = new Vector2(-0.21f, -0.4f);
			AvatarControl.s_BonePositions[num, 5] = new Vector2(0.23f, -0.8f);
			AvatarControl.s_BonePositions[num, 6] = new Vector2(-0.23f, -0.8f);
			AvatarControl.s_BonePositions[num, 7] = new Vector2(0f, 0.2f);
			AvatarControl.s_BonePositions[num, 8] = new Vector2(0f, 0.35f);
			AvatarControl.s_BonePositions[num, 54] = new Vector2(0f, 0.5f);
			AvatarControl.s_BonePositions[num, 9] = new Vector2(0f, 0.66f);
			AvatarControl.s_BonePositions[num, 10] = new Vector2(0f, 0.76f);
			AvatarControl.s_BonePositions[num, 11] = new Vector2(0.14f, 0.6f);
			AvatarControl.s_BonePositions[num, 12] = new Vector2(-0.14f, 0.6f);
			AvatarControl.s_BonePositions[num, 13] = new Vector2(0.3f, 0.57f);
			AvatarControl.s_BonePositions[num, 14] = new Vector2(-0.3f, 0.57f);
			AvatarControl.s_BonePositions[num, 15] = new Vector2(0.48f, 0.3f);
			AvatarControl.s_BonePositions[num, 16] = new Vector2(-0.48f, 0.3f);
			AvatarControl.s_BonePositions[num, 17] = new Vector2(0.66f, 0.03f);
			AvatarControl.s_BonePositions[num, 18] = new Vector2(-0.66f, 0.03f);
			AvatarControl.s_BonePositions[num, 19] = new Vector2(0.25f, -0.89f);
			AvatarControl.s_BonePositions[num, 20] = new Vector2(-0.25f, -0.89f);
			num = 1;
			AvatarControl.s_BonePositions[num, 9] = new Vector2(-0.2f, -0.62f);
			AvatarControl.s_BonePositions[num, 10] = new Vector2(-0.15f, -0.3f);
			AvatarControl.s_BonePositions[num, 21] = new Vector2(0.63f, 0.16f);
			AvatarControl.s_BonePositions[num, 22] = new Vector2(0.15f, 0.16f);
			AvatarControl.s_BonePositions[num, 23] = new Vector2(0.45f, -0.4f);
			num = 2;
			AvatarControl.s_BonePositions[num, 24] = new Vector2(-0.35f, 0.11f);
			AvatarControl.s_BonePositions[num, 27] = new Vector2(0.19f, 0.11f);
			AvatarControl.s_BonePositions[num, 30] = new Vector2(0.22f, 0f);
			AvatarControl.s_BonePositions[num, 33] = new Vector2(0.16f, -0.12f);
			AvatarControl.s_BonePositions[num, 36] = new Vector2(0.09f, -0.23f);
			AvatarControl.s_BonePositions[num, 26] = new Vector2(-0.03f, 0.33f);
			AvatarControl.s_BonePositions[num, 29] = new Vector2(0.65f, 0.16f);
			AvatarControl.s_BonePositions[num, 32] = new Vector2(0.74f, 0f);
			AvatarControl.s_BonePositions[num, 35] = new Vector2(0.66f, -0.14f);
			AvatarControl.s_BonePositions[num, 38] = new Vector2(0.45f, -0.25f);
			for (int i = 0; i < 5; i++)
			{
				AvatarControl.s_BonePositions[num, 25 + i * 3] = Vector2.Lerp(AvatarControl.s_BonePositions[num, 24 + i * 3], AvatarControl.s_BonePositions[num, 26 + i * 3], 0.58f);
			}
			num = 3;
			for (int j = 0; j < 15; j++)
			{
				AvatarControl.s_BonePositions[num, 24 + j + 15] = Vector2.Scale(AvatarControl.s_BonePositions[num - 1, 24 + j], new Vector2(-1f, 1f));
			}
		}

		public static int ShowBoneMapping(int shownBodyView, AvatarControl.BodyPartFeedback bodyPartCallback, AvatarSetupTool.BoneWrapper[] bones, SerializedObject serializedObject, AvatarMappingEditor editor)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (AvatarControl.styles.Silhouettes[shownBodyView].image)
			{
				Rect rect = GUILayoutUtility.GetRect(AvatarControl.styles.Silhouettes[shownBodyView], GUIStyle.none, new GUILayoutOption[]
				{
					GUILayout.MaxWidth((float)AvatarControl.styles.Silhouettes[shownBodyView].image.width)
				});
				AvatarControl.DrawBodyParts(rect, shownBodyView, bodyPartCallback);
				for (int i = 0; i < bones.Length; i++)
				{
					AvatarControl.DrawBone(shownBodyView, i, rect, bones[i], serializedObject, editor);
				}
			}
			else
			{
				GUILayout.Label("texture missing,\nfix me!", new GUILayoutOption[0]);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			Rect lastRect = GUILayoutUtility.GetLastRect();
			string[] array = new string[]
			{
				"Body",
				"Head",
				"Left Hand",
				"Right Hand"
			};
			lastRect.x += 5f;
			lastRect.width = 70f;
			lastRect.yMin = lastRect.yMax - 69f;
			lastRect.height = 16f;
			for (int j = 0; j < array.Length; j++)
			{
				if (GUI.Toggle(lastRect, shownBodyView == j, array[j], EditorStyles.miniButton))
				{
					shownBodyView = j;
				}
				lastRect.y += 16f;
			}
			return shownBodyView;
		}

		public static void DrawBodyParts(Rect rect, int shownBodyView, AvatarControl.BodyPartFeedback bodyPartCallback)
		{
			GUI.color = new Color(0.2f, 0.2f, 0.2f, 1f);
			if (AvatarControl.styles.Silhouettes[shownBodyView] != null)
			{
				GUI.DrawTexture(rect, AvatarControl.styles.Silhouettes[shownBodyView].image);
			}
			for (int i = 1; i < 9; i++)
			{
				AvatarControl.DrawBodyPart(shownBodyView, i, rect, bodyPartCallback((BodyPart)i));
			}
		}

		protected static void DrawBodyPart(int shownBodyView, int i, Rect rect, AvatarControl.BodyPartColor bodyPartColor)
		{
			if (AvatarControl.styles.BodyPart[shownBodyView, i] != null && AvatarControl.styles.BodyPart[shownBodyView, i].image != null)
			{
				if ((bodyPartColor & AvatarControl.BodyPartColor.Green) == AvatarControl.BodyPartColor.Green)
				{
					GUI.color = Color.green;
				}
				else if ((bodyPartColor & AvatarControl.BodyPartColor.Red) == AvatarControl.BodyPartColor.Red)
				{
					GUI.color = Color.red;
				}
				else
				{
					GUI.color = Color.gray;
				}
				GUI.DrawTexture(rect, AvatarControl.styles.BodyPart[shownBodyView, i].image);
				GUI.color = Color.white;
			}
		}

		public static List<int> GetViewsThatContainBone(int bone)
		{
			List<int> list = new List<int>();
			List<int> result;
			if (bone < 0 || bone >= HumanTrait.BoneCount)
			{
				result = list;
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					if (AvatarControl.s_BonePositions[i, bone] != Vector2.zero)
					{
						list.Add(i);
					}
				}
				result = list;
			}
			return result;
		}

		protected static void DrawBone(int shownBodyView, int i, Rect rect, AvatarSetupTool.BoneWrapper bone, SerializedObject serializedObject, AvatarMappingEditor editor)
		{
			if (!(AvatarControl.s_BonePositions[shownBodyView, i] == Vector2.zero))
			{
				Vector2 b = AvatarControl.s_BonePositions[shownBodyView, i];
				b.y *= -1f;
				b.Scale(new Vector2(rect.width * 0.5f, rect.height * 0.5f));
				b = rect.center + b;
				int num = 19;
				Rect rect2 = new Rect(b.x - (float)num * 0.5f, b.y - (float)num * 0.5f, (float)num, (float)num);
				bone.BoneDotGUI(rect2, rect2, i, true, true, true, serializedObject, editor);
			}
		}
	}
}
