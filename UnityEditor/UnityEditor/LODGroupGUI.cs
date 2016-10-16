using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal static class LODGroupGUI
	{
		public class GUIStyles
		{
			public readonly GUIStyle m_LODSliderBG = "LODSliderBG";

			public readonly GUIStyle m_LODSliderRange = "LODSliderRange";

			public readonly GUIStyle m_LODSliderRangeSelected = "LODSliderRangeSelected";

			public readonly GUIStyle m_LODSliderText = "LODSliderText";

			public readonly GUIStyle m_LODSliderTextSelected = "LODSliderTextSelected";

			public readonly GUIStyle m_LODStandardButton = "Button";

			public readonly GUIStyle m_LODRendererButton = "LODRendererButton";

			public readonly GUIStyle m_LODRendererAddButton = "LODRendererAddButton";

			public readonly GUIStyle m_LODRendererRemove = "LODRendererRemove";

			public readonly GUIStyle m_LODBlackBox = "LODBlackBox";

			public readonly GUIStyle m_LODCameraLine = "LODCameraLine";

			public readonly GUIStyle m_LODSceneText = "LODSceneText";

			public readonly GUIStyle m_LODRenderersText = "LODRenderersText";

			public readonly GUIStyle m_LODLevelNotifyText = "LODLevelNotifyText";

			public readonly GUIContent m_IconRendererPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add New Renderers");

			public readonly GUIContent m_IconRendererMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove Renderer");

			public readonly GUIContent m_CameraIcon = EditorGUIUtility.IconContent("Camera Icon");

			public readonly GUIContent m_UploadToImporter = EditorGUIUtility.TextContent("Upload to Importer|Upload the modified screen percentages to the model importer.");

			public readonly GUIContent m_UploadToImporterDisabled = EditorGUIUtility.TextContent("Upload to Importer|Number of LOD's in the scene instance differ from the number of LOD's in the imported model.");

			public readonly GUIContent m_RecalculateBounds = EditorGUIUtility.TextContent("Bounds|Recalculate bounds for the current LOD group.");

			public readonly GUIContent m_LightmapScale = EditorGUIUtility.TextContent("Lightmap Scale|Set the lightmap scale to match the LOD percentages.");

			public readonly GUIContent m_RendersTitle = EditorGUIUtility.TextContent("Renderers:");

			public readonly GUIContent m_AnimatedCrossFadeInvalidText = EditorGUIUtility.TextContent("Animated cross-fading is currently disabled. Please enable \"Animate Between Next LOD\" on either the current or the previous LOD.");

			public readonly GUIContent m_AnimatedCrossFadeInconsistentText = EditorGUIUtility.TextContent("Animated cross-fading is currently disabled. \"Animate Between Next LOD\" is enabled but the next LOD is not in Animated Cross Fade mode.");

			public readonly GUIContent m_AnimateBetweenPreviousLOD = EditorGUIUtility.TextContent("Animate Between Previous LOD|Cross-fade animation plays when transits between this LOD and the previous (lower) LOD.");
		}

		public class LODInfo
		{
			public Rect m_ButtonPosition;

			public Rect m_RangePosition;

			public int LODLevel
			{
				get;
				private set;
			}

			public string LODName
			{
				get;
				private set;
			}

			public float RawScreenPercent
			{
				get;
				set;
			}

			public float ScreenPercent
			{
				get
				{
					return LODGroupGUI.DelinearizeScreenPercentage(this.RawScreenPercent);
				}
				set
				{
					this.RawScreenPercent = LODGroupGUI.LinearizeScreenPercentage(value);
				}
			}

			public LODInfo(int lodLevel, string name, float screenPercentage)
			{
				this.LODLevel = lodLevel;
				this.LODName = name;
				this.RawScreenPercent = screenPercentage;
			}
		}

		public const int kSceneLabelHalfWidth = 100;

		public const int kSceneLabelHeight = 45;

		public const int kSceneHeaderOffset = 40;

		public const int kSliderBarTopMargin = 18;

		public const int kSliderBarHeight = 30;

		public const int kSliderBarBottomMargin = 16;

		public const int kRenderersButtonHeight = 60;

		public const int kButtonPadding = 2;

		public const int kDeleteButtonSize = 20;

		public const int kSelectedLODRangePadding = 3;

		public const int kRenderAreaForegroundPadding = 3;

		public static readonly Color[] kLODColors = new Color[]
		{
			new Color(0.4831376f, 0.6211768f, 0.0219608f, 1f),
			new Color(0.279216f, 0.4078432f, 0.5835296f, 1f),
			new Color(0.2070592f, 0.5333336f, 0.6556864f, 1f),
			new Color(0.5333336f, 0.16f, 0.0282352f, 1f),
			new Color(0.3827448f, 0.2886272f, 0.5239216f, 1f),
			new Color(0.8f, 0.4423528f, 0f, 1f),
			new Color(0.4486272f, 0.4078432f, 0.050196f, 1f),
			new Color(0.7749016f, 0.6368624f, 0.0250984f, 1f)
		};

		public static readonly Color kCulledLODColor = new Color(0.4f, 0f, 0f, 1f);

		private static LODGroupGUI.GUIStyles s_Styles;

		public static LODGroupGUI.GUIStyles Styles
		{
			get
			{
				if (LODGroupGUI.s_Styles == null)
				{
					LODGroupGUI.s_Styles = new LODGroupGUI.GUIStyles();
				}
				return LODGroupGUI.s_Styles;
			}
		}

		public static float DelinearizeScreenPercentage(float percentage)
		{
			if (Mathf.Approximately(0f, percentage))
			{
				return 0f;
			}
			return Mathf.Sqrt(percentage);
		}

		public static float LinearizeScreenPercentage(float percentage)
		{
			return percentage * percentage;
		}

		public static Rect CalcLODButton(Rect totalRect, float percentage)
		{
			return new Rect(totalRect.x + Mathf.Round(totalRect.width * (1f - percentage)) - 5f, totalRect.y, 10f, totalRect.height);
		}

		public static Rect GetCulledBox(Rect totalRect, float previousLODPercentage)
		{
			Rect result = LODGroupGUI.CalcLODRange(totalRect, previousLODPercentage, 0f);
			result.height -= 2f;
			result.width -= 1f;
			result.center += new Vector2(0f, 1f);
			return result;
		}

		public static List<LODGroupGUI.LODInfo> CreateLODInfos(int numLODs, Rect area, Func<int, string> nameGen, Func<int, float> heightGen)
		{
			List<LODGroupGUI.LODInfo> list = new List<LODGroupGUI.LODInfo>();
			for (int i = 0; i < numLODs; i++)
			{
				LODGroupGUI.LODInfo lODInfo = new LODGroupGUI.LODInfo(i, nameGen(i), heightGen(i));
				lODInfo.m_ButtonPosition = LODGroupGUI.CalcLODButton(area, lODInfo.ScreenPercent);
				float startPercent = (i != 0) ? list[i - 1].ScreenPercent : 1f;
				lODInfo.m_RangePosition = LODGroupGUI.CalcLODRange(area, startPercent, lODInfo.ScreenPercent);
				list.Add(lODInfo);
			}
			return list;
		}

		public static void SetSelectedLODLevelPercentage(float newScreenPercentage, int lod, List<LODGroupGUI.LODInfo> lods)
		{
			float num = 0f;
			LODGroupGUI.LODInfo lODInfo = lods.FirstOrDefault((LODGroupGUI.LODInfo x) => x.LODLevel == lods[lod].LODLevel + 1);
			if (lODInfo != null)
			{
				num = lODInfo.ScreenPercent;
			}
			float num2 = 1f;
			LODGroupGUI.LODInfo lODInfo2 = lods.FirstOrDefault((LODGroupGUI.LODInfo x) => x.LODLevel == lods[lod].LODLevel - 1);
			if (lODInfo2 != null)
			{
				num2 = lODInfo2.ScreenPercent;
			}
			num2 = Mathf.Clamp01(num2);
			num = Mathf.Clamp01(num);
			lods[lod].ScreenPercent = Mathf.Clamp(newScreenPercentage, num, num2);
		}

		public static void DrawLODSlider(Rect area, IList<LODGroupGUI.LODInfo> lods, int selectedLevel)
		{
			LODGroupGUI.Styles.m_LODSliderBG.Draw(area, GUIContent.none, false, false, false, false);
			for (int i = 0; i < lods.Count; i++)
			{
				LODGroupGUI.LODInfo currentLOD = lods[i];
				LODGroupGUI.DrawLODRange(currentLOD, (i != 0) ? lods[i - 1].RawScreenPercent : 1f, i == selectedLevel);
				LODGroupGUI.DrawLODButton(currentLOD);
			}
			LODGroupGUI.DrawCulledRange(area, (lods.Count <= 0) ? 1f : lods[lods.Count - 1].RawScreenPercent);
		}

		public static void DrawMixedValueLODSlider(Rect area)
		{
			LODGroupGUI.Styles.m_LODSliderBG.Draw(area, GUIContent.none, false, false, false, false);
			Rect culledBox = LODGroupGUI.GetCulledBox(area, 1f);
			Color color = GUI.color;
			GUI.color = LODGroupGUI.kLODColors[1] * 0.6f;
			LODGroupGUI.Styles.m_LODSliderRange.Draw(culledBox, GUIContent.none, false, false, false, false);
			GUI.color = color;
			GUIStyle style = new GUIStyle(EditorStyles.whiteLargeLabel)
			{
				alignment = TextAnchor.MiddleCenter
			};
			GUI.Label(area, "---", style);
		}

		private static Rect CalcLODRange(Rect totalRect, float startPercent, float endPercent)
		{
			float num = Mathf.Round(totalRect.width * (1f - startPercent));
			float num2 = Mathf.Round(totalRect.width * (1f - endPercent));
			return new Rect(totalRect.x + num, totalRect.y, num2 - num, totalRect.height);
		}

		private static void DrawLODButton(LODGroupGUI.LODInfo currentLOD)
		{
			EditorGUIUtility.AddCursorRect(currentLOD.m_ButtonPosition, MouseCursor.ResizeHorizontal);
		}

		private static void DrawLODRange(LODGroupGUI.LODInfo currentLOD, float previousLODPercentage, bool isSelected)
		{
			Color backgroundColor = GUI.backgroundColor;
			string text = string.Format("{0}\n{1:0}%", currentLOD.LODName, previousLODPercentage * 100f);
			if (isSelected)
			{
				Rect rangePosition = currentLOD.m_RangePosition;
				rangePosition.width -= 6f;
				rangePosition.height -= 6f;
				rangePosition.center += new Vector2(3f, 3f);
				LODGroupGUI.Styles.m_LODSliderRangeSelected.Draw(currentLOD.m_RangePosition, GUIContent.none, false, false, false, false);
				GUI.backgroundColor = LODGroupGUI.kLODColors[currentLOD.LODLevel];
				if (rangePosition.width > 0f)
				{
					LODGroupGUI.Styles.m_LODSliderRange.Draw(rangePosition, GUIContent.none, false, false, false, false);
				}
				LODGroupGUI.Styles.m_LODSliderText.Draw(currentLOD.m_RangePosition, text, false, false, false, false);
			}
			else
			{
				GUI.backgroundColor = LODGroupGUI.kLODColors[currentLOD.LODLevel];
				GUI.backgroundColor *= 0.6f;
				LODGroupGUI.Styles.m_LODSliderRange.Draw(currentLOD.m_RangePosition, GUIContent.none, false, false, false, false);
				LODGroupGUI.Styles.m_LODSliderText.Draw(currentLOD.m_RangePosition, text, false, false, false, false);
			}
			GUI.backgroundColor = backgroundColor;
		}

		private static void DrawCulledRange(Rect totalRect, float previousLODPercentage)
		{
			if (Mathf.Approximately(previousLODPercentage, 0f))
			{
				return;
			}
			Rect culledBox = LODGroupGUI.GetCulledBox(totalRect, LODGroupGUI.DelinearizeScreenPercentage(previousLODPercentage));
			Color color = GUI.color;
			GUI.color = LODGroupGUI.kCulledLODColor;
			LODGroupGUI.Styles.m_LODSliderRange.Draw(culledBox, GUIContent.none, false, false, false, false);
			GUI.color = color;
			string text = string.Format("Culled\n{0:0}%", previousLODPercentage * 100f);
			LODGroupGUI.Styles.m_LODSliderText.Draw(culledBox, text, false, false, false, false);
		}
	}
}
