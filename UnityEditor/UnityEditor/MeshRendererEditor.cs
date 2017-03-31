using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(MeshRenderer))]
	internal class MeshRendererEditor : RendererEditorBase
	{
		private class Styles
		{
			public static readonly string MaterialWarning = "This renderer has more materials than the Mesh has submeshes. Multiple materials will be applied to the same submesh, which costs performance. Consider using multiple shader passes.";

			public static readonly string StaticBatchingWarning = "This renderer is statically batched and uses an instanced shader at the same time. Instancing will be disabled in such a case. Consider disabling static batching if you want it to be instanced.";
		}

		private SerializedProperty m_Materials;

		private LightingSettingsInspector m_Lighting;

		private const string kDisplayLightingKey = "MeshRendererEditor.Lighting.ShowSettings";

		private const string kDisplayLightmapKey = "MeshRendererEditor.Lighting.ShowLightmapSettings";

		private const string kDisplayChartingKey = "MeshRendererEditor.Lighting.ShowChartingSettings";

		private SerializedObject m_GameObjectsSerializedObject;

		private SerializedProperty m_GameObjectStaticFlags;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Materials = base.serializedObject.FindProperty("m_Materials");
			this.m_GameObjectsSerializedObject = new SerializedObject((from t in base.targets
			select ((MeshRenderer)t).gameObject).ToArray<GameObject>());
			this.m_GameObjectStaticFlags = this.m_GameObjectsSerializedObject.FindProperty("m_StaticEditorFlags");
			base.InitializeProbeFields();
			this.InitializeLightingFields();
		}

		private void InitializeLightingFields()
		{
			this.m_Lighting = new LightingSettingsInspector(base.serializedObject);
			this.m_Lighting.showSettings = EditorPrefs.GetBool("MeshRendererEditor.Lighting.ShowSettings", false);
			this.m_Lighting.showChartingSettings = SessionState.GetBool("MeshRendererEditor.Lighting.ShowChartingSettings", true);
			this.m_Lighting.showLightmapSettings = SessionState.GetBool("MeshRendererEditor.Lighting.ShowLightmapSettings", true);
		}

		private void LightingFieldsGUI()
		{
			bool showSettings = this.m_Lighting.showSettings;
			bool showChartingSettings = this.m_Lighting.showChartingSettings;
			bool showLightmapSettings = this.m_Lighting.showLightmapSettings;
			if (this.m_Lighting.Begin())
			{
				base.RenderProbeFields();
				this.m_Lighting.RenderMeshSettings(true);
			}
			this.m_Lighting.End();
			if (this.m_Lighting.showSettings != showSettings)
			{
				EditorPrefs.SetBool("MeshRendererEditor.Lighting.ShowSettings", this.m_Lighting.showSettings);
			}
			if (this.m_Lighting.showChartingSettings != showChartingSettings)
			{
				SessionState.SetBool("MeshRendererEditor.Lighting.ShowChartingSettings", this.m_Lighting.showChartingSettings);
			}
			if (this.m_Lighting.showLightmapSettings != showLightmapSettings)
			{
				SessionState.SetBool("MeshRendererEditor.Lighting.ShowLightmapSettings", this.m_Lighting.showLightmapSettings);
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.LightingFieldsGUI();
			bool flag = false;
			if (!this.m_Materials.hasMultipleDifferentValues)
			{
				MeshFilter component = ((MeshRenderer)base.serializedObject.targetObject).GetComponent<MeshFilter>();
				flag = (component != null && component.sharedMesh != null && this.m_Materials.arraySize > component.sharedMesh.subMeshCount);
			}
			EditorGUILayout.PropertyField(this.m_Materials, true, new GUILayoutOption[0]);
			if (!this.m_Materials.hasMultipleDifferentValues && flag)
			{
				EditorGUILayout.HelpBox(MeshRendererEditor.Styles.MaterialWarning, MessageType.Warning, true);
			}
			if (ShaderUtil.MaterialsUseInstancingShader(this.m_Materials))
			{
				this.m_GameObjectsSerializedObject.Update();
				if (!this.m_GameObjectStaticFlags.hasMultipleDifferentValues && (this.m_GameObjectStaticFlags.intValue & 4) != 0)
				{
					EditorGUILayout.HelpBox(MeshRendererEditor.Styles.StaticBatchingWarning, MessageType.Warning, true);
				}
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
