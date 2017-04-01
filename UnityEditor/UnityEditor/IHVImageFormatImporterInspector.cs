using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(IHVImageFormatImporter))]
	internal class IHVImageFormatImporterInspector : AssetImporterInspector
	{
		internal class Styles
		{
			public static readonly GUIContent readWrite = EditorGUIUtility.TextContent("Read/Write Enabled|Enable to be able to access the raw pixel data from code.");

			public static readonly GUIContent wrapMode = EditorGUIUtility.TextContent("Wrap Mode");

			public static readonly GUIContent filterMode = EditorGUIUtility.TextContent("Filter Mode");

			public static readonly int[] filterModeValues = new int[]
			{
				0,
				1,
				2
			};

			public static readonly GUIContent[] filterModeOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Point (no filter)"),
				EditorGUIUtility.TextContent("Bilinear"),
				EditorGUIUtility.TextContent("Trilinear")
			};
		}

		private SerializedProperty m_IsReadable;

		private SerializedProperty m_FilterMode;

		private SerializedProperty m_WrapMode;

		internal override bool showImportedObject
		{
			get
			{
				return false;
			}
		}

		public virtual void OnEnable()
		{
			this.m_IsReadable = base.serializedObject.FindProperty("m_IsReadable");
			this.m_FilterMode = base.serializedObject.FindProperty("m_TextureSettings.m_FilterMode");
			this.m_WrapMode = base.serializedObject.FindProperty("m_TextureSettings.m_WrapMode");
		}

		public void IsReadableGUI()
		{
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			EditorGUI.BeginProperty(controlRect, IHVImageFormatImporterInspector.Styles.readWrite, this.m_IsReadable);
			EditorGUI.BeginChangeCheck();
			bool boolValue = EditorGUI.Toggle(controlRect, IHVImageFormatImporterInspector.Styles.readWrite, this.m_IsReadable.boolValue);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_IsReadable.boolValue = boolValue;
			}
			EditorGUI.EndProperty();
		}

		public void TextureSettingsGUI()
		{
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			EditorGUI.BeginProperty(controlRect, IHVImageFormatImporterInspector.Styles.wrapMode, this.m_WrapMode);
			EditorGUI.BeginChangeCheck();
			TextureWrapMode textureWrapMode = (TextureWrapMode)((this.m_WrapMode.intValue != -1) ? this.m_WrapMode.intValue : 0);
			textureWrapMode = (TextureWrapMode)EditorGUI.EnumPopup(controlRect, IHVImageFormatImporterInspector.Styles.wrapMode, textureWrapMode);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_WrapMode.intValue = (int)textureWrapMode;
			}
			EditorGUI.EndProperty();
			controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			EditorGUI.BeginProperty(controlRect, IHVImageFormatImporterInspector.Styles.filterMode, this.m_FilterMode);
			EditorGUI.BeginChangeCheck();
			FilterMode filterMode = (FilterMode)((this.m_FilterMode.intValue != -1) ? this.m_FilterMode.intValue : 1);
			filterMode = (FilterMode)EditorGUI.IntPopup(controlRect, IHVImageFormatImporterInspector.Styles.filterMode, (int)filterMode, IHVImageFormatImporterInspector.Styles.filterModeOptions, IHVImageFormatImporterInspector.Styles.filterModeValues);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_FilterMode.intValue = (int)filterMode;
			}
			EditorGUI.EndProperty();
		}

		public override void OnInspectorGUI()
		{
			this.IsReadableGUI();
			EditorGUI.BeginChangeCheck();
			this.TextureSettingsGUI();
			if (EditorGUI.EndChangeCheck())
			{
				UnityEngine.Object[] targets = base.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					AssetImporter assetImporter = (AssetImporter)targets[i];
					Texture tex = AssetDatabase.LoadMainAssetAtPath(assetImporter.assetPath) as Texture;
					if (this.m_FilterMode.intValue != -1)
					{
						TextureUtil.SetFilterModeNoDirty(tex, (FilterMode)this.m_FilterMode.intValue);
					}
					if (this.m_WrapMode.intValue != -1)
					{
						TextureUtil.SetWrapModeNoDirty(tex, (TextureWrapMode)this.m_WrapMode.intValue);
					}
				}
				SceneView.RepaintAll();
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			base.ApplyRevertGUI();
			GUILayout.EndHorizontal();
		}
	}
}
