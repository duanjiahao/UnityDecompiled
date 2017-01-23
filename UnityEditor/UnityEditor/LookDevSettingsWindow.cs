using System;
using UnityEngine;

namespace UnityEditor
{
	internal class LookDevSettingsWindow : PopupWindowContent
	{
		public class Styles
		{
			public readonly GUIStyle sMenuItem = "MenuItem";

			public readonly GUIStyle sSeparator = "sv_iconselector_sep";

			public readonly GUIContent sTitle = EditorGUIUtility.TextContent("Settings");

			public readonly GUIContent sMultiView = EditorGUIUtility.TextContent("Multi-view");

			public readonly GUIContent sCamera = EditorGUIUtility.TextContent("Camera");

			public readonly GUIContent sLighting = EditorGUIUtility.TextContent("Lighting");

			public readonly GUIContent sAnimation = EditorGUIUtility.TextContent("Animation");

			public readonly GUIContent sViewport = EditorGUIUtility.TextContent("Viewport");

			public readonly GUIContent sEnvLibrary = EditorGUIUtility.TextContent("Environment Library");

			public readonly GUIContent sMisc = EditorGUIUtility.TextContent("Misc");

			public readonly GUIContent sResetCamera = EditorGUIUtility.TextContent("Fit View        F");

			public readonly GUIContent sCreateNewLibrary = EditorGUIUtility.TextContent("Save as new library");

			public readonly GUIContent sSaveCurrentLibrary = EditorGUIUtility.TextContent("Save current library");

			public readonly GUIContent sResetView = EditorGUIUtility.TextContent("Reset View");

			public readonly GUIContent sEnableToneMap = EditorGUIUtility.TextContent("Enable Tone Mapping");

			public readonly GUIContent sEnableAutoExp = EditorGUIUtility.TextContent("Enable Auto Exposure");

			public readonly GUIContent sExposureRange = EditorGUIUtility.TextContent("Exposure Range");

			public readonly GUIContent sEnableShadows = EditorGUIUtility.TextContent("Enable Shadows");

			public readonly GUIContent sShadowDistance = EditorGUIUtility.TextContent("Shadow distance");

			public readonly GUIContent sShowBalls = EditorGUIUtility.TextContent("Show Chrome/grey balls");

			public readonly GUIContent sShowControlWindows = EditorGUIUtility.TextContent("Show Controls");

			public readonly GUIContent sAllowDifferentObjects = EditorGUIUtility.TextContent("Allow Different Objects");

			public readonly GUIContent sResyncObjects = EditorGUIUtility.TextContent("Resynchronize Objects");

			public readonly GUIContent sRotateObjectMode = EditorGUIUtility.TextContent("Rotate Objects");

			public readonly GUIContent sObjRotationSpeed = EditorGUIUtility.TextContent("Rotate Objects speed");

			public readonly GUIContent sRotateEnvMode = EditorGUIUtility.TextContent("Rotate environment");

			public readonly GUIContent sEnvRotationSpeed = EditorGUIUtility.TextContent("Rotate Env. speed");

			public readonly GUIContent sEnableShadowIcon = EditorGUIUtility.IconContent("LookDevShadow", "Shadow|Toggles shadows on and off");

			public readonly GUIContent sEnableObjRotationIcon = EditorGUIUtility.IconContent("LookDevObjRotation", "ObjRotation|Toggles object rotation (turntable) on and off");

			public readonly GUIContent sEnableEnvRotationIcon = EditorGUIUtility.IconContent("LookDevEnvRotation", "EnvRotation|Toggles environment rotation on and off");

			public readonly Texture sEnableShadowTexture = EditorGUIUtility.FindTexture("LookDevShadow");

			public readonly Texture sEnableObjRotationTexture = EditorGUIUtility.FindTexture("LookDevObjRotation");

			public readonly Texture sEnableEnvRotationTexture = EditorGUIUtility.FindTexture("LookDevEnvRotation");

			public readonly GUIContent[] sMultiViewMode = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Single1"),
				EditorGUIUtility.TextContent("Single2"),
				EditorGUIUtility.TextContent("Side by side"),
				EditorGUIUtility.TextContent("Split-screen"),
				EditorGUIUtility.TextContent("Zone")
			};

