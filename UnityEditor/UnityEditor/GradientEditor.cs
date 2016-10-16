using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class GradientEditor
	{
		private class Styles
		{
			public GUIStyle upSwatch = "Grad Up Swatch";

			public GUIStyle upSwatchOverlay = "Grad Up Swatch Overlay";

			public GUIStyle downSwatch = "Grad Down Swatch";

			public GUIStyle downSwatchOverlay = "Grad Down Swatch Overlay";

			public GUIContent alphaText = new GUIContent("Alpha");

			public GUIContent colorText = new GUIContent("Color");

			public GUIContent locationText = new GUIContent("Location");

			public GUIContent percentText = new GUIContent("%");

			private static GUIStyle GetStyle(string name)
			{
				GUISkin gUISkin = (GUISkin)EditorGUIUtility.LoadRequired("GradientEditor.GUISkin");
				return gUISkin.GetStyle(name);
			}
		}

		public class Swatch
		{
			public float m_Time;

			public Color m_Value;

			public bool m_IsAlpha;

			public Swatch(float time, Color value, bool isAlpha)
			{
				this.m_Time = time;
				this.m_Value = value;
				this.m_IsAlpha = isAlpha;
			}
		}

		private const int k_MaxNumKeys = 8;

		private static GradientEditor.Styles s_Styles;

		private static Texture2D s_BackgroundTexture;

		private List<GradientEditor.Swatch> m_RGBSwatches;

		private List<GradientEditor.Swatch> m_AlphaSwatches;

		[NonSerialized]
		private GradientEditor.Swatch m_SelectedSwatch;

		private Gradient m_Gradient;

		private int m_NumSteps;

		public Gradient target
		{
			get
			{
				return this.m_Gradient;
			}
		}

		public void Init(Gradient gradient, int numSteps)
		{
			this.m_Gradient = gradient;
			this.m_NumSteps = numSteps;
			this.BuildArrays();
			if (this.m_RGBSwatches.Count > 0)
			{
				this.m_SelectedSwatch = this.m_RGBSwatches[0];
			}
		}

		private float GetTime(float actualTime)
		{
			actualTime = Mathf.Clamp01(actualTime);
			if (this.m_NumSteps > 1)
			{
				float num = 1f / (float)(this.m_NumSteps - 1);
				int num2 = Mathf.RoundToInt(actualTime / num);
				return (float)num2 / (float)(this.m_NumSteps - 1);
			}
			return actualTime;
		}

		private void BuildArrays()
		{
			if (this.m_Gradient == null)
			{
				return;
			}
			GradientColorKey[] colorKeys = this.m_Gradient.colorKeys;
			this.m_RGBSwatches = new List<GradientEditor.Swatch>(colorKeys.Length);
			for (int i = 0; i < colorKeys.Length; i++)
			{
				Color color = colorKeys[i].color;
				color.a = 1f;
				this.m_RGBSwatches.Add(new GradientEditor.Swatch(colorKeys[i].time, color, false));
			}
			GradientAlphaKey[] alphaKeys = this.m_Gradient.alphaKeys;
			this.m_AlphaSwatches = new List<GradientEditor.Swatch>(alphaKeys.Length);
			for (int j = 0; j < alphaKeys.Length; j++)
			{
				float alpha = alphaKeys[j].alpha;
				this.m_AlphaSwatches.Add(new GradientEditor.Swatch(alphaKeys[j].time, new Color(alpha, alpha, alpha, 1f), true));
			}
		}

		public static void DrawGradientWithBackground(Rect position, Texture2D gradientTexture)
		{
			Rect position2 = new Rect(position.x + 1f, position.y + 1f, position.width - 2f, position.height - 2f);
			Texture2D backgroundTexture = GradientEditor.GetBackgroundTexture();
			Rect texCoords = new Rect(0f, 0f, position2.width / (float)backgroundTexture.width, position2.height / (float)backgroundTexture.height);
			GUI.DrawTextureWithTexCoords(position2, backgroundTexture, texCoords, false);
			if (gradientTexture != null)
			{
				GUI.DrawTexture(position2, gradientTexture, ScaleMode.StretchToFill, true);
			}
			GUI.Label(position, GUIContent.none, EditorStyles.colorPickerBox);
		}

		public void OnGUI(Rect position)
		{
			if (GradientEditor.s_Styles == null)
			{
				GradientEditor.s_Styles = new GradientEditor.Styles();
			}
			float num = 16f;
			float num2 = 30f;
			float num3 = position.height - 2f * num - num2;
			position.height = num;
			this.ShowSwatchArray(position, this.m_AlphaSwatches, true);
			position.y += num;
			if (Event.current.type == EventType.Repaint)
			{
				position.height = num3;
				GradientEditor.DrawGradientWithBackground(position, GradientPreviewCache.GetGradientPreview(this.m_Gradient));
			}
			position.y += num3;
			position.height = num;
			this.ShowSwatchArray(position, this.m_RGBSwatches, false);
			if (this.m_SelectedSwatch != null)
			{
				position.y += num;
				position.height = num2;
				position.y += 10f;
				float num4 = 45f;
				float num5 = 60f;
				float num6 = 20f;
				float labelWidth = 50f;
				float num7 = num5 + num6 + num5 + num4;
				Rect position2 = position;
				position2.height = 18f;
				position2.x += 17f;
				position2.width -= num7;
				EditorGUIUtility.labelWidth = labelWidth;
				if (this.m_SelectedSwatch.m_IsAlpha)
				{
					EditorGUIUtility.fieldWidth = 30f;
					EditorGUI.BeginChangeCheck();
					float num8 = (float)EditorGUI.IntSlider(position2, GradientEditor.s_Styles.alphaText, (int)(this.m_SelectedSwatch.m_Value.r * 255f), 0, 255) / 255f;
					if (EditorGUI.EndChangeCheck())
					{
						num8 = Mathf.Clamp01(num8);
						this.m_SelectedSwatch.m_Value.r = (this.m_SelectedSwatch.m_Value.g = (this.m_SelectedSwatch.m_Value.b = num8));
						this.AssignBack();
						HandleUtility.Repaint();
					}
				}
				else
				{
					EditorGUI.BeginChangeCheck();
					this.m_SelectedSwatch.m_Value = EditorGUI.ColorField(position2, GradientEditor.s_Styles.colorText, this.m_SelectedSwatch.m_Value, true, false);
					if (EditorGUI.EndChangeCheck())
					{
						this.AssignBack();
						HandleUtility.Repaint();
					}
				}
				position2.x += position2.width + num6;
				position2.width = num4 + num5;
				EditorGUIUtility.labelWidth = num5;
				string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
				EditorGUI.kFloatFieldFormatString = "f1";
				EditorGUI.BeginChangeCheck();
				float value = EditorGUI.FloatField(position2, GradientEditor.s_Styles.locationText, this.m_SelectedSwatch.m_Time * 100f) / 100f;
				if (EditorGUI.EndChangeCheck())
				{
					this.m_SelectedSwatch.m_Time = Mathf.Clamp(value, 0f, 1f);
					this.AssignBack();
				}
				EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
				position2.x += position2.width;
				position2.width = 20f;
				GUI.Label(position2, GradientEditor.s_Styles.percentText);
			}
		}

		private void ShowSwatchArray(Rect position, List<GradientEditor.Swatch> swatches, bool isAlpha)
		{
			int controlID = GUIUtility.GetControlID(652347689, FocusType.Passive);
			Event current = Event.current;
			float time = this.GetTime((Event.current.mousePosition.x - position.x) / position.width);
			Vector2 point = new Vector3(position.x + time * position.width, Event.current.mousePosition.y);
			EventType typeForControl = current.GetTypeForControl(controlID);
			switch (typeForControl)
			{
			case EventType.MouseDown:
			{
				Rect rect = position;
				rect.xMin -= 10f;
				rect.xMax += 10f;
				if (rect.Contains(current.mousePosition))
				{
					GUIUtility.hotControl = controlID;
					current.Use();
					if (swatches.Contains(this.m_SelectedSwatch) && !this.m_SelectedSwatch.m_IsAlpha && this.CalcSwatchRect(position, this.m_SelectedSwatch).Contains(current.mousePosition))
					{
						if (current.clickCount == 2)
						{
							GUIUtility.keyboardControl = controlID;
							ColorPicker.Show(GUIView.current, this.m_SelectedSwatch.m_Value, false, false, null);
							GUIUtility.ExitGUI();
						}
					}
					else
					{
						bool flag = false;
						foreach (GradientEditor.Swatch current2 in swatches)
						{
							if (this.CalcSwatchRect(position, current2).Contains(point))
							{
								flag = true;
								this.m_SelectedSwatch = current2;
								break;
							}
						}
						if (!flag)
						{
							if (swatches.Count < 8)
							{
								Color value = this.m_Gradient.Evaluate(time);
								if (isAlpha)
								{
									value = new Color(value.a, value.a, value.a, 1f);
								}
								else
								{
									value.a = 1f;
								}
								this.m_SelectedSwatch = new GradientEditor.Swatch(time, value, isAlpha);
								swatches.Add(this.m_SelectedSwatch);
								this.AssignBack();
							}
							else
							{
								Debug.LogWarning(string.Concat(new object[]
								{
									"Max ",
									8,
									" color keys and ",
									8,
									" alpha keys are allowed in a gradient."
								}));
							}
						}
					}
				}
				return;
			}
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					current.Use();
					if (!swatches.Contains(this.m_SelectedSwatch))
					{
						this.m_SelectedSwatch = null;
					}
					this.RemoveDuplicateOverlappingSwatches();
				}
				return;
			case EventType.MouseMove:
			case EventType.KeyUp:
			case EventType.ScrollWheel:
				IL_9B:
				if (typeForControl == EventType.ValidateCommand)
				{
					if (current.commandName == "Delete")
					{
						Event.current.Use();
					}
					return;
				}
				if (typeForControl != EventType.ExecuteCommand)
				{
					return;
				}
				if (current.commandName == "ColorPickerChanged")
				{
					GUI.changed = true;
					this.m_SelectedSwatch.m_Value = ColorPicker.color;
					this.AssignBack();
					HandleUtility.Repaint();
				}
				else if (current.commandName == "Delete" && swatches.Count > 1)
				{
					swatches.Remove(this.m_SelectedSwatch);
					this.AssignBack();
					HandleUtility.Repaint();
				}
				return;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID && this.m_SelectedSwatch != null)
				{
					current.Use();
					if (current.mousePosition.y + 5f < position.y || current.mousePosition.y - 5f > position.yMax)
					{
						if (swatches.Count > 1)
						{
							swatches.Remove(this.m_SelectedSwatch);
							this.AssignBack();
							return;
						}
					}
					else if (!swatches.Contains(this.m_SelectedSwatch))
					{
						swatches.Add(this.m_SelectedSwatch);
					}
					this.m_SelectedSwatch.m_Time = time;
					this.AssignBack();
				}
				return;
			case EventType.KeyDown:
				if (current.keyCode == KeyCode.Delete)
				{
					if (this.m_SelectedSwatch != null)
					{
						List<GradientEditor.Swatch> list;
						if (this.m_SelectedSwatch.m_IsAlpha)
						{
							list = this.m_AlphaSwatches;
						}
						else
						{
							list = this.m_RGBSwatches;
						}
						if (list.Count > 1)
						{
							list.Remove(this.m_SelectedSwatch);
							this.AssignBack();
							HandleUtility.Repaint();
						}
					}
					current.Use();
				}
				return;
			case EventType.Repaint:
			{
				bool flag2 = false;
				foreach (GradientEditor.Swatch current3 in swatches)
				{
					if (this.m_SelectedSwatch == current3)
					{
						flag2 = true;
					}
					else
					{
						this.DrawSwatch(position, current3, !isAlpha);
					}
				}
				if (flag2 && this.m_SelectedSwatch != null)
				{
					this.DrawSwatch(position, this.m_SelectedSwatch, !isAlpha);
				}
				return;
			}
			}
			goto IL_9B;
		}

		private void DrawSwatch(Rect totalPos, GradientEditor.Swatch s, bool upwards)
		{
			Color backgroundColor = GUI.backgroundColor;
			Rect position = this.CalcSwatchRect(totalPos, s);
			GUI.backgroundColor = s.m_Value;
			GUIStyle gUIStyle = (!upwards) ? GradientEditor.s_Styles.downSwatch : GradientEditor.s_Styles.upSwatch;
			GUIStyle gUIStyle2 = (!upwards) ? GradientEditor.s_Styles.downSwatchOverlay : GradientEditor.s_Styles.upSwatchOverlay;
			gUIStyle.Draw(position, false, false, this.m_SelectedSwatch == s, false);
			GUI.backgroundColor = backgroundColor;
			gUIStyle2.Draw(position, false, false, this.m_SelectedSwatch == s, false);
		}

		private Rect CalcSwatchRect(Rect totalRect, GradientEditor.Swatch s)
		{
			float time = s.m_Time;
			return new Rect(totalRect.x + Mathf.Round(totalRect.width * time) - 5f, totalRect.y, 10f, totalRect.height);
		}

		private int SwatchSort(GradientEditor.Swatch lhs, GradientEditor.Swatch rhs)
		{
			if (lhs.m_Time == rhs.m_Time && lhs == this.m_SelectedSwatch)
			{
				return -1;
			}
			if (lhs.m_Time == rhs.m_Time && rhs == this.m_SelectedSwatch)
			{
				return 1;
			}
			return lhs.m_Time.CompareTo(rhs.m_Time);
		}

		private void AssignBack()
		{
			this.m_RGBSwatches.Sort((GradientEditor.Swatch a, GradientEditor.Swatch b) => this.SwatchSort(a, b));
			GradientColorKey[] array = new GradientColorKey[this.m_RGBSwatches.Count];
			for (int i = 0; i < this.m_RGBSwatches.Count; i++)
			{
				array[i].color = this.m_RGBSwatches[i].m_Value;
				array[i].time = this.m_RGBSwatches[i].m_Time;
			}
			this.m_AlphaSwatches.Sort((GradientEditor.Swatch a, GradientEditor.Swatch b) => this.SwatchSort(a, b));
			GradientAlphaKey[] array2 = new GradientAlphaKey[this.m_AlphaSwatches.Count];
			for (int j = 0; j < this.m_AlphaSwatches.Count; j++)
			{
				array2[j].alpha = this.m_AlphaSwatches[j].m_Value.r;
				array2[j].time = this.m_AlphaSwatches[j].m_Time;
			}
			this.m_Gradient.colorKeys = array;
			this.m_Gradient.alphaKeys = array2;
			GUI.changed = true;
		}

		private void RemoveDuplicateOverlappingSwatches()
		{
			bool flag = false;
			for (int i = 1; i < this.m_RGBSwatches.Count; i++)
			{
				if (Mathf.Approximately(this.m_RGBSwatches[i - 1].m_Time, this.m_RGBSwatches[i].m_Time))
				{
					this.m_RGBSwatches.RemoveAt(i);
					i--;
					flag = true;
				}
			}
			for (int j = 1; j < this.m_AlphaSwatches.Count; j++)
			{
				if (Mathf.Approximately(this.m_AlphaSwatches[j - 1].m_Time, this.m_AlphaSwatches[j].m_Time))
				{
					this.m_AlphaSwatches.RemoveAt(j);
					j--;
					flag = true;
				}
			}
			if (flag)
			{
				this.AssignBack();
			}
		}

		public static Texture2D GetBackgroundTexture()
		{
			if (GradientEditor.s_BackgroundTexture == null)
			{
				GradientEditor.s_BackgroundTexture = GradientEditor.CreateCheckerTexture(32, 4, 4, Color.white, new Color(0.7f, 0.7f, 0.7f));
			}
			return GradientEditor.s_BackgroundTexture;
		}

		public static Texture2D CreateCheckerTexture(int numCols, int numRows, int cellPixelWidth, Color col1, Color col2)
		{
			int num = numRows * cellPixelWidth;
			int num2 = numCols * cellPixelWidth;
			Texture2D texture2D = new Texture2D(num2, num, TextureFormat.ARGB32, false);
			texture2D.hideFlags = HideFlags.HideAndDontSave;
			Color[] array = new Color[num2 * num];
			for (int i = 0; i < numRows; i++)
			{
				for (int j = 0; j < numCols; j++)
				{
					for (int k = 0; k < cellPixelWidth; k++)
					{
						for (int l = 0; l < cellPixelWidth; l++)
						{
							array[(i * cellPixelWidth + k) * num2 + j * cellPixelWidth + l] = (((i + j) % 2 != 0) ? col2 : col1);
						}
					}
				}
			}
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		public static void DrawGradientSwatch(Rect position, Gradient gradient, Color bgColor)
		{
			GradientEditor.DrawGradientSwatchInternal(position, gradient, null, bgColor);
		}

		public static void DrawGradientSwatch(Rect position, SerializedProperty property, Color bgColor)
		{
			GradientEditor.DrawGradientSwatchInternal(position, null, property, bgColor);
		}

		private static void DrawGradientSwatchInternal(Rect position, Gradient gradient, SerializedProperty property, Color bgColor)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			Texture2D backgroundTexture = GradientEditor.GetBackgroundTexture();
			if (backgroundTexture != null)
			{
				Color color = GUI.color;
				GUI.color = bgColor;
				GUIStyle basicTextureStyle = EditorGUIUtility.GetBasicTextureStyle(backgroundTexture);
				basicTextureStyle.Draw(position, false, false, false, false);
				GUI.color = color;
			}
			Texture2D texture2D;
			if (property != null)
			{
				texture2D = GradientPreviewCache.GetPropertyPreview(property);
			}
			else
			{
				texture2D = GradientPreviewCache.GetGradientPreview(gradient);
			}
			if (texture2D == null)
			{
				Debug.Log("Warning: Could not create preview for gradient");
				return;
			}
			GUIStyle basicTextureStyle2 = EditorGUIUtility.GetBasicTextureStyle(texture2D);
			basicTextureStyle2.Draw(position, false, false, false, false);
		}
	}
}
