using System;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(MeshRenderer))]
	internal class MeshRendererEditor : RendererEditorBase
	{
		private string[] m_ExcludedProperties;
		public override void OnEnable()
		{
			base.OnEnable();
			base.InitializeProbeFields();
			List<string> list = new List<string>();
			list.Add("m_LightmapParameters");
			list.AddRange(RendererEditorBase.Probes.GetFieldsStringArray());
			this.m_ExcludedProperties = list.ToArray();
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			Editor.DrawPropertiesExcluding(base.serializedObject, this.m_ExcludedProperties);
			SerializedProperty serializedProperty = base.serializedObject.FindProperty("m_Materials");
			if (!serializedProperty.hasMultipleDifferentValues)
			{
				MeshRendererEditor.DisplayMaterialWarning(base.serializedObject, serializedProperty);
			}
			base.RenderProbeFields();
			base.serializedObject.ApplyModifiedProperties();
		}
		private static void DisplayMaterialWarning(SerializedObject obj, SerializedProperty property)
		{
			MeshFilter component = ((MeshRenderer)obj.targetObject).GetComponent<MeshFilter>();
			if (component != null && component.sharedMesh != null && property.arraySize > component.sharedMesh.subMeshCount)
			{
				EditorGUILayout.HelpBox("This renderer has more materials than the Mesh has submeshes. Multiple materials will be applied to the same submesh, which costs performance. Consider using multiple shader passes.", MessageType.Warning, true);
			}
		}
	}
}
