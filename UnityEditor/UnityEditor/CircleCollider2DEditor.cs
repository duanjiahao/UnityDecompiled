using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(CircleCollider2D))]
	internal class CircleCollider2DEditor : PrimitiveCollider2DEditor
	{
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
			this.m_Radius = base.serializedObject.FindProperty("m_Radius");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.InspectorEditButtonGUI();
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
			base.FinalizeInspectorGUI();
		}

		protected override void CopyColliderSizeToHandle()
		{
			CircleCollider2D circleCollider2D = (CircleCollider2D)base.target;
			this.m_BoundsHandle.radius = circleCollider2D.radius * this.GetRadiusScaleFactor();
		}

		protected override bool CopyHandleSizeToCollider()
		{
			CircleCollider2D circleCollider2D = (CircleCollider2D)base.target;
			float radius = circleCollider2D.radius;
			float radiusScaleFactor = this.GetRadiusScaleFactor();
			circleCollider2D.radius = ((!Mathf.Approximately(radiusScaleFactor, 0f)) ? (this.m_BoundsHandle.radius / this.GetRadiusScaleFactor()) : 0f);
			return circleCollider2D.radius != radius;
		}

		private float GetRadiusScaleFactor()
		{
			Vector3 lossyScale = ((Component)base.target).transform.lossyScale;
			return Mathf.Max(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y));
		}
	}
}
