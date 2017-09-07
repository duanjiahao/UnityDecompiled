using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SphereCollider))]
	internal class SphereColliderEditor : PrimitiveCollider3DEditor
	{
		private SerializedProperty m_Center;

		private SerializedProperty m_Radius;

		private readonly SphereBoundsHandle m_BoundsHandle = new SphereBoundsHandle();

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
			this.m_Radius = base.serializedObject.FindProperty("m_Radius");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.InspectorEditButtonGUI();
			EditorGUILayout.PropertyField(this.m_IsTrigger, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		protected override void CopyColliderPropertiesToHandle()
		{
			SphereCollider sphereCollider = (SphereCollider)base.target;
			this.m_BoundsHandle.center = base.TransformColliderCenterToHandleSpace(sphereCollider.transform, sphereCollider.center);
			this.m_BoundsHandle.radius = sphereCollider.radius * this.GetRadiusScaleFactor();
		}

		protected override void CopyHandlePropertiesToCollider()
		{
			SphereCollider sphereCollider = (SphereCollider)base.target;
			sphereCollider.center = base.TransformHandleCenterToColliderSpace(sphereCollider.transform, this.m_BoundsHandle.center);
			float radiusScaleFactor = this.GetRadiusScaleFactor();
			sphereCollider.radius = ((!Mathf.Approximately(radiusScaleFactor, 0f)) ? (this.m_BoundsHandle.radius / this.GetRadiusScaleFactor()) : 0f);
		}

		private float GetRadiusScaleFactor()
		{
			float num = 0f;
			Vector3 lossyScale = ((SphereCollider)base.target).transform.lossyScale;
			for (int i = 0; i < 3; i++)
			{
				num = Mathf.Max(num, Mathf.Abs(lossyScale[i]));
			}
			return num;
		}
	}
}
