using System;
using UnityEngine;

namespace UnityEditor
{
	internal class LayerMatrixGUI
	{
		public delegate bool GetValueFunc(int layerA, int layerB);

		public delegate void SetValueFunc(int layerA, int layerB, bool val);

		public static void DoGUI(string title, ref bool show, ref Vector2 scrollPos, LayerMatrixGUI.GetValueFunc getValue, LayerMatrixGUI.SetValueFunc setValue)
		{
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				if (LayerMask.LayerToName(i) != string.Empty)
				{
					num++;
				}
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(0f);
			show = EditorGUILayout.Foldout(show, title);
			GUILayout.EndHorizontal();
			if (show)
			{
				scrollPos = GUILayout.BeginScrollView(scrollPos, new GUILayoutOption[]
				{
					GUILayout.MinHeight(120f),
					GUILayout.MaxHeight((float)(100 + (num + 1) * 16))
				});
				Rect rect = GUILayoutUtility.GetRect((float)(16 * num + 100), 100f);
				Rect topmostRect = GUIClip.topmostRect;
				Vector2 vector = GUIClip.Unclip(new Vector2(rect.x, rect.y));
				int num2 = 0;
				for (int j = 0; j < 32; j++)
				{
					if (LayerMask.LayerToName(j) != string.Empty)
					{
						float num3 = (float)(130 + (num - num2) * 16) - (topmostRect.width + scrollPos.x);
						if (num3 < 0f)
						{
							num3 = 0f;
						}
						Vector3 pos = new Vector3((float)(130 + 16 * (num - num2)) + vector.y + vector.x + scrollPos.y - num3, vector.y + scrollPos.y, 0f);
						GUI.matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one) * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
						if (SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 9.0"))
						{
							GUI.matrix *= Matrix4x4.TRS(new Vector3(-0.5f, -0.5f, 0f), Quaternion.identity, Vector3.one);
						}
						GUI.Label(new Rect(2f - vector.x - scrollPos.y, scrollPos.y - num3, 100f, 16f), LayerMask.LayerToName(j), "RightLabel");
						num2++;
					}
				}
				GUI.matrix = Matrix4x4.identity;
				num2 = 0;
				for (int k = 0; k < 32; k++)
				{
					if (LayerMask.LayerToName(k) != string.Empty)
					{
						int num4 = 0;
						Rect rect2 = GUILayoutUtility.GetRect((float)(30 + 16 * num + 100), 16f);
						GUI.Label(new Rect(rect2.x + 30f, rect2.y, 100f, 16f), LayerMask.LayerToName(k), "RightLabel");
						for (int l = 31; l >= 0; l--)
						{
							if (LayerMask.LayerToName(l) != string.Empty)
							{
								if (num4 < num - num2)
								{
									GUIContent content = new GUIContent(string.Empty, LayerMask.LayerToName(k) + "/" + LayerMask.LayerToName(l));
									bool flag = getValue(k, l);
									bool flag2 = GUI.Toggle(new Rect(130f + rect2.x + (float)(num4 * 16), rect2.y, 16f, 16f), flag, content);
									if (flag2 != flag)
									{
										setValue(k, l, flag2);
									}
								}
								num4++;
							}
						}
						num2++;
					}
				}
				GUILayout.EndScrollView();
			}
		}
	}
}
