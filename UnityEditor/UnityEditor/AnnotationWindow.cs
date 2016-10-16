using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AnnotationWindow : EditorWindow
	{
		private class Styles
		{
			public GUIStyle toolbar = "toolbar";

			public GUIStyle toggle = "OL Toggle";

			public GUIStyle listEvenBg = "ObjectPickerResultsOdd";

			public GUIStyle listOddBg = "ObjectPickerResultsEven";

			public GUIStyle listSectionHeaderBg = "ObjectPickerResultsEven";

			public GUIStyle background = "grey_border";

			public GUIStyle seperator = "sv_iconselector_sep";

			public GUIStyle iconDropDown = "IN dropdown";

			public GUIStyle listTextStyle;

			public GUIStyle listHeaderStyle;

			public GUIStyle texelWorldSizeStyle;

			public GUIStyle columnHeaderStyle;

			public Styles()
			{
				this.listTextStyle = new GUIStyle(EditorStyles.label);
				this.listTextStyle.alignment = TextAnchor.MiddleLeft;
				this.listTextStyle.padding.left = 10;
				this.listHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
				this.listHeaderStyle.padding.left = 5;
				this.texelWorldSizeStyle = new GUIStyle(EditorStyles.label);
				this.texelWorldSizeStyle.alignment = TextAnchor.UpperRight;
				this.texelWorldSizeStyle.font = EditorStyles.miniLabel.font;
				this.texelWorldSizeStyle.fontSize = EditorStyles.miniLabel.fontSize;
				this.texelWorldSizeStyle.padding.right = 0;
				this.columnHeaderStyle = new GUIStyle(EditorStyles.miniLabel);
			}
		}

		private const float kWindowWidth = 270f;

		private const float scrollBarWidth = 14f;

		private const float listElementHeight = 18f;

		private const float gizmoRightAlign = 23f;

		private const float iconRightAlign = 64f;

		private const float frameWidth = 1f;

		private const float k_AnimDuration = 0.4f;

		private const int maxShowRecent = 5;

		private const string textGizmoVisible = "Show/Hide Gizmo";

		private const float exponentStart = -3f;

		private const float exponentRange = 3f;

		private const string kAlwaysFullSizeText = "Always Full Size";

		private const string kHideAllIconsText = "Hide All Icons";

		private static bool s_Debug;

		private static AnnotationWindow s_AnnotationWindow;

		private static long s_LastClosedTime;

		private static AnnotationWindow.Styles m_Styles;

		private List<AInfo> m_RecentAnnotations;

		private List<AInfo> m_BuiltinAnnotations;

		private List<AInfo> m_ScriptAnnotations;

		private Vector2 m_ScrollPosition;

		private bool m_SyncWithState;

		private string m_LastScriptThatHasShownTheIconSelector;

		private List<MonoScript> m_MonoScriptIconsChanged;

		private GUIContent iconToggleContent = new GUIContent(string.Empty, "Show/Hide Icon");

		private GUIContent iconSelectContent = new GUIContent(string.Empty, "Select Icon");

		private GUIContent icon3dGizmoContent = new GUIContent("3D Icons");

		private GUIContent showGridContent = new GUIContent("Show Grid");

		private bool m_IsGameView;

		private static float ConvertTexelWorldSizeTo01(float texelWorldSize)
		{
			if (texelWorldSize == -1f)
			{
				return 1f;
			}
			if (texelWorldSize == 0f)
			{
				return 0f;
			}
			return (Mathf.Log10(texelWorldSize) - -3f) / 3f;
		}

		private static float Convert01ToTexelWorldSize(float value01)
		{
			if (value01 <= 0f)
			{
				return 0f;
			}
			return Mathf.Pow(10f, -3f + 3f * value01);
		}

		private static string ConvertTexelWorldSizeToString(float texelWorldSize)
		{
			if (texelWorldSize == -1f)
			{
				return "Always Full Size";
			}
			if (texelWorldSize == 0f)
			{
				return "Hide All Icons";
			}
			float num = texelWorldSize * 32f;
			int numberOfDecimalsForMinimumDifference = MathUtils.GetNumberOfDecimalsForMinimumDifference(num * 0.1f);
			return num.ToString("N" + numberOfDecimalsForMinimumDifference);
		}

		public void MonoScriptIconChanged(MonoScript monoScript)
		{
			if (monoScript == null)
			{
				return;
			}
			bool flag = true;
			foreach (MonoScript current in this.m_MonoScriptIconsChanged)
			{
				if (current.GetInstanceID() == monoScript.GetInstanceID())
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.m_MonoScriptIconsChanged.Add(monoScript);
			}
		}

		public static void IconChanged()
		{
			if (AnnotationWindow.s_AnnotationWindow != null)
			{
				AnnotationWindow.s_AnnotationWindow.IconHasChanged();
			}
		}

		private float GetTopSectionHeight()
		{
			return 50f;
		}

		private void OnEnable()
		{
			base.hideFlags = HideFlags.DontSave;
		}

		private void OnDisable()
		{
			foreach (MonoScript current in this.m_MonoScriptIconsChanged)
			{
				MonoImporter.CopyMonoScriptIconToImporters(current);
			}
			AnnotationWindow.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
			AnnotationWindow.s_AnnotationWindow = null;
		}

		internal static bool ShowAtPosition(Rect buttonRect, bool isGameView)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num >= AnnotationWindow.s_LastClosedTime + 50L)
			{
				Event.current.Use();
				if (AnnotationWindow.s_AnnotationWindow == null)
				{
					AnnotationWindow.s_AnnotationWindow = ScriptableObject.CreateInstance<AnnotationWindow>();
				}
				AnnotationWindow.s_AnnotationWindow.Init(buttonRect, isGameView);
				return true;
			}
			return false;
		}

		private void Init(Rect buttonRect, bool isGameView)
		{
			buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
			this.m_MonoScriptIconsChanged = new List<MonoScript>();
			this.m_SyncWithState = true;
			this.m_IsGameView = isGameView;
			this.SyncToState();
			float num = 2f + this.GetTopSectionHeight() + this.DrawNormalList(false, 100f, 0f, 10000f);
			num = Mathf.Min(num, 900f);
			Vector2 windowSize = new Vector2(270f, num);
			base.ShowAsDropDown(buttonRect, windowSize);
		}

		private void IconHasChanged()
		{
			if (string.IsNullOrEmpty(this.m_LastScriptThatHasShownTheIconSelector))
			{
				return;
			}
			foreach (AInfo current in this.m_ScriptAnnotations)
			{
				if (current.m_ScriptClass == this.m_LastScriptThatHasShownTheIconSelector && !current.m_IconEnabled)
				{
					current.m_IconEnabled = true;
					this.SetIconState(current);
					break;
				}
			}
			base.Repaint();
		}

		private void Cancel()
		{
			base.Close();
			GUI.changed = true;
			GUIUtility.ExitGUI();
		}

		private AInfo GetAInfo(int classID, string scriptClass)
		{
			if (scriptClass != string.Empty)
			{
				return this.m_ScriptAnnotations.Find((AInfo o) => o.m_ScriptClass == scriptClass);
			}
			return this.m_BuiltinAnnotations.Find((AInfo o) => o.m_ClassID == classID);
		}

		private void SyncToState()
		{
			Annotation[] annotations = AnnotationUtility.GetAnnotations();
			string text = string.Empty;
			if (AnnotationWindow.s_Debug)
			{
				text += "AnnotationWindow: SyncToState\n";
			}
			this.m_BuiltinAnnotations = new List<AInfo>();
			this.m_ScriptAnnotations = new List<AInfo>();
			for (int i = 0; i < annotations.Length; i++)
			{
				if (AnnotationWindow.s_Debug)
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						"   same as below: icon ",
						annotations[i].iconEnabled,
						" gizmo ",
						annotations[i].gizmoEnabled,
						"\n"
					});
				}
				bool gizmoEnabled = annotations[i].gizmoEnabled == 1;
				bool iconEnabled = annotations[i].iconEnabled == 1;
				AInfo aInfo = new AInfo(gizmoEnabled, iconEnabled, annotations[i].flags, annotations[i].classID, annotations[i].scriptClass);
				if (aInfo.m_ScriptClass == string.Empty)
				{
					this.m_BuiltinAnnotations.Add(aInfo);
					if (AnnotationWindow.s_Debug)
					{
						string text2 = text;
						text = string.Concat(new object[]
						{
							text2,
							"   ",
							BaseObjectTools.ClassIDToString(aInfo.m_ClassID),
							": icon ",
							aInfo.m_IconEnabled,
							" gizmo ",
							aInfo.m_GizmoEnabled,
							"\n"
						});
					}
				}
				else
				{
					this.m_ScriptAnnotations.Add(aInfo);
					if (AnnotationWindow.s_Debug)
					{
						string text2 = text;
						text = string.Concat(new object[]
						{
							text2,
							"   ",
							annotations[i].scriptClass,
							": icon ",
							aInfo.m_IconEnabled,
							" gizmo ",
							aInfo.m_GizmoEnabled,
							"\n"
						});
					}
				}
			}
			this.m_BuiltinAnnotations.Sort();
			this.m_ScriptAnnotations.Sort();
			this.m_RecentAnnotations = new List<AInfo>();
			Annotation[] recentlyChangedAnnotations = AnnotationUtility.GetRecentlyChangedAnnotations();
			int num = 0;
			while (num < recentlyChangedAnnotations.Length && num < 5)
			{
				AInfo aInfo2 = this.GetAInfo(recentlyChangedAnnotations[num].classID, recentlyChangedAnnotations[num].scriptClass);
				if (aInfo2 != null)
				{
					this.m_RecentAnnotations.Add(aInfo2);
				}
				num++;
			}
			this.m_SyncWithState = false;
			if (AnnotationWindow.s_Debug)
			{
				Debug.Log(text);
			}
		}

		internal void OnGUI()
		{
			if (Event.current.type == EventType.Layout)
			{
				return;
			}
			if (AnnotationWindow.m_Styles == null)
			{
				AnnotationWindow.m_Styles = new AnnotationWindow.Styles();
			}
			if (this.m_SyncWithState)
			{
				this.SyncToState();
			}
			float topSectionHeight = this.GetTopSectionHeight();
			this.DrawTopSection(topSectionHeight);
			this.DrawAnnotationList(topSectionHeight, base.position.height - topSectionHeight);
			GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, AnnotationWindow.m_Styles.background);
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
			{
				this.Cancel();
			}
		}

		private void DrawTopSection(float topSectionHeight)
		{
			GUI.Label(new Rect(1f, 0f, base.position.width - 2f, topSectionHeight), string.Empty, EditorStyles.inspectorBig);
			float num = 7f;
			float num2 = 11f;
			float num3 = num;
			Rect position = new Rect(num2 - 2f, num3, 80f, 20f);
			AnnotationUtility.use3dGizmos = GUI.Toggle(position, AnnotationUtility.use3dGizmos, this.icon3dGizmoContent);
			float iconSize = AnnotationUtility.iconSize;
			if (AnnotationWindow.s_Debug)
			{
				Rect position2 = new Rect(0f, num3 + 10f, base.position.width - num2, 20f);
				GUI.Label(position2, AnnotationWindow.ConvertTexelWorldSizeToString(iconSize), AnnotationWindow.m_Styles.texelWorldSizeStyle);
			}
			using (new EditorGUI.DisabledScope(!AnnotationUtility.use3dGizmos))
			{
				float num4 = 160f;
				float num5 = AnnotationWindow.ConvertTexelWorldSizeTo01(iconSize);
				Rect position3 = new Rect(base.position.width - num2 - num4, num3, num4, 20f);
				num5 = GUI.HorizontalSlider(position3, num5, 0f, 1f);
				if (GUI.changed)
				{
					AnnotationUtility.iconSize = AnnotationWindow.Convert01ToTexelWorldSize(num5);
					SceneView.RepaintAll();
				}
			}
			num3 += 20f;
			using (new EditorGUI.DisabledScope(this.m_IsGameView))
			{
				position = new Rect(num2 - 2f, num3, 80f, 20f);
				AnnotationUtility.showGrid = GUI.Toggle(position, AnnotationUtility.showGrid, this.showGridContent);
			}
		}

		private void DrawAnnotationList(float startY, float height)
		{
			Rect position = new Rect(1f, startY + 1f, base.position.width - 2f, height - 1f - 1f);
			float num = this.DrawNormalList(false, 0f, 0f, 100000f);
			Rect viewRect = new Rect(0f, 0f, 1f, num);
			bool flag = num > position.height;
			float num2 = position.width;
			if (flag)
			{
				num2 -= 14f;
			}
			this.m_ScrollPosition = GUI.BeginScrollView(position, this.m_ScrollPosition, viewRect);
			this.DrawNormalList(true, num2, this.m_ScrollPosition.y - 18f, this.m_ScrollPosition.y + num);
			GUI.EndScrollView();
		}

		private void Flip(ref bool even)
		{
			even = !even;
		}

		private float DrawNormalList(bool doDraw, float listElementWidth, float startY, float endY)
		{
			bool flag = true;
			float y = 0f;
			bool flag2 = false;
			y = this.DrawListSection(y, "Recently Changed", this.m_RecentAnnotations, doDraw, listElementWidth, startY, endY, ref flag, true, ref flag2);
			y = this.DrawListSection(y, "Scripts", this.m_ScriptAnnotations, doDraw, listElementWidth, startY, endY, ref flag, false, ref flag2);
			return this.DrawListSection(y, "Built-in Components", this.m_BuiltinAnnotations, doDraw, listElementWidth, startY, endY, ref flag, false, ref flag2);
		}

		private float DrawListSection(float y, string sectionHeader, List<AInfo> listElements, bool doDraw, float listElementWidth, float startY, float endY, ref bool even, bool useSeperator, ref bool headerDrawn)
		{
			float num = y;
			if (listElements.Count > 0)
			{
				if (doDraw)
				{
					Rect position = new Rect(1f, num, listElementWidth - 2f, 30f);
					this.Flip(ref even);
					GUIStyle style = (!even) ? AnnotationWindow.m_Styles.listOddBg : AnnotationWindow.m_Styles.listEvenBg;
					GUI.Label(position, GUIContent.Temp(string.Empty), style);
				}
				num += 10f;
				if (doDraw)
				{
					this.DrawListHeader(sectionHeader, new Rect(3f, num, listElementWidth, 20f), ref headerDrawn);
				}
				num += 20f;
				for (int i = 0; i < listElements.Count; i++)
				{
					this.Flip(ref even);
					if (num > startY && num < endY)
					{
						Rect rect = new Rect(1f, num, listElementWidth - 2f, 18f);
						if (doDraw)
						{
							this.DrawListElement(rect, even, listElements[i]);
						}
					}
					num += 18f;
				}
				if (useSeperator)
				{
					float num2 = 6f;
					if (doDraw)
					{
						GUIStyle style2 = (!even) ? AnnotationWindow.m_Styles.listOddBg : AnnotationWindow.m_Styles.listEvenBg;
						GUI.Label(new Rect(1f, num, listElementWidth - 2f, num2), GUIContent.Temp(string.Empty), style2);
						GUI.Label(new Rect(10f, num + 3f, listElementWidth - 15f, 3f), GUIContent.Temp(string.Empty), AnnotationWindow.m_Styles.seperator);
					}
					num += num2;
				}
			}
			return num;
		}

		private void DrawListHeader(string header, Rect rect, ref bool headerDrawn)
		{
			GUI.Label(rect, GUIContent.Temp(header), AnnotationWindow.m_Styles.listHeaderStyle);
			if (!headerDrawn)
			{
				headerDrawn = true;
				GUI.color = new Color(1f, 1f, 1f, 0.65f);
				Rect position = rect;
				position.y += -10f;
				position.x = rect.width - 32f;
				GUI.Label(position, "gizmo", AnnotationWindow.m_Styles.columnHeaderStyle);
				position.x = rect.width - 64f;
				GUI.Label(position, "icon", AnnotationWindow.m_Styles.columnHeaderStyle);
				GUI.color = Color.white;
			}
		}

		private void DrawListElement(Rect rect, bool even, AInfo ainfo)
		{
			if (ainfo == null)
			{
				Debug.LogError("DrawListElement: AInfo not valid!");
				return;
			}
			float num = 17f;
			float a = 0.3f;
			bool changed = GUI.changed;
			bool enabled = GUI.enabled;
			Color color = GUI.color;
			GUI.changed = false;
			GUI.enabled = true;
			GUIStyle style = (!even) ? AnnotationWindow.m_Styles.listOddBg : AnnotationWindow.m_Styles.listEvenBg;
			GUI.Label(rect, GUIContent.Temp(string.Empty), style);
			Rect position = rect;
			position.width = rect.width - 64f - 22f;
			GUI.Label(position, ainfo.m_DisplayText, AnnotationWindow.m_Styles.listTextStyle);
			float num2 = 16f;
			Rect rect2 = new Rect(rect.width - 64f, rect.y + (rect.height - num2) * 0.5f, num2, num2);
			Texture texture = null;
			if (ainfo.m_ScriptClass != string.Empty)
			{
				texture = EditorGUIUtility.GetIconForObject(EditorGUIUtility.GetScript(ainfo.m_ScriptClass));
				Rect position2 = rect2;
				position2.x += 18f;
				position2.y += 1f;
				position2.width = 1f;
				position2.height = 12f;
				if (!EditorGUIUtility.isProSkin)
				{
					GUI.color = new Color(0f, 0f, 0f, 0.33f);
				}
				else
				{
					GUI.color = new Color(1f, 1f, 1f, 0.13f);
				}
				GUI.DrawTexture(position2, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
				GUI.color = Color.white;
				Rect rect3 = rect2;
				rect3.x += 18f;
				rect3.y = rect3.y;
				rect3.width = 9f;
				if (GUI.Button(rect3, this.iconSelectContent, AnnotationWindow.m_Styles.iconDropDown))
				{
					UnityEngine.Object script = EditorGUIUtility.GetScript(ainfo.m_ScriptClass);
					if (script != null)
					{
						this.m_LastScriptThatHasShownTheIconSelector = ainfo.m_ScriptClass;
						if (IconSelector.ShowAtPosition(script, rect3, true))
						{
							IconSelector.SetMonoScriptIconChangedCallback(new IconSelector.MonoScriptIconChangedCallback(this.MonoScriptIconChanged));
							GUIUtility.ExitGUI();
						}
					}
				}
			}
			else if (ainfo.HasIcon())
			{
				texture = AssetPreview.GetMiniTypeThumbnailFromClassID(ainfo.m_ClassID);
			}
			if (texture != null)
			{
				if (!ainfo.m_IconEnabled)
				{
					GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a);
					string tooltip = string.Empty;
				}
				this.iconToggleContent.image = texture;
				if (GUI.Button(rect2, this.iconToggleContent, GUIStyle.none))
				{
					ainfo.m_IconEnabled = !ainfo.m_IconEnabled;
					this.SetIconState(ainfo);
				}
				GUI.color = color;
			}
			if (GUI.changed)
			{
				this.SetIconState(ainfo);
				GUI.changed = false;
			}
			GUI.enabled = true;
			GUI.color = color;
			if (ainfo.HasGizmo())
			{
				string tooltip = "Show/Hide Gizmo";
				Rect position3 = new Rect(rect.width - 23f, rect.y + (rect.height - num) * 0.5f, num, num);
				ainfo.m_GizmoEnabled = GUI.Toggle(position3, ainfo.m_GizmoEnabled, new GUIContent(string.Empty, tooltip), AnnotationWindow.m_Styles.toggle);
				if (GUI.changed)
				{
					this.SetGizmoState(ainfo);
				}
			}
			GUI.enabled = enabled;
			GUI.changed = changed;
			GUI.color = color;
		}

		private void SetIconState(AInfo ainfo)
		{
			AnnotationUtility.SetIconEnabled(ainfo.m_ClassID, ainfo.m_ScriptClass, (!ainfo.m_IconEnabled) ? 0 : 1);
			SceneView.RepaintAll();
		}

		private void SetGizmoState(AInfo ainfo)
		{
			AnnotationUtility.SetGizmoEnabled(ainfo.m_ClassID, ainfo.m_ScriptClass, (!ainfo.m_GizmoEnabled) ? 0 : 1);
			SceneView.RepaintAll();
		}
	}
}
