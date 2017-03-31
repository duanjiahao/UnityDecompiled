using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SkinnedMeshRenderer))]
	internal class SkinnedMeshRendererEditor : RendererEditorBase
	{
		private static int s_HandleControlIDHint = typeof(SkinnedMeshRendererEditor).Name.GetHashCode();

		private const string kDisplayLightingKey = "SkinnedMeshRendererEditor.Lighting.ShowSettings";

		private SerializedProperty m_Materials;

		private SerializedProperty m_AABB;

		private SerializedProperty m_DirtyAABB;

		private SerializedProperty m_BlendShapeWeights;

		private LightingSettingsInspector m_Lighting;

		private BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle(SkinnedMeshRendererEditor.s_HandleControlIDHint);

		private string[] m_ExcludedProperties;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Materials = base.serializedObject.FindProperty("m_Materials");
			this.m_BlendShapeWeights = base.serializedObject.FindProperty("m_BlendShapeWeights");
			this.m_AABB = base.serializedObject.FindProperty("m_AABB");
			this.m_DirtyAABB = base.serializedObject.FindProperty("m_DirtyAABB");
			this.m_BoundsHandle.SetColor(Handles.s_BoundingBoxHandleColor);
			base.InitializeProbeFields();
			this.InitializeLightingFields();
			List<string> list = new List<string>();
			list.AddRange(new string[]
			{
				"m_CastShadows",
				"m_ReceiveShadows",
				"m_MotionVectors",
				"m_Materials",
				"m_BlendShapeWeights",
				"m_AABB",
				"m_LightmapParameters"
			});
			list.AddRange(RendererEditorBase.Probes.GetFieldsStringArray());
			this.m_ExcludedProperties = list.ToArray();
		}

		private void InitializeLightingFields()
		{
			this.m_Lighting = new LightingSettingsInspector(base.serializedObject);
			this.m_Lighting.showSettings = EditorPrefs.GetBool("SkinnedMeshRendererEditor.Lighting.ShowSettings", false);
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.OnBlendShapeUI();
			Editor.DrawPropertiesExcluding(base.serializedObject, this.m_ExcludedProperties);
			EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Collider, "Edit Bounds", PrimitiveBoundsHandle.editModeButton, (base.target as SkinnedMeshRenderer).bounds, this);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_AABB, new GUIContent("Bounds"), new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_DirtyAABB.boolValue = false;
			}
			this.LightingFieldsGUI();
			EditorGUILayout.PropertyField(this.m_Materials, true, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		private void LightingFieldsGUI()
		{
			bool showSettings = this.m_Lighting.showSettings;
			if (this.m_Lighting.Begin())
			{
				base.RenderProbeFields();
				this.m_Lighting.RenderMeshSettings(false);
			}
			this.m_Lighting.End();
			if (this.m_Lighting.showSettings != showSettings)
			{
				EditorPrefs.SetBool("SkinnedMeshRendererEditor.Lighting.ShowSettings", this.m_Lighting.showSettings);
			}
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
				using (new Handles.DrawingScope(skinnedMeshRenderer.actualRootBone.localToWorldMatrix))
				{
					Bounds localBounds = skinnedMeshRenderer.localBounds;
					this.m_BoundsHandle.center = localBounds.center;
					this.m_BoundsHandle.size = localBounds.size;
					this.m_BoundsHandle.handleColor = ((EditMode.editMode != EditMode.SceneViewEditMode.Collider || !EditMode.IsOwner(this)) ? Color.clear : this.m_BoundsHandle.wireframeColor);
					EditorGUI.BeginChangeCheck();
					this.m_BoundsHandle.DrawHandle();
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(skinnedMeshRenderer, "Resize Bounds");
						skinnedMeshRenderer.localBounds = new Bounds(this.m_BoundsHandle.center, this.m_BoundsHandle.size);
					}
				}
			}
		}
	}
}