			public readonly Texture[] sMultiViewTextures = new Texture[]
			{
				EditorGUIUtility.FindTexture("LookDevSingle1"),
				EditorGUIUtility.FindTexture("LookDevSingle2"),
				EditorGUIUtility.FindTexture("LookDevSideBySide"),
				EditorGUIUtility.FindTexture("LookDevSplit"),
				EditorGUIUtility.FindTexture("LookDevZone")
			};
		}

		private enum UINumElement
		{
			UINumDrawHeader = 6,
			UINumToggle = 12,
			UINumSlider = 4,
			UINumSeparator = 7,
			UINumButton = 6,
			UITotalElement = 35
		}

		private static LookDevSettingsWindow.Styles s_Styles = null;

		private readonly float m_WindowHeight = 560f;

		private const float m_WindowWidth = 180f;

		private const float kIconSize = 16f;

		private const float kIconHorizontalPadding = 3f;

		private readonly LookDevView m_LookDevView;

		public static LookDevSettingsWindow.Styles styles
		{
			get
			{
				if (LookDevSettingsWindow.s_Styles == null)
				{
					LookDevSettingsWindow.s_Styles = new LookDevSettingsWindow.Styles();
				}
				return LookDevSettingsWindow.s_Styles;
			}
		}

		public LookDevSettingsWindow(LookDevView lookDevView)
		{
			this.m_LookDevView = lookDevView;
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(180f, this.m_WindowHeight);
		}

