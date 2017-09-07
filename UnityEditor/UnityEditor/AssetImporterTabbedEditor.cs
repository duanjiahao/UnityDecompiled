using System;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace UnityEditor
{
	internal abstract class AssetImporterTabbedEditor : AssetImporterEditor
	{
		protected string[] m_TabNames = null;

		private int m_ActiveEditorIndex = 0;

		private BaseAssetImporterTabUI[] m_Tabs = null;

		public BaseAssetImporterTabUI activeTab
		{
			get;
			private set;
		}

		protected BaseAssetImporterTabUI[] tabs
		{
			get
			{
				return this.m_Tabs;
			}
			set
			{
				this.m_Tabs = value;
			}
		}

		public override void OnEnable()
		{
			BaseAssetImporterTabUI[] tabs = this.m_Tabs;
			for (int i = 0; i < tabs.Length; i++)
			{
				BaseAssetImporterTabUI baseAssetImporterTabUI = tabs[i];
				baseAssetImporterTabUI.OnEnable();
			}
			this.m_ActiveEditorIndex = EditorPrefs.GetInt(base.GetType().Name + "ActiveEditorIndex", 0);
			if (this.activeTab == null)
			{
				this.activeTab = this.m_Tabs[this.m_ActiveEditorIndex];
			}
		}

		private void OnDestroy()
		{
			if (this.m_Tabs != null)
			{
				BaseAssetImporterTabUI[] tabs = this.m_Tabs;
				for (int i = 0; i < tabs.Length; i++)
				{
					BaseAssetImporterTabUI baseAssetImporterTabUI = tabs[i];
					baseAssetImporterTabUI.OnDestroy();
				}
				this.m_Tabs = null;
				this.activeTab = null;
			}
		}

		protected override void ResetValues()
		{
			base.ResetValues();
			if (this.m_Tabs != null)
			{
				BaseAssetImporterTabUI[] tabs = this.m_Tabs;
				for (int i = 0; i < tabs.Length; i++)
				{
					BaseAssetImporterTabUI baseAssetImporterTabUI = tabs[i];
					baseAssetImporterTabUI.ResetValues();
				}
			}
		}

		public override void OnInspectorGUI()
		{
			using (new EditorGUI.DisabledScope(false))
			{
				GUI.enabled = true;
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				EditorGUI.BeginChangeCheck();
				this.m_ActiveEditorIndex = GUILayout.Toolbar(this.m_ActiveEditorIndex, this.m_TabNames, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					EditorPrefs.SetInt(base.GetType().Name + "ActiveEditorIndex", this.m_ActiveEditorIndex);
					this.activeTab = this.m_Tabs[this.m_ActiveEditorIndex];
					this.activeTab.OnInspectorGUI();
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			if (this.activeTab != null)
			{
				this.activeTab.OnInspectorGUI();
			}
			base.ApplyRevertGUI();
		}

		public override void OnPreviewSettings()
		{
			if (this.activeTab != null)
			{
				this.activeTab.OnPreviewSettings();
			}
		}

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (this.activeTab != null)
			{
				this.activeTab.OnInteractivePreviewGUI(r, background);
			}
		}

		public override bool HasPreviewGUI()
		{
			return this.activeTab != null && this.activeTab.HasPreviewGUI();
		}

		protected override void Apply()
		{
			if (this.m_Tabs != null)
			{
				BaseAssetImporterTabUI[] tabs = this.m_Tabs;
				for (int i = 0; i < tabs.Length; i++)
				{
					BaseAssetImporterTabUI baseAssetImporterTabUI = tabs[i];
					baseAssetImporterTabUI.PreApply();
				}
				base.Apply();
				BaseAssetImporterTabUI[] tabs2 = this.m_Tabs;
				for (int j = 0; j < tabs2.Length; j++)
				{
					BaseAssetImporterTabUI baseAssetImporterTabUI2 = tabs2[j];
					baseAssetImporterTabUI2.PostApply();
				}
			}
		}
	}
}
