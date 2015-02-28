using System;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(RenderSettings))]
	internal class FogEditor : Editor
	{
		internal class Styles
		{
			public static readonly GUIContent fogHeader = EditorGUIUtility.TextContent("RenderSettings.FogHeader");
			public static readonly GUIContent fogWarning = EditorGUIUtility.TextContent("RenderSettings.FogWarning");
			public static readonly GUIContent fogDensity = EditorGUIUtility.TextContent("RenderSettings.FogDensity");
			public static readonly GUIContent fogLinearStart = EditorGUIUtility.TextContent("RenderSettings.FogLinearStart");
			public static readonly GUIContent fogLinearEnd = EditorGUIUtility.TextContent("RenderSettings.FogLinearEnd");
		}
		private const string kShowEditorKey = "ShowFogEditorFoldout";
		protected SerializedProperty m_Fog;
		protected SerializedProperty m_FogColor;
		protected SerializedProperty m_FogMode;
		protected SerializedProperty m_FogDensity;
		protected SerializedProperty m_LinearFogStart;
		protected SerializedProperty m_LinearFogEnd;
		private bool m_ShowEditor;
		public virtual void OnEnable()
		{
			this.m_Fog = base.serializedObject.FindProperty("m_Fog");
			this.m_FogColor = base.serializedObject.FindProperty("m_FogColor");
			this.m_FogMode = base.serializedObject.FindProperty("m_FogMode");
			this.m_FogDensity = base.serializedObject.FindProperty("m_FogDensity");
			this.m_LinearFogStart = base.serializedObject.FindProperty("m_LinearFogStart");
			this.m_LinearFogEnd = base.serializedObject.FindProperty("m_LinearFogEnd");
			this.m_ShowEditor = InspectorState.GetBool("ShowFogEditorFoldout", false);
		}
		public virtual void OnDisable()
		{
			InspectorState.SetBool("ShowFogEditorFoldout", this.m_ShowEditor);
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.m_ShowEditor = EditorGUILayout.ToggleTitlebar(this.m_ShowEditor, FogEditor.Styles.fogHeader, this.m_Fog);
			if (this.m_ShowEditor)
			{
				EditorGUI.indentLevel++;
				EditorGUI.BeginDisabledGroup(!this.m_Fog.boolValue);
				EditorGUILayout.PropertyField(this.m_FogColor, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_FogMode, new GUILayoutOption[0]);
				EditorGUI.indentLevel++;
				FogMode intValue = (FogMode)this.m_FogMode.intValue;
				if (intValue != FogMode.Linear)
				{
					EditorGUILayout.PropertyField(this.m_FogDensity, FogEditor.Styles.fogDensity, new GUILayoutOption[0]);
				}
				else
				{
					EditorGUILayout.PropertyField(this.m_LinearFogStart, FogEditor.Styles.fogLinearStart, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_LinearFogEnd, FogEditor.Styles.fogLinearEnd, new GUILayoutOption[0]);
				}
				EditorGUI.indentLevel--;
				if (SceneView.GetSceneViewRenderingPath() == RenderingPath.DeferredShading)
				{
					EditorGUILayout.HelpBox(FogEditor.Styles.fogWarning.text, MessageType.Info);
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUI.EndDisabledGroup();
				EditorGUI.indentLevel--;
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
