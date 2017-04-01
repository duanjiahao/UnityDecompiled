using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(BoxCollider))]
	internal class BoxColliderEditor : PrimitiveCollider3DEditor
	{
		private static readonly int s_HandleControlIDHint = typeof(BoxColliderEditor).Name.GetHashCode();

		private SerializedProperty m_Center;

		private SerializedProperty m_Size;

		private readonly BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle(BoxColliderEditor.s_HandleControlIDHint);

		protected override PrimitiveBoundsHandle boundsHandle
		{
			get
			{
				return this.m_BoundsHandle;
			}
		}

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Center = base.serializedObject.FindProperty("m_Center");
			this.m_Size = base.serializedObject.FindProperty("m_Size");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.InspectorEditButtonGUI();
			EditorGUILayout.PropertyField(this.m_IsTrigger, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		protected override void CopyColliderPropertiesToHandle()
		{
			BoxCollider boxCollider = (BoxCollider)base.target;
			this.m_BoundsHandle.center = base.TransformColliderCenterToHandleSpace(boxCollider.transform, boxCollider.center);
			this.m_BoundsHandle.size = Vector3.Scale(boxCollider.size, boxCollider.transform.lossyScale);
		}

		protected override void CopyHandlePropertiesToCollider()
		{
			BoxCollider boxCollider = (BoxCollider)base.target;
			boxCollider.center = base.TransformHandleCenterToColliderSpace(boxCollider.transform, this.m_BoundsHandle.center);
			Vector3 size = Vector3.Scale(this.m_BoundsHandle.size, base.InvertScaleVector(boxCollider.transform.lossyScale));
			size = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));
			boxCollider.size = size;
		}
	}
}
