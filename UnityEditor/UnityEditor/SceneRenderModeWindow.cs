using System;
using UnityEngine;
namespace UnityEditor
{
	internal class SceneRenderModeWindow : PopupWindowContent
	{
		private class Styles
		{
			public static readonly GUIStyle sMenuItem = "MenuItem";
			public static readonly GUIStyle sSeparator = "sv_iconselector_sep";
			public static readonly GUIContent sShadedHeader = EditorGUIUtility.TextContent("SceneRenderMode.Headers.ShadingMode");
			public static readonly GUIContent sMiscellaneous = EditorGUIUtility.TextContent("SceneRenderMode.Headers.Miscellaneous");
			public static readonly GUIContent sDeferredHeader = EditorGUIUtility.TextContent("SceneRenderMode.Headers.Deferred");
			public static readonly GUIContent sGlobalIlluminationHeader = EditorGUIUtility.TextContent("SceneRenderMode.Headers.GlobalIllumination");
			public static readonly GUIContent sResolutionToggle = EditorGUIUtility.TextContent("SceneRenderMode.Headers.ResolutionToggle");
			public static readonly GUIContent[] sRenderModeOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("SceneRenderMode.Options.Shaded"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.Wireframe"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.ShadedWireframe"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.ShadowCascades"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.RenderPaths"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.AlphaChannel"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.Overdraw"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.Mipmaps"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.DeferredAlbedo"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.DeferredSpecular"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.DeferredSmoothness"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.DeferredNormal"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.Charting"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.Systems"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.Albedo"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.Emissive"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.Irradiance"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.Directionality"),
				EditorGUIUtility.TextContent("SceneRenderMode.Options.Baked"),
				new GUIContent("Clustering [internal]"),
				new GUIContent("Lit Clustering [internal]")
			};
		}
		private const float m_WindowWidth = 205f;
		private const int kMenuHeaderCount = 4;
		private const int kNumInternalRenderModes = 2;
		private const float kSeparatorHeight = 3f;
		private const float kFrameWidth = 1f;
		private const float kHeaderHorizontalPadding = 5f;
		private const float kHeaderVerticalPadding = 1f;
		private const float kShowLightmapResolutionHeight = 22f;
		private const float kTogglePadding = 7f;
		private readonly float m_WindowHeight = (float)SceneRenderModeWindow.sMenuRowCount * 16f + 9f + 22f;
		private static readonly int sRenderModeCount = (!Unsupported.IsDeveloperBuild()) ? (SceneRenderModeWindow.Styles.sRenderModeOptions.Length - 2) : SceneRenderModeWindow.Styles.sRenderModeOptions.Length;
		private static readonly int sMenuRowCount = SceneRenderModeWindow.sRenderModeCount + 4;
		private SerializedProperty m_EnableRealtimeGI;
		private SerializedProperty m_EnableBakedGI;
		private readonly SceneView m_SceneView;
		public SceneRenderModeWindow(SceneView sceneView)
		{
			this.m_SceneView = sceneView;
		}
		public override Vector2 GetWindowSize()
		{
			return new Vector2(205f, this.m_WindowHeight);
		}
		public override void OnOpen()
		{
			SerializedObject serializedObject = new SerializedObject(LightmapEditorSettings.GetLightmapSettings());
			this.m_EnableRealtimeGI = serializedObject.FindProperty("m_GISettings.m_EnableRealtimeLightmaps");
			this.m_EnableBakedGI = serializedObject.FindProperty("m_GISettings.m_EnableBakedLightmaps");
		}
		public override void OnGUI(Rect rect)
		{
			if (this.m_SceneView == null || this.m_SceneView.m_SceneViewState == null)
			{
				return;
			}
			if (Event.current.type == EventType.Layout)
			{
				return;
			}
			this.Draw(base.editorWindow, rect.width);
			if (Event.current.type == EventType.MouseMove)
			{
				Event.current.Use();
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
			{
				base.editorWindow.Close();
				GUIUtility.ExitGUI();
			}
		}
		private void DrawSeparator(ref Rect rect)
		{
			Rect position = rect;
			position.x += 5f;
			position.y += 3f;
			position.width -= 10f;
			position.height = 3f;
			GUI.Label(position, GUIContent.none, SceneRenderModeWindow.Styles.sSeparator);
			rect.y += 3f;
		}
		private void DrawHeader(ref Rect rect, GUIContent label)
		{
			Rect position = rect;
			position.y += 1f;
			position.x += 5f;
			position.width = EditorStyles.miniLabel.CalcSize(label).x;
			position.height = EditorStyles.miniLabel.CalcSize(label).y;
			GUI.Label(position, label, EditorStyles.miniLabel);
			rect.y += 16f;
		}
		private void Draw(EditorWindow caller, float listElementWidth)
		{
			Rect rect = new Rect(0f, 0f, listElementWidth, 16f);
			this.DrawHeader(ref rect, SceneRenderModeWindow.Styles.sShadedHeader);
			for (int i = 0; i < SceneRenderModeWindow.sRenderModeCount; i++)
			{
				DrawCameraMode drawCameraMode = (DrawCameraMode)i;
				DrawCameraMode drawCameraMode2 = drawCameraMode;
				if (drawCameraMode2 != DrawCameraMode.ShadowCascades)
				{
					if (drawCameraMode2 != DrawCameraMode.DeferredDiffuse)
					{
						if (drawCameraMode2 == DrawCameraMode.Charting)
						{
							this.DrawSeparator(ref rect);
							this.DrawHeader(ref rect, SceneRenderModeWindow.Styles.sGlobalIlluminationHeader);
						}
					}
					else
					{
						this.DrawSeparator(ref rect);
						this.DrawHeader(ref rect, SceneRenderModeWindow.Styles.sDeferredHeader);
					}
				}
				else
				{
					this.DrawSeparator(ref rect);
					this.DrawHeader(ref rect, SceneRenderModeWindow.Styles.sMiscellaneous);
				}
				EditorGUI.BeginDisabledGroup(this.IsModeDisabled(drawCameraMode));
				this.DoOneMode(caller, ref rect, drawCameraMode);
				EditorGUI.EndDisabledGroup();
			}
			bool disabled = this.m_SceneView.renderMode < DrawCameraMode.Charting || this.IsModeDisabled(this.m_SceneView.renderMode);
			this.DoResolutionToggle(rect, disabled);
		}
		private bool IsModeDisabled(DrawCameraMode mode)
		{
			return (!this.m_EnableBakedGI.boolValue && mode == DrawCameraMode.Baked) || (!this.m_EnableRealtimeGI.boolValue && !this.m_EnableBakedGI.boolValue && mode >= DrawCameraMode.Charting);
		}
		private void DoResolutionToggle(Rect rect, bool disabled)
		{
			GUI.Label(new Rect(1f, rect.y, 203f, 22f), string.Empty, EditorStyles.inspectorBig);
			rect.y += 3f;
			rect.x += 7f;
			EditorGUI.BeginDisabledGroup(disabled);
			EditorGUI.BeginChangeCheck();
			bool showResolution = GUI.Toggle(rect, LightmapVisualization.showResolution, SceneRenderModeWindow.Styles.sResolutionToggle);
			if (EditorGUI.EndChangeCheck())
			{
				LightmapVisualization.showResolution = showResolution;
				SceneView.RepaintAll();
			}
			EditorGUI.EndDisabledGroup();
		}
		private void DoOneMode(EditorWindow caller, ref Rect rect, DrawCameraMode drawCameraMode)
		{
			EditorGUI.BeginDisabledGroup(!this.m_SceneView.CheckDrawModeForRenderingPath(drawCameraMode));
			EditorGUI.BeginChangeCheck();
			GUI.Toggle(rect, this.m_SceneView.renderMode == drawCameraMode, SceneRenderModeWindow.GetGUIContent(drawCameraMode), SceneRenderModeWindow.Styles.sMenuItem);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_SceneView.renderMode = drawCameraMode;
				this.m_SceneView.Repaint();
				GUIUtility.ExitGUI();
			}
			rect.y += 16f;
			EditorGUI.EndDisabledGroup();
		}
		public static GUIContent GetGUIContent(DrawCameraMode drawCameraMode)
		{
			return SceneRenderModeWindow.Styles.sRenderModeOptions[(int)drawCameraMode];
		}
	}
}
