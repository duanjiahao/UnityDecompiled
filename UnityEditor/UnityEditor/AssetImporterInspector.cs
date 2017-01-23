using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal abstract class AssetImporterInspector : Editor
	{
		private ulong m_AssetTimeStamp = 0uL;

		private bool m_MightHaveModified = false;

		private Editor m_AssetEditor;

		internal virtual Editor assetEditor
		{
			get
			{
				return this.m_AssetEditor;
			}
			set
			{
				this.m_AssetEditor = value;
			}
		}

		internal override string targetTitle
		{
			get
			{
				return string.Format("{0} Import Settings", (!(this.assetEditor == null)) ? this.assetEditor.targetTitle : string.Empty);
			}
		}

		internal override int referenceTargetIndex
		{
			get
			{
				return base.referenceTargetIndex;
			}
			set
			{
				base.referenceTargetIndex = value;
				if (this.assetEditor != null)
				{
					this.assetEditor.referenceTargetIndex = value;
				}
			}
		}

		internal override IPreviewable preview
		{
			get
			{
				IPreviewable result;
				if (this.useAssetDrawPreview && this.assetEditor != null)
				{
					result = this.assetEditor;
				}
				else
				{
					result = base.preview;
				}
				return result;
			}
		}

		protected virtual bool useAssetDrawPreview
		{
			get
			{
				return true;
			}
		}

		internal virtual bool showImportedObject
		{
			get
			{
				return true;
			}
		}

		internal override void OnHeaderIconGUI(Rect iconRect)
		{
			if (this.assetEditor != null)
			{
				this.assetEditor.OnHeaderIconGUI(iconRect);
			}
		}

		internal override SerializedObject GetSerializedObjectInternal()
		{
			if (this.m_SerializedObject == null)
			{
				this.m_SerializedObject = SerializedObject.LoadFromCache(base.GetInstanceID());
			}
			if (this.m_SerializedObject == null)
			{
				this.m_SerializedObject = new SerializedObject(base.targets);
			}
			return this.m_SerializedObject;
		}

		public virtual void OnDisable()
		{
			AssetImporter assetImporter = base.target as AssetImporter;
			if (Unsupported.IsDestroyScriptableObject(this) && this.m_MightHaveModified && assetImporter != null && !InternalEditorUtility.ignoreInspectorChanges && this.HasModified() && !this.AssetWasUpdated())
			{
				string message = "Unapplied import settings for '" + assetImporter.assetPath + "'";
				if (base.targets.Length > 1)
				{
					message = "Unapplied import settings for '" + base.targets.Length + "' files";
				}
				if (EditorUtility.DisplayDialog("Unapplied import settings", message, "Apply", "Revert"))
				{
					this.Apply();
					this.m_MightHaveModified = false;
					AssetImporterInspector.ImportAssets(this.GetAssetPaths());
				}
			}
			if (this.m_SerializedObject != null && this.m_SerializedObject.hasModifiedProperties)
			{
				this.m_SerializedObject.Cache(base.GetInstanceID());
				this.m_SerializedObject = null;
			}
		}

		internal virtual void Awake()
		{
			this.ResetTimeStamp();
			this.ResetValues();
		}

		private string[] GetAssetPaths()
		{
			UnityEngine.Object[] targets = base.targets;
			string[] array = new string[targets.Length];
			for (int i = 0; i < targets.Length; i++)
			{
				AssetImporter assetImporter = targets[i] as AssetImporter;
				array[i] = assetImporter.assetPath;
			}
			return array;
		}

		internal virtual void ResetValues()
		{
			base.serializedObject.SetIsDifferentCacheDirty();
			base.serializedObject.Update();
		}

		internal virtual bool HasModified()
		{
			return base.serializedObject.hasModifiedProperties;
		}

		internal virtual void Apply()
		{
			base.serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}

		internal bool AssetWasUpdated()
		{
			AssetImporter assetImporter = base.target as AssetImporter;
			if (this.m_AssetTimeStamp == 0uL)
			{
				this.ResetTimeStamp();
			}
			return assetImporter != null && this.m_AssetTimeStamp != assetImporter.assetTimeStamp;
		}

		internal void ResetTimeStamp()
		{
			AssetImporter assetImporter = base.target as AssetImporter;
			if (assetImporter != null)
			{
				this.m_AssetTimeStamp = assetImporter.assetTimeStamp;
			}
		}

		internal void ApplyAndImport()
		{
			this.Apply();
			this.m_MightHaveModified = false;
			AssetImporterInspector.ImportAssets(this.GetAssetPaths());
			this.ResetValues();
		}

		private static void ImportAssets(string[] paths)
		{
			for (int i = 0; i < paths.Length; i++)
			{
				string path = paths[i];
				AssetDatabase.WriteImportSettingsIfDirty(path);
			}
			try
			{
				AssetDatabase.StartAssetEditing();
				for (int j = 0; j < paths.Length; j++)
				{
					string path2 = paths[j];
					AssetDatabase.ImportAsset(path2);
				}
			}
			finally
			{
				AssetDatabase.StopAssetEditing();
			}
		}

		protected void RevertButton()
		{
			this.RevertButton("Revert");
		}

		protected void RevertButton(string buttonText)
		{
			if (GUILayout.Button(buttonText, new GUILayoutOption[0]))
			{
				this.m_MightHaveModified = false;
				this.ResetTimeStamp();
				this.ResetValues();
				if (this.HasModified())
				{
					Debug.LogError("Importer reports modified values after reset.");
				}
			}
		}

		protected bool ApplyButton()
		{
			return this.ApplyButton("Apply");
		}

		protected bool ApplyButton(string buttonText)
		{
			bool result;
			if (GUILayout.Button(buttonText, new GUILayoutOption[0]))
			{
				this.ApplyAndImport();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected virtual bool ApplyRevertGUIButtons()
		{
			bool result;
			using (new EditorGUI.DisabledScope(!this.HasModified()))
			{
				this.RevertButton();
				result = this.ApplyButton();
			}
			return result;
		}

		protected void ApplyRevertGUI()
		{
			this.m_MightHaveModified = true;
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			bool flag = this.ApplyRevertGUIButtons();
			if (this.AssetWasUpdated() && Event.current.type != EventType.Layout)
			{
				IPreviewable preview = this.preview;
				if (preview != null)
				{
					preview.ReloadPreviewInstances();
				}
				this.ResetTimeStamp();
				this.ResetValues();
				base.Repaint();
			}
			GUILayout.EndHorizontal();
			if (flag)
			{
				GUIUtility.ExitGUI();
			}
		}
	}
}
