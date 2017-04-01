using System;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(OcclusionPortal))]
	internal class OcclusionPortalEditor : Editor
	{
		private static readonly int s_HandleControlIDHint = typeof(OcclusionPortalEditor).Name.GetHashCode();

		private readonly BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle(OcclusionPortalEditor.s_HandleControlIDHint);

		private SerializedProperty m_Center;

		private SerializedProperty m_Size;

		protected virtual void OnEnable()
		{
			this.m_Center = base.serializedObject.FindProperty("m_Center");
			this.m_Size = base.serializedObject.FindProperty("m_Size");
			this.m_BoundsHandle.SetColor(Handles.s_ColliderHandleColor);
		}

		public override void OnInspectorGUI()
		{
			EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Collider, "Edit Bounds", PrimitiveBoundsHandle.editModeButton, this.GetWorldBounds(this.m_Center.vector3Value, this.m_Size.vector3Value), this);
			base.OnInspectorGUI();
		}

		protected virtual void OnSceneGUI()
		{
			if (EditMode.editMode == EditMode.SceneViewEditMode.Collider && EditMode.IsOwner(this))
			{
				OcclusionPortal occlusionPortal = base.target as OcclusionPortal;
				SerializedObject serializedObject = new SerializedObject(occlusionPortal);
				serializedObject.Update();
				using (new Handles.DrawingScope(occlusionPortal.transform.localToWorldMatrix))
				{
					SerializedProperty serializedProperty = serializedObject.FindProperty(this.m_Center.propertyPath);
					SerializedProperty serializedProperty2 = serializedObject.FindProperty(this.m_Size.propertyPath);
					this.m_BoundsHandle.center = serializedProperty.vector3Value;
					this.m_BoundsHandle.size = serializedProperty2.vector3Value;
					EditorGUI.BeginChangeCheck();
					this.m_BoundsHandle.DrawHandle();
					if (EditorGUI.EndChangeCheck())
					{
						serializedProperty.vector3Value = this.m_BoundsHandle.center;
						serializedProperty2.vector3Value = this.m_BoundsHandle.size;
						serializedObject.ApplyModifiedProperties();
					}
				}
			}
		}

		private Bounds GetWorldBounds(Vector3 center, Vector3 size)
		{
			Bounds bounds = new Bounds(center, size);
			Vector3 max = bounds.max;
			Vector3 min = bounds.min;
			Matrix4x4 localToWorldMatrix = ((OcclusionPortal)base.target).transform.localToWorldMatrix;
			Bounds result = new Bounds(localToWorldMatrix.MultiplyPoint3x4(new Vector3(max.x, max.y, max.z)), Vector3.zero);
			result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(max.x, max.y, max.z)));
			result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(max.x, max.y, min.z)));
			result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(max.x, min.y, max.z)));
			result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(min.x, max.y, max.z)));
			result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(max.x, min.y, min.z)));
			result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(min.x, max.y, min.z)));
			result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(min.x, min.y, max.z)));
			result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(min.x, min.y, min.z)));
			return result;
		}
	}
}
