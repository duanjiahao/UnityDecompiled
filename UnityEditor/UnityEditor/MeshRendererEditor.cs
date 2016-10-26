using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(MeshRenderer))]
	internal class MeshRendererEditor : RendererEditorBase
	{
		private SerializedProperty m_CastShadows;

		private SerializedProperty m_ReceiveShadows;

		private SerializedProperty m_MotionVectors;

		private SerializedProperty m_Materials;

		private SerializedObject m_GameObjectsSerializedObject;

		private SerializedProperty m_GameObjectStaticFlags;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_CastShadows = base.serializedObject.FindProperty("m_CastShadows");
			this.m_ReceiveShadows = base.serializedObject.FindProperty("m_ReceiveShadows");
			this.m_MotionVectors = base.serializedObject.FindProperty("m_MotionVectors");
			this.m_Materials = base.serializedObject.FindProperty("m_Materials");
			this.m_GameObjectsSerializedObject = new SerializedObject((from t in base.targets
			select ((MeshRenderer)t).gameObject).ToArray<GameObject>());
			this.m_GameObjectStaticFlags = this.m_GameObjectsSerializedObject.FindProperty("m_StaticEditorFlags");
			base.InitializeProbeFields();
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			bool flag = false;
			SerializedProperty serializedProperty = base.serializedObject.FindProperty("m_Materials");
			if (!serializedProperty.hasMultipleDifferentValues)
			{
				MeshFilter component = ((MeshRenderer)base.serializedObject.targetObject).GetComponent<MeshFilter>();
				flag = (component != null && component.sharedMesh != null && serializedProperty.arraySize > component.sharedMesh.subMeshCount);
			}
			EditorGUILayout.PropertyField(this.m_CastShadows, true, new GUILayoutOption[0]);
			bool disabled = SceneView.IsUsingDeferredRenderingPath();
			using (new EditorGUI.DisabledScope(disabled))
			{
				EditorGUILayout.PropertyField(this.m_ReceiveShadows, true, new GUILayoutOption[0]);
			}
			EditorGUILayout.PropertyField(this.m_MotionVectors, true, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Materials, true, new GUILayoutOption[0]);
			if (!this.m_Materials.hasMultipleDifferentValues && flag)
			{
				EditorGUILayout.HelpBox("This renderer has more materials than the Mesh has submeshes. Multiple materials will be applied to the same submesh, which costs performance. Consider using multiple shader passes.", MessageType.Warning, true);
			}
			if (ShaderUtil.MaterialsUseInstancingShader(serializedProperty))
			{
				this.m_GameObjectsSerializedObject.Update();
				if (!this.m_GameObjectStaticFlags.hasMultipleDifferentValues && (this.m_GameObjectStaticFlags.intValue & 4) != 0)
				{
					EditorGUILayout.HelpBox("This renderer is statically batched and uses an instanced shader at the same time. Instancing will be disabled in such a case. Consider disabling static batching if you want it to be instanced.", MessageType.Warning, true);
				}
			}
			base.RenderProbeFields();
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
