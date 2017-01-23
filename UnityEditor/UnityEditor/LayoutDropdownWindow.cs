using System;
using UnityEngine;

namespace UnityEditor
{
	internal class LayoutDropdownWindow : PopupWindowContent
	{
		private class Styles
		{
			public Color tableHeaderColor;

			public Color tableLineColor;

			public Color parentColor;

			public Color selfColor;

			public Color simpleAnchorColor;

			public Color stretchAnchorColor;

			public Color anchorCornerColor;

			public Color pivotColor;

			public GUIStyle frame;

			public GUIStyle label = new GUIStyle(EditorStyles.miniLabel);

			public Styles()
			{
				this.frame = new GUIStyle();
				Texture2D texture2D = new Texture2D(4, 4);
				texture2D.SetPixels(new Color[]
				{
					Color.white,
					Color.white,
					Color.white,
					Color.white,
					Color.white,
					Color.clear,
					Color.clear,
					Color.white,
					Color.white,
					Color.clear,
					Color.clear,
					Color.white,
					Color.white,
					Color.white,
					Color.white,
					Color.white
				});
				texture2D.filterMode = FilterMode.Point;
				texture2D.Apply();
				texture2D.hideFlags = HideFlags.HideAndDontSave;
				this.frame.normal.background = texture2D;
				this.frame.border = new RectOffset(2, 2, 2, 2);
				this.label.alignment = TextAnchor.LowerCenter;
				if (EditorGUIUtility.isProSkin)
				{
					this.tableHeaderColor = new Color(0.18f, 0.18f, 0.18f, 1f);
					this.tableLineColor = new Color(1f, 1f, 1f, 0.3f);
					this.parentColor = new Color(0.4f, 0.4f, 0.4f, 1f);
					this.selfColor = new Color(0.6f, 0.6f, 0.6f, 1f);
					this.simpleAnchorColor = new Color(0.7f, 0.3f, 0.3f, 1f);
					this.stretchAnchorColor = new Color(0f, 0.6f, 0.8f, 1f);
					this.anchorCornerColor = new Color(0.8f, 0.6f, 0f, 1f);
					this.pivotColor = new Color(0f, 0.6f, 0.8f, 1f);
				}
				else
				{
					this.tableHeaderColor = new Color(0.8f, 0.8f, 0.8f, 1f);
					this.tableLineColor = new Color(0f, 0f, 0f, 0.5f);
					this.parentColor = new Color(0.55f, 0.55f, 0.55f, 1f);
					this.selfColor = new Color(0.2f, 0.2f, 0.2f, 1f);
					this.simpleAnchorColor = new Color(0.8f, 0.3f, 0.3f, 1f);
					this.stretchAnchorColor = new Color(0.2f, 0.5f, 0.9f, 1f);
					this.anchorCornerColor = new Color(0.6f, 0.4f, 0f, 1f);
					this.pivotColor = new Color(0.2f, 0.5f, 0.9f, 1f);
				}
			}
		}

		public enum LayoutMode
		{
			Undefined = -1,
			Min,
			Middle,
			Max,
			Stretch
		}

		private static LayoutDropdownWindow.Styles s_Styles;

		private SerializedProperty m_AnchorMin;

		private SerializedProperty m_AnchorMax;

		private SerializedProperty m_Position;

		private SerializedProperty m_SizeDelta;

		private SerializedProperty m_Pivot;

		private Vector2[,] m_InitValues;

		private const int kTopPartHeight = 38;

		private static float[] kPivotsForModes = new float[]
		{
			0f,
			0.5f,
			1f,
			0.5f,
			0.5f
		};

		private static string[] kHLabels = new string[]
		{
			"custom",
			"left",
			"center",
			"right",
			"stretch",
			"%"
		};

		private static string[] kVLabels = new string[]
		{
			"custom",
			"top",
			"middle",
			"bottom",
			"stretch",
			"%"
		};

