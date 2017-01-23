using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SkinnedMeshRenderer))]
	internal class SkinnedMeshRendererEditor : RendererEditorBase
	{
		private static int s_BoxHash = "SkinnedMeshRendererEditor".GetHashCode();

		private SerializedProperty m_CastShadows;

		private SerializedProperty m_ReceiveShadows;

		private SerializedProperty m_MotionVectors;

		private SerializedProperty m_Materials;

		private SerializedProperty m_AABB;

		private SerializedProperty m_DirtyAABB;

		private SerializedProperty m_BlendShapeWeights;

		private BoxEditor m_BoxEditor = new BoxEditor(false, SkinnedMeshRendererEditor.s_BoxHash);

		private string[] m_ExcludedProperties;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_CastShadows = base.serializedObject.FindProperty("m_CastShadows");
			this.m_ReceiveShadows = base.serializedObject.FindProperty("m_ReceiveShadows");
			this.m_MotionVectors = base.serializedObject.FindProperty("m_MotionVectors");
			this.m_Materials = base.serializedObject.FindProperty("m_Materials");
			this.m_BlendShapeWeights = base.serializedObject.FindProperty("m_BlendShapeWeights");
			this.m_AABB = base.serializedObject.FindProperty("m_AABB");
			this.m_DirtyAABB = base.serializedObject.FindProperty("m_DirtyAABB");
			this.m_BoxEditor.OnEnable();
			this.m_BoxEditor.SetAlwaysDisplayHandles(true);
			base.InitializeProbeFields();
			List<string> list = new List<string>();
			list.AddRange(new string[]
			{
				"m_CastShadows",
				"m_ReceiveShadows",
				"m_MotionVectors",
				"m_Materials",
				"m_BlendShapeWeights",
				"m_AABB"
			});
			list.AddRange(RendererEditorBase.Probes.GetFieldsStringArray());
			this.m_ExcludedProperties = list.ToArray();
		}

		public void OnDisable()
		{
			this.m_BoxEditor.OnDisable();
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.OnBlendShapeUI();
			EditorGUILayout.PropertyField(this.m_CastShadows, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_ReceiveShadows, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_MotionVectors, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Materials, true, new GUILayoutOption[0]);
			base.RenderProbeFields();
			Editor.DrawPropertiesExcluding(base.serializedObject, this.m_ExcludedProperties);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_AABB, new GUIContent("Bounds"), new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_DirtyAABB.boolValue = false;
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		public void OnBlendShapeUI()
		{
			SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)base.target;
			int num = (!(skinnedMeshRenderer.sharedMesh == null)) ? skinnedMeshRenderer.sharedMesh.blendShapeCount : 0;
			if (num != 0)
			{
				GUIContent gUIContent = new GUIContent();
				gUIContent.text = "BlendShapes";
				EditorGUILayout.PropertyField(this.m_BlendShapeWeights, gUIContent, false, new GUILayoutOption[0]);
				if (this.m_BlendShapeWeights.isExpanded)
				{
					EditorGUI.indentLevel++;
					Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
					int num2 = this.m_BlendShapeWeights.arraySize;
					for (int i = 0; i < num; i++)
					{
						gUIContent.text = sharedMesh.GetBlendShapeName(i);
						if (i < num2)
						{
							EditorGUILayout.PropertyField(this.m_BlendShapeWeights.GetArrayElementAtIndex(i), gUIContent, new GUILayoutOption[0]);
						}
						else
						{
							EditorGUI.BeginChangeCheck();
							float floatValue = EditorGUILayout.FloatField(gUIContent, 0f, new GUILayoutOption[0]);
							if (EditorGUI.EndChangeCheck())
							{
								this.m_BlendShapeWeights.arraySize = num;
								num2 = num;
								this.m_BlendShapeWeights.GetArrayElementAtIndex(i).floatValue = floatValue;
							}
						}
					}
					EditorGUI.indentLevel--;
				}
			}
		}

		public void OnSceneGUI()
		{
			SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)base.target;
			if (skinnedMeshRenderer.updateWhenOffscreen)
			{
				Bounds bounds = skinnedMeshRenderer.bounds;
				Vector3 center = bounds.center;
				Vector3 size = bounds.size;
				Handles.DrawWireCube(center, size);
			}
			else
			{
				Bounds localBounds = skinnedMeshRenderer.localBounds;
				Vector3 center2 = localBounds.center;
				Vector3 size2 = localBounds.size;
				if (this.m_BoxEditor.OnSceneGUI(skinnedMeshRenderer.actualRootBone, Handles.s_BoundingBoxHandleColor, false, ref center2, ref size2))
				{
					Undo.RecordObject(skinnedMeshRenderer, "Resize Bounds");
					skinnedMeshRenderer.localBounds = new Bounds(center2, size2);
				}
			}
		}
	}
}
