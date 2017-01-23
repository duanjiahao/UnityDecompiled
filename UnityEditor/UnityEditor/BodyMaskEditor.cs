using System;
using UnityEngine;

namespace UnityEditor
{
	internal class BodyMaskEditor
	{
		private class Styles
		{
			public GUIContent UnityDude = EditorGUIUtility.IconContent("AvatarInspector/BodySIlhouette");

			public GUIContent PickingTexture = EditorGUIUtility.IconContent("AvatarInspector/BodyPartPicker");

			public GUIContent[] BodyPart = new GUIContent[]
			{
				EditorGUIUtility.IconContent("AvatarInspector/MaskEditor_Root"),
				EditorGUIUtility.IconContent("AvatarInspector/Torso"),
				EditorGUIUtility.IconContent("AvatarInspector/Head"),
				EditorGUIUtility.IconContent("AvatarInspector/LeftLeg"),
				EditorGUIUtility.IconContent("AvatarInspector/RightLeg"),
				EditorGUIUtility.IconContent("AvatarInspector/LeftArm"),
				EditorGUIUtility.IconContent("AvatarInspector/RightArm"),
				EditorGUIUtility.IconContent("AvatarInspector/LeftFingers"),
				EditorGUIUtility.IconContent("AvatarInspector/RightFingers"),
				EditorGUIUtility.IconContent("AvatarInspector/LeftFeetIk"),
				EditorGUIUtility.IconContent("AvatarInspector/RightFeetIk"),
				EditorGUIUtility.IconContent("AvatarInspector/LeftFingersIk"),
				EditorGUIUtility.IconContent("AvatarInspector/RightFingersIk")
			};
		}

		private static BodyMaskEditor.Styles styles = new BodyMaskEditor.Styles();

		protected static Color[] m_MaskBodyPartPicker = new Color[]
		{
			new Color(1f, 0.5647059f, 0f),
			new Color(0f, 0.68235296f, 0.9411765f),
			new Color(0.670588255f, 0.627451f, 0f),
			new Color(0f, 1f, 1f),
			new Color(0.968627453f, 0.5921569f, 0.4745098f),
			new Color(0f, 1f, 0f),
			new Color(0.3372549f, 0.454901963f, 0.7254902f),
			new Color(1f, 1f, 0f),
			new Color(0.509803951f, 0.7921569f, 0.6117647f),
			new Color(0.321568638f, 0.321568638f, 0.321568638f),
			new Color(1f, 0.4509804f, 0.4509804f),
			new Color(0.623529434f, 0.623529434f, 0.623529434f),
			new Color(0.7921569f, 0.7921569f, 0.7921569f),
			new Color(0.396078438f, 0.396078438f, 0.396078438f)
		};

		private static string sAvatarBodyMaskStr = "AvatarMask";

		private static int s_Hint = BodyMaskEditor.sAvatarBodyMaskStr.GetHashCode();

		public static void Show(SerializedProperty bodyMask, int count)
		{
			if (BodyMaskEditor.styles.UnityDude.image)
			{
				Rect rect = GUILayoutUtility.GetRect(BodyMaskEditor.styles.UnityDude, GUIStyle.none, new GUILayoutOption[]
				{
					GUILayout.MaxWidth((float)BodyMaskEditor.styles.UnityDude.image.width)
				});
				rect.x += (GUIView.current.position.width - rect.width) / 2f;
				Color color = GUI.color;
				GUI.color = ((bodyMask.GetArrayElementAtIndex(0).intValue != 1) ? Color.red : Color.green);
				if (BodyMaskEditor.styles.BodyPart[0].image)
				{
					GUI.DrawTexture(rect, BodyMaskEditor.styles.BodyPart[0].image);
				}
				GUI.color = new Color(0.2f, 0.2f, 0.2f, 1f);
				GUI.DrawTexture(rect, BodyMaskEditor.styles.UnityDude.image);
				for (int i = 1; i < count; i++)
				{
					GUI.color = ((bodyMask.GetArrayElementAtIndex(i).intValue != 1) ? Color.red : Color.green);
					if (BodyMaskEditor.styles.BodyPart[i].image)
					{
						GUI.DrawTexture(rect, BodyMaskEditor.styles.BodyPart[i].image);
					}
				}
				GUI.color = color;
				BodyMaskEditor.DoPicking(rect, bodyMask, count);
			}
		}

		protected static void DoPicking(Rect rect, SerializedProperty bodyMask, int count)
		{
			if (BodyMaskEditor.styles.PickingTexture.image)
			{
				int controlID = GUIUtility.GetControlID(BodyMaskEditor.s_Hint, FocusType.Passive, rect);
				Event current = Event.current;
				EventType typeForControl = current.GetTypeForControl(controlID);
				if (typeForControl == EventType.MouseDown)
				{
					if (rect.Contains(current.mousePosition))
					{
						current.Use();
						int x = (int)current.mousePosition.x - (int)rect.x;
						int y = BodyMaskEditor.styles.UnityDude.image.height - ((int)current.mousePosition.y - (int)rect.y);
						Texture2D texture2D = BodyMaskEditor.styles.PickingTexture.image as Texture2D;
						Color pixel = texture2D.GetPixel(x, y);
						bool flag = false;
						for (int i = 0; i < count; i++)
						{
							if (BodyMaskEditor.m_MaskBodyPartPicker[i] == pixel)
							{
								GUI.changed = true;
								bodyMask.GetArrayElementAtIndex(i).intValue = ((bodyMask.GetArrayElementAtIndex(i).intValue != 1) ? 1 : 0);
								flag = true;
							}
						}
						if (!flag)
						{
							bool flag2 = false;
							int num = 0;
							while (num < count && !flag2)
							{
								flag2 = (bodyMask.GetArrayElementAtIndex(num).intValue == 1);
								num++;
							}
							for (int j = 0; j < count; j++)
							{
								bodyMask.GetArrayElementAtIndex(j).intValue = (flag2 ? 0 : 1);
							}
							GUI.changed = true;
						}
					}
				}
			}
		}
	}
}