		public LayoutDropdownWindow(SerializedObject so)
		{
			this.m_AnchorMin = so.FindProperty("m_AnchorMin");
			this.m_AnchorMax = so.FindProperty("m_AnchorMax");
			this.m_Position = so.FindProperty("m_Position");
			this.m_SizeDelta = so.FindProperty("m_SizeDelta");
			this.m_Pivot = so.FindProperty("m_Pivot");
			this.m_InitValues = new Vector2[so.targetObjects.Length, 4];
			for (int i = 0; i < so.targetObjects.Length; i++)
			{
				RectTransform rectTransform = so.targetObjects[i] as RectTransform;
				this.m_InitValues[i, 0] = rectTransform.anchorMin;
				this.m_InitValues[i, 1] = rectTransform.anchorMax;
				this.m_InitValues[i, 2] = rectTransform.anchoredPosition;
				this.m_InitValues[i, 3] = rectTransform.sizeDelta;
			}
		}

		public override void OnOpen()
		{
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(base.editorWindow.Repaint));
		}

		public override void OnClose()
		{
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(base.editorWindow.Repaint));
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(262f, 300f);
		}

		public override void OnGUI(Rect rect)
		{
			if (LayoutDropdownWindow.s_Styles == null)
			{
				LayoutDropdownWindow.s_Styles = new LayoutDropdownWindow.Styles();
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
			{
				base.editorWindow.Close();
			}
			GUI.Label(new Rect(rect.x + 5f, rect.y + 3f, rect.width - 10f, 16f), new GUIContent("Anchor Presets"), EditorStyles.boldLabel);
			GUI.Label(new Rect(rect.x + 5f, rect.y + 3f + 16f, rect.width - 10f, 16f), new GUIContent("Shift: Also set pivot     Alt: Also set position"), EditorStyles.label);
			Color color = GUI.color;
			GUI.color = LayoutDropdownWindow.s_Styles.tableLineColor * color;
			GUI.DrawTexture(new Rect(0f, 37f, 400f, 1f), EditorGUIUtility.whiteTexture);
			GUI.color = color;
			GUI.BeginGroup(new Rect(rect.x, rect.y + 38f, rect.width, rect.height - 38f));
			this.TableGUI(rect);
			GUI.EndGroup();
		}

		private static LayoutDropdownWindow.LayoutMode SwappedVMode(LayoutDropdownWindow.LayoutMode vMode)
		{
			LayoutDropdownWindow.LayoutMode result;
			if (vMode == LayoutDropdownWindow.LayoutMode.Min)
			{
				result = LayoutDropdownWindow.LayoutMode.Max;
			}
			else if (vMode == LayoutDropdownWindow.LayoutMode.Max)
			{
				result = LayoutDropdownWindow.LayoutMode.Min;
			}
			else
			{
				result = vMode;
			}
			return result;
		}

		internal static void DrawLayoutModeHeadersOutsideRect(Rect rect, SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta)
		{
			LayoutDropdownWindow.LayoutMode layoutModeForAxis = LayoutDropdownWindow.GetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, 0);
			LayoutDropdownWindow.LayoutMode layoutMode = LayoutDropdownWindow.GetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, 1);
			layoutMode = LayoutDropdownWindow.SwappedVMode(layoutMode);
			LayoutDropdownWindow.DrawLayoutModeHeaderOutsideRect(rect, 0, layoutModeForAxis);
			LayoutDropdownWindow.DrawLayoutModeHeaderOutsideRect(rect, 1, layoutMode);
		}

		internal static void DrawLayoutModeHeaderOutsideRect(Rect position, int axis, LayoutDropdownWindow.LayoutMode mode)
		{
			Rect position2 = new Rect(position.x, position.y - 16f, position.width, 16f);
			Matrix4x4 matrix = GUI.matrix;
			if (axis == 1)
			{
				GUIUtility.RotateAroundPivot(-90f, position.center);
			}
			int num = (int)(mode + 1);
			GUI.Label(position2, (axis != 0) ? LayoutDropdownWindow.kVLabels[num] : LayoutDropdownWindow.kHLabels[num], LayoutDropdownWindow.s_Styles.label);
			GUI.matrix = matrix;
		}

		private void TableGUI(Rect rect)
		{
			int num = 6;
			int num2 = 31 + num * 2;
			int num3 = 0;
			int[] array = new int[]
			{
				15,
				30,
				30,
				30,
				45,
				45
			};
			Color color = GUI.color;
			int num4 = 62;
			GUI.color = LayoutDropdownWindow.s_Styles.tableHeaderColor * color;
			GUI.DrawTexture(new Rect(0f, 0f, 400f, (float)num4), EditorGUIUtility.whiteTexture);
			GUI.DrawTexture(new Rect(0f, 0f, (float)num4, 400f), EditorGUIUtility.whiteTexture);
			GUI.color = LayoutDropdownWindow.s_Styles.tableLineColor * color;
			GUI.DrawTexture(new Rect(0f, (float)num4, 400f, 1f), EditorGUIUtility.whiteTexture);
			GUI.DrawTexture(new Rect((float)num4, 0f, 1f, 400f), EditorGUIUtility.whiteTexture);
			GUI.color = color;
			LayoutDropdownWindow.LayoutMode layoutModeForAxis = LayoutDropdownWindow.GetLayoutModeForAxis(this.m_AnchorMin, this.m_AnchorMax, this.m_Position, this.m_SizeDelta, 0);
			LayoutDropdownWindow.LayoutMode layoutMode = LayoutDropdownWindow.GetLayoutModeForAxis(this.m_AnchorMin, this.m_AnchorMax, this.m_Position, this.m_SizeDelta, 1);
			layoutMode = LayoutDropdownWindow.SwappedVMode(layoutMode);
			bool shift = Event.current.shift;
			bool alt = Event.current.alt;
			int num5 = 5;
			for (int i = 0; i < num5; i++)
			{
				LayoutDropdownWindow.LayoutMode layoutMode2 = (LayoutDropdownWindow.LayoutMode)(i - 1);
				for (int j = 0; j < num5; j++)
				{
					LayoutDropdownWindow.LayoutMode layoutMode3 = (LayoutDropdownWindow.LayoutMode)(j - 1);
					if (i != 0 || j != 0 || layoutMode < LayoutDropdownWindow.LayoutMode.Min || layoutModeForAxis < LayoutDropdownWindow.LayoutMode.Min)
					{
						Rect position = new Rect((float)(i * (num2 + num3) + array[i]), (float)(j * (num2 + num3) + array[j]), (float)num2, (float)num2);
						if (j == 0 && (i != 0 || layoutModeForAxis == LayoutDropdownWindow.LayoutMode.Undefined))
						{
							LayoutDropdownWindow.DrawLayoutModeHeaderOutsideRect(position, 0, layoutMode2);
						}
						if (i == 0 && (j != 0 || layoutMode == LayoutDropdownWindow.LayoutMode.Undefined))
						{
							LayoutDropdownWindow.DrawLayoutModeHeaderOutsideRect(position, 1, layoutMode3);
						}
						bool flag = layoutMode2 == layoutModeForAxis && layoutMode3 == layoutMode;
						bool flag2 = false;
						if (i == 0 && layoutMode3 == layoutMode)
						{
							flag2 = true;
						}
						if (j == 0 && layoutMode2 == layoutModeForAxis)
						{
							flag2 = true;
						}
						if (Event.current.type == EventType.Repaint)
						{
							if (flag)
							{
								GUI.color = Color.white * color;
								LayoutDropdownWindow.s_Styles.frame.Draw(position, false, false, false, false);
							}
							else if (flag2)
							{
								GUI.color = new Color(1f, 1f, 1f, 0.7f) * color;
								LayoutDropdownWindow.s_Styles.frame.Draw(position, false, false, false, false);
							}
						}
						LayoutDropdownWindow.DrawLayoutMode(new Rect(position.x + (float)num, position.y + (float)num, position.width - (float)(num * 2), position.height - (float)(num * 2)), layoutMode2, layoutMode3, shift, alt);
						int clickCount = Event.current.clickCount;
						if (GUI.Button(position, GUIContent.none, GUIStyle.none))
						{
							LayoutDropdownWindow.SetLayoutModeForAxis(this.m_AnchorMin, this.m_AnchorMax, this.m_Position, this.m_SizeDelta, this.m_Pivot, 0, layoutMode2, shift, alt, this.m_InitValues);
							LayoutDropdownWindow.SetLayoutModeForAxis(this.m_AnchorMin, this.m_AnchorMax, this.m_Position, this.m_SizeDelta, this.m_Pivot, 1, LayoutDropdownWindow.SwappedVMode(layoutMode3), shift, alt, this.m_InitValues);
							if (clickCount == 2)
							{
								base.editorWindow.Close();
							}
							else
							{
								base.editorWindow.Repaint();
							}
						}
					}
				}
			}
			GUI.color = color;
		}

		private static LayoutDropdownWindow.LayoutMode GetLayoutModeForAxis(SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta, int axis)
		{
			LayoutDropdownWindow.LayoutMode result;
			if (anchorMin.vector2Value[axis] == 0f && anchorMax.vector2Value[axis] == 0f)
			{
				result = LayoutDropdownWindow.LayoutMode.Min;
			}
			else if (anchorMin.vector2Value[axis] == 0.5f && anchorMax.vector2Value[axis] == 0.5f)
			{
				result = LayoutDropdownWindow.LayoutMode.Middle;
			}
			else if (anchorMin.vector2Value[axis] == 1f && anchorMax.vector2Value[axis] == 1f)
			{
				result = LayoutDropdownWindow.LayoutMode.Max;
			}
			else if (anchorMin.vector2Value[axis] == 0f && anchorMax.vector2Value[axis] == 1f)
			{
				result = LayoutDropdownWindow.LayoutMode.Stretch;
			}
			else
			{
				result = LayoutDropdownWindow.LayoutMode.Undefined;
			}
			return result;
		}

		private static void SetLayoutModeForAxis(SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta, SerializedProperty pivot, int axis, LayoutDropdownWindow.LayoutMode layoutMode)
		{
			LayoutDropdownWindow.SetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, pivot, axis, layoutMode, false, false, null);
		}

		private static void SetLayoutModeForAxis(SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta, SerializedProperty pivot, int axis, LayoutDropdownWindow.LayoutMode layoutMode, bool doPivot)
		{
			LayoutDropdownWindow.SetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, pivot, axis, layoutMode, doPivot, false, null);
		}

		private static void SetLayoutModeForAxis(SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta, SerializedProperty pivot, int axis, LayoutDropdownWindow.LayoutMode layoutMode, bool doPivot, bool doPosition)
		{
			LayoutDropdownWindow.SetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, pivot, axis, layoutMode, doPivot, doPosition, null);
		}

		private static void SetLayoutModeForAxis(SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta, SerializedProperty pivot, int axis, LayoutDropdownWindow.LayoutMode layoutMode, bool doPivot, bool doPosition, Vector2[,] defaultValues)
		{
			anchorMin.serializedObject.ApplyModifiedProperties();
			for (int i = 0; i < anchorMin.serializedObject.targetObjects.Length; i++)
			{
				RectTransform rectTransform = anchorMin.serializedObject.targetObjects[i] as RectTransform;
				Undo.RecordObject(rectTransform, "Change Rectangle Anchors");
				if (doPosition)
				{
					if (defaultValues != null && defaultValues.Length > i)
					{
						Vector2 vector = rectTransform.anchorMin;
						vector[axis] = defaultValues[i, 0][axis];
						rectTransform.anchorMin = vector;
						vector = rectTransform.anchorMax;
						vector[axis] = defaultValues[i, 1][axis];
						rectTransform.anchorMax = vector;
						vector = rectTransform.anchoredPosition;
						vector[axis] = defaultValues[i, 2][axis];
						rectTransform.anchoredPosition = vector;
						vector = rectTransform.sizeDelta;
						vector[axis] = defaultValues[i, 3][axis];
						rectTransform.sizeDelta = vector;
					}
				}
				if (doPivot && layoutMode != LayoutDropdownWindow.LayoutMode.Undefined)
				{
					RectTransformEditor.SetPivotSmart(rectTransform, LayoutDropdownWindow.kPivotsForModes[(int)layoutMode], axis, true, true);
				}
				Vector2 vector2 = Vector2.zero;
				switch (layoutMode)
				{
				case LayoutDropdownWindow.LayoutMode.Min:
					RectTransformEditor.SetAnchorSmart(rectTransform, 0f, axis, false, true, true);
					RectTransformEditor.SetAnchorSmart(rectTransform, 0f, axis, true, true, true);
					vector2 = rectTransform.offsetMin;
					EditorUtility.SetDirty(rectTransform);
					break;
				case LayoutDropdownWindow.LayoutMode.Middle:
					RectTransformEditor.SetAnchorSmart(rectTransform, 0.5f, axis, false, true, true);
					RectTransformEditor.SetAnchorSmart(rectTransform, 0.5f, axis, true, true, true);
					vector2 = (rectTransform.offsetMin + rectTransform.offsetMax) * 0.5f;
					EditorUtility.SetDirty(rectTransform);
					break;
				case LayoutDropdownWindow.LayoutMode.Max:
					RectTransformEditor.SetAnchorSmart(rectTransform, 1f, axis, false, true, true);
					RectTransformEditor.SetAnchorSmart(rectTransform, 1f, axis, true, true, true);
					vector2 = rectTransform.offsetMax;
					EditorUtility.SetDirty(rectTransform);
					break;
				case LayoutDropdownWindow.LayoutMode.Stretch:
					RectTransformEditor.SetAnchorSmart(rectTransform, 0f, axis, false, true, true);
					RectTransformEditor.SetAnchorSmart(rectTransform, 1f, axis, true, true, true);
					vector2 = (rectTransform.offsetMin + rectTransform.offsetMax) * 0.5f;
					EditorUtility.SetDirty(rectTransform);
					break;
				}
				if (doPosition)
				{
					Vector2 anchoredPosition = rectTransform.anchoredPosition;
					anchoredPosition[axis] -= vector2[axis];
					rectTransform.anchoredPosition = anchoredPosition;
					if (layoutMode == LayoutDropdownWindow.LayoutMode.Stretch)
					{
						Vector2 sizeDelta2 = rectTransform.sizeDelta;
						sizeDelta2[axis] = 0f;
						rectTransform.sizeDelta = sizeDelta2;
					}
				}
			}
			anchorMin.serializedObject.Update();
		}

		internal static void DrawLayoutMode(Rect rect, SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta)
		{
			LayoutDropdownWindow.LayoutMode layoutModeForAxis = LayoutDropdownWindow.GetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, 0);
			LayoutDropdownWindow.LayoutMode vMode = LayoutDropdownWindow.GetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, 1);
			vMode = LayoutDropdownWindow.SwappedVMode(vMode);
			LayoutDropdownWindow.DrawLayoutMode(rect, layoutModeForAxis, vMode);
		}

		internal static void DrawLayoutMode(Rect position, LayoutDropdownWindow.LayoutMode hMode, LayoutDropdownWindow.LayoutMode vMode)
		{
			LayoutDropdownWindow.DrawLayoutMode(position, hMode, vMode, false, false);
		}

		internal static void DrawLayoutMode(Rect position, LayoutDropdownWindow.LayoutMode hMode, LayoutDropdownWindow.LayoutMode vMode, bool doPivot)
		{
			LayoutDropdownWindow.DrawLayoutMode(position, hMode, vMode, doPivot, false);
		}

		internal static void DrawLayoutMode(Rect position, LayoutDropdownWindow.LayoutMode hMode, LayoutDropdownWindow.LayoutMode vMode, bool doPivot, bool doPosition)
		{
			if (LayoutDropdownWindow.s_Styles == null)
			{
				LayoutDropdownWindow.s_Styles = new LayoutDropdownWindow.Styles();
			}
			Color color = GUI.color;
			int num = (int)Mathf.Min(position.width, position.height);
			if (num % 2 == 0)
			{
				num--;
			}
			int num2 = num / 2;
			if (num2 % 2 == 0)
			{
				num2++;
			}
			Vector2 b = (float)num * Vector2.one;
			Vector2 b2 = (float)num2 * Vector2.one;
			Vector2 vector = (position.size - b) / 2f;
			vector.x = Mathf.Floor(vector.x);
			vector.y = Mathf.Floor(vector.y);
			Vector2 vector2 = (position.size - b2) / 2f;
			vector2.x = Mathf.Floor(vector2.x);
			vector2.y = Mathf.Floor(vector2.y);
			Rect position2 = new Rect(position.x + vector.x, position.y + vector.y, b.x, b.y);
			Rect position3 = new Rect(position.x + vector2.x, position.y + vector2.y, b2.x, b2.y);
			if (doPosition)
			{
				for (int i = 0; i < 2; i++)
				{
					LayoutDropdownWindow.LayoutMode layoutMode = (i != 0) ? vMode : hMode;
					if (layoutMode == LayoutDropdownWindow.LayoutMode.Min)
					{
						Vector2 center = position3.center;
						int index;
						center[index = i] = center[index] + (position2.min[i] - position3.min[i]);
						position3.center = center;
					}
					if (layoutMode == LayoutDropdownWindow.LayoutMode.Middle)
					{
					}
					if (layoutMode == LayoutDropdownWindow.LayoutMode.Max)
					{
						Vector2 center2 = position3.center;
						int index2;
						center2[index2 = i] = center2[index2] + (position2.max[i] - position3.max[i]);
						position3.center = center2;
					}
					if (layoutMode == LayoutDropdownWindow.LayoutMode.Stretch)
					{
						Vector2 min = position3.min;
						Vector2 max = position3.max;
						min[i] = position2.min[i];
						max[i] = position2.max[i];
						position3.min = min;
						position3.max = max;
					}
				}
			}
			Rect rect = default(Rect);
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			for (int j = 0; j < 2; j++)
			{
				LayoutDropdownWindow.LayoutMode layoutMode2 = (j != 0) ? vMode : hMode;
				if (layoutMode2 == LayoutDropdownWindow.LayoutMode.Min)
				{
					zero[j] = position2.min[j] + 0.5f;
					zero2[j] = position2.min[j] + 0.5f;
				}
				if (layoutMode2 == LayoutDropdownWindow.LayoutMode.Middle)
				{
					zero[j] = position2.center[j];
					zero2[j] = position2.center[j];
				}
				if (layoutMode2 == LayoutDropdownWindow.LayoutMode.Max)
				{
					zero[j] = position2.max[j] - 0.5f;
					zero2[j] = position2.max[j] - 0.5f;
				}
				if (layoutMode2 == LayoutDropdownWindow.LayoutMode.Stretch)
				{
					zero[j] = position2.min[j] + 0.5f;
					zero2[j] = position2.max[j] - 0.5f;
				}
			}
			rect.min = zero;
			rect.max = zero2;
			if (Event.current.type == EventType.Repaint)
			{
				GUI.color = LayoutDropdownWindow.s_Styles.parentColor * color;
				LayoutDropdownWindow.s_Styles.frame.Draw(position2, false, false, false, false);
			}
			if (hMode != LayoutDropdownWindow.LayoutMode.Undefined && hMode != LayoutDropdownWindow.LayoutMode.Stretch)
			{
				GUI.color = LayoutDropdownWindow.s_Styles.simpleAnchorColor * color;
				GUI.DrawTexture(new Rect(rect.xMin - 0.5f, position2.y + 1f, 1f, position2.height - 2f), EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(new Rect(rect.xMax - 0.5f, position2.y + 1f, 1f, position2.height - 2f), EditorGUIUtility.whiteTexture);
			}
			if (vMode != LayoutDropdownWindow.LayoutMode.Undefined && vMode != LayoutDropdownWindow.LayoutMode.Stretch)
			{
				GUI.color = LayoutDropdownWindow.s_Styles.simpleAnchorColor * color;
				GUI.DrawTexture(new Rect(position2.x + 1f, rect.yMin - 0.5f, position2.width - 2f, 1f), EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(new Rect(position2.x + 1f, rect.yMax - 0.5f, position2.width - 2f, 1f), EditorGUIUtility.whiteTexture);
			}
			if (hMode == LayoutDropdownWindow.LayoutMode.Stretch)
			{
				GUI.color = LayoutDropdownWindow.s_Styles.stretchAnchorColor * color;
				LayoutDropdownWindow.DrawArrow(new Rect(position3.x + 1f, position3.center.y - 0.5f, position3.width - 2f, 1f));
			}
			if (vMode == LayoutDropdownWindow.LayoutMode.Stretch)
			{
				GUI.color = LayoutDropdownWindow.s_Styles.stretchAnchorColor * color;
				LayoutDropdownWindow.DrawArrow(new Rect(position3.center.x - 0.5f, position3.y + 1f, 1f, position3.height - 2f));
			}
			if (Event.current.type == EventType.Repaint)
			{
				GUI.color = LayoutDropdownWindow.s_Styles.selfColor * color;
				LayoutDropdownWindow.s_Styles.frame.Draw(position3, false, false, false, false);
			}
			if (doPivot && hMode != LayoutDropdownWindow.LayoutMode.Undefined && vMode != LayoutDropdownWindow.LayoutMode.Undefined)
			{
				Vector2 vector3 = new Vector2(Mathf.Lerp(position3.xMin + 0.5f, position3.xMax - 0.5f, LayoutDropdownWindow.kPivotsForModes[(int)hMode]), Mathf.Lerp(position3.yMin + 0.5f, position3.yMax - 0.5f, LayoutDropdownWindow.kPivotsForModes[(int)vMode]));
				GUI.color = LayoutDropdownWindow.s_Styles.pivotColor * color;
				GUI.DrawTexture(new Rect(vector3.x - 2.5f, vector3.y - 1.5f, 5f, 3f), EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(new Rect(vector3.x - 1.5f, vector3.y - 2.5f, 3f, 5f), EditorGUIUtility.whiteTexture);
			}
			if (hMode != LayoutDropdownWindow.LayoutMode.Undefined && vMode != LayoutDropdownWindow.LayoutMode.Undefined)
			{
				GUI.color = LayoutDropdownWindow.s_Styles.anchorCornerColor * color;
				GUI.DrawTexture(new Rect(rect.xMin - 1.5f, rect.yMin - 1.5f, 2f, 2f), EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(new Rect(rect.xMax - 0.5f, rect.yMin - 1.5f, 2f, 2f), EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(new Rect(rect.xMin - 1.5f, rect.yMax - 0.5f, 2f, 2f), EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(new Rect(rect.xMax - 0.5f, rect.yMax - 0.5f, 2f, 2f), EditorGUIUtility.whiteTexture);
			}
			GUI.color = color;
		}

		private static void DrawArrow(Rect lineRect)
		{
			GUI.DrawTexture(lineRect, EditorGUIUtility.whiteTexture);
			if (lineRect.width == 1f)
			{
				GUI.DrawTexture(new Rect(lineRect.x - 1f, lineRect.y + 1f, 3f, 1f), EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(new Rect(lineRect.x - 2f, lineRect.y + 2f, 5f, 1f), EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(new Rect(lineRect.x - 1f, lineRect.yMax - 2f, 3f, 1f), EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(new Rect(lineRect.x - 2f, lineRect.yMax - 3f, 5f, 1f), EditorGUIUtility.whiteTexture);
			}
			else
			{
				GUI.DrawTexture(new Rect(lineRect.x + 1f, lineRect.y - 1f, 1f, 3f), EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(new Rect(lineRect.x + 2f, lineRect.y - 2f, 1f, 5f), EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(new Rect(lineRect.xMax - 2f, lineRect.y - 1f, 1f, 3f), EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(new Rect(lineRect.xMax - 3f, lineRect.y - 2f, 1f, 5f), EditorGUIUtility.whiteTexture);
			}
		}
	}
}
