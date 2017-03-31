using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	internal class TierSettingsWindow : EditorWindow
	{
		private static TierSettingsWindow s_Instance;

		private Editor m_TierSettingsEditor;

		private UnityEngine.Object graphicsSettings
		{
			get
			{
				return GraphicsSettings.GetGraphicsSettings();
			}
		}

		private Editor tierSettingsEditor
		{
			get
			{
				Editor.CreateCachedEditor(this.graphicsSettings, typeof(GraphicsSettingsWindow.TierSettingsEditor), ref this.m_TierSettingsEditor);
				((GraphicsSettingsWindow.TierSettingsEditor)this.m_TierSettingsEditor).verticalLayout = false;
				return this.m_TierSettingsEditor;
			}
		}

		public static void CreateWindow()
		{
			TierSettingsWindow.s_Instance = EditorWindow.GetWindow<TierSettingsWindow>();
			TierSettingsWindow.s_Instance.minSize = new Vector2(600f, 300f);
			TierSettingsWindow.s_Instance.titleContent = EditorGUIUtility.TextContent("Tier Settings");
		}

		internal static TierSettingsWindow GetInstance()
		{
			return TierSettingsWindow.s_Instance;
		}

		private void OnEnable()
		{
			TierSettingsWindow.s_Instance = this;
		}

		private void OnDisable()
		{
			UnityEngine.Object.DestroyImmediate(this.m_TierSettingsEditor);
			this.m_TierSettingsEditor = null;
			if (TierSettingsWindow.s_Instance == this)
			{
				TierSettingsWindow.s_Instance = null;
			}
		}

		private void OnGUI()
		{
			this.tierSettingsEditor.OnInspectorGUI();
		}
	}
}