		public override void OnGUI(Rect rect)
		{
			if (!(this.m_LookDevView == null))
			{
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				EditorGUIUtility.labelWidth = 130f;
				EditorGUIUtility.fieldWidth = 35f;
				this.DrawHeader(LookDevSettingsWindow.styles.sMultiView);
				for (int i = 0; i < 5; i++)
				{
					EditorGUI.BeginChangeCheck();
					bool value = GUILayout.Toggle(this.m_LookDevView.config.lookDevMode == (LookDevMode)i, LookDevSettingsWindow.styles.sMultiViewMode[i], LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						this.m_LookDevView.UpdateLookDevModeToggle((LookDevMode)i, value);
						this.m_LookDevView.Repaint();
						GUIUtility.ExitGUI();
					}
				}
				this.DrawSeparator();
				this.DrawHeader(LookDevSettingsWindow.styles.sCamera);
				if (GUILayout.Button(LookDevSettingsWindow.styles.sResetCamera, LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]))
				{
					this.m_LookDevView.Frame();
				}
				this.m_LookDevView.config.enableToneMap = GUILayout.Toggle(this.m_LookDevView.config.enableToneMap, LookDevSettingsWindow.styles.sEnableToneMap, LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]);
				EditorGUI.BeginChangeCheck();
				float exposureRange = (float)EditorGUILayout.IntSlider(LookDevSettingsWindow.styles.sExposureRange, (int)this.m_LookDevView.config.exposureRange, 1, 32, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(this.m_LookDevView.config, "Change exposure range");
					this.m_LookDevView.config.exposureRange = exposureRange;
				}
				this.DrawSeparator();
				this.DrawHeader(LookDevSettingsWindow.styles.sLighting);
				EditorGUI.BeginChangeCheck();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.m_LookDevView.config.enableShadowCubemap = GUILayout.Toggle(this.m_LookDevView.config.enableShadowCubemap, LookDevSettingsWindow.styles.sEnableShadows, LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				if (EditorGUI.EndChangeCheck())
				{
					this.m_LookDevView.Repaint();
				}
				EditorGUI.BeginChangeCheck();
				float shadowDistance = EditorGUILayout.Slider(LookDevSettingsWindow.styles.sShadowDistance, this.m_LookDevView.config.shadowDistance, 0f, 1000f, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(this.m_LookDevView.config, "Change shadow distance");
					this.m_LookDevView.config.shadowDistance = shadowDistance;
				}
				this.DrawSeparator();
				this.DrawHeader(LookDevSettingsWindow.styles.sAnimation);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.m_LookDevView.config.rotateObjectMode = GUILayout.Toggle(this.m_LookDevView.config.rotateObjectMode, LookDevSettingsWindow.styles.sRotateObjectMode, LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.m_LookDevView.config.rotateEnvMode = GUILayout.Toggle(this.m_LookDevView.config.rotateEnvMode, LookDevSettingsWindow.styles.sRotateEnvMode, LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				EditorGUI.BeginChangeCheck();
				float objRotationSpeed = EditorGUILayout.Slider(LookDevSettingsWindow.styles.sObjRotationSpeed, this.m_LookDevView.config.objRotationSpeed, -5f, 5f, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(this.m_LookDevView.config, "Change rotation speed");
					this.m_LookDevView.config.objRotationSpeed = objRotationSpeed;
				}
				EditorGUI.BeginChangeCheck();
				float envRotationSpeed = EditorGUILayout.Slider(LookDevSettingsWindow.styles.sEnvRotationSpeed, this.m_LookDevView.config.envRotationSpeed, -5f, 5f, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(this.m_LookDevView.config, "Change env speed");
					this.m_LookDevView.config.envRotationSpeed = envRotationSpeed;
				}
				this.DrawSeparator();
				this.DrawHeader(LookDevSettingsWindow.styles.sViewport);
				if (GUILayout.Button(LookDevSettingsWindow.styles.sResetView, LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]))
				{
					this.m_LookDevView.ResetView();
				}
				this.DrawSeparator();
				this.DrawHeader(LookDevSettingsWindow.styles.sEnvLibrary);
				using (new EditorGUI.DisabledScope(!this.m_LookDevView.envLibrary.dirty))
				{
					if (GUILayout.Button(LookDevSettingsWindow.styles.sSaveCurrentLibrary, LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]))
					{
						if (this.m_LookDevView.SaveLookDevLibrary())
						{
							this.m_LookDevView.envLibrary.dirty = false;
						}
					}
				}
				if (GUILayout.Button(LookDevSettingsWindow.styles.sCreateNewLibrary, LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]))
				{
					string text = EditorUtility.SaveFilePanelInProject("Save New Environment Library", "New Env Library", "asset", "");
					if (!string.IsNullOrEmpty(text))
					{
						this.m_LookDevView.CreateNewLibrary(text);
					}
				}
				EditorGUI.BeginChangeCheck();
				LookDevEnvironmentLibrary envLibrary = EditorGUILayout.ObjectField(this.m_LookDevView.userEnvLibrary, typeof(LookDevEnvironmentLibrary), false, new GUILayoutOption[0]) as LookDevEnvironmentLibrary;
				if (EditorGUI.EndChangeCheck())
				{
					this.m_LookDevView.envLibrary = envLibrary;
				}
				this.DrawSeparator();
				this.DrawHeader(LookDevSettingsWindow.styles.sMisc);
				this.m_LookDevView.config.showBalls = GUILayout.Toggle(this.m_LookDevView.config.showBalls, LookDevSettingsWindow.styles.sShowBalls, LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]);
				this.m_LookDevView.config.showControlWindows = GUILayout.Toggle(this.m_LookDevView.config.showControlWindows, LookDevSettingsWindow.styles.sShowControlWindows, LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]);
				EditorGUI.BeginChangeCheck();
				bool allowDifferentObjects = GUILayout.Toggle(this.m_LookDevView.config.allowDifferentObjects, LookDevSettingsWindow.styles.sAllowDifferentObjects, LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_LookDevView.config.allowDifferentObjects = allowDifferentObjects;
				}
				if (GUILayout.Button(LookDevSettingsWindow.styles.sResyncObjects, LookDevSettingsWindow.styles.sMenuItem, new GUILayoutOption[0]))
				{
					this.m_LookDevView.config.ResynchronizeObjects();
				}
				GUILayout.EndVertical();
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
		}

		private void DrawSeparator()
		{
			GUILayout.Space(3f);
			GUILayout.Label(GUIContent.none, LookDevSettingsWindow.styles.sSeparator, new GUILayoutOption[0]);
		}

		private void DrawHeader(GUIContent label)
		{
			GUILayout.Label(label, EditorStyles.miniLabel, new GUILayoutOption[0]);
		}
	}
}
