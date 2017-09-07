using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(CapsuleCollider2D))]
	internal class CapsuleCollider2DEditor : PrimitiveCollider2DEditor
	{
		private SerializedProperty m_Size;

		private SerializedProperty m_Direction;

		private readonly CapsuleBoundsHandle m_BoundsHandle = new CapsuleBoundsHandle();

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
			this.m_Size = base.serializedObject.FindProperty("m_Size");
			this.m_Direction = base.serializedObject.FindProperty("m_Direction");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.InspectorEditButtonGUI();
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Direction, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
			base.FinalizeInspectorGUI();
		}

		protected override void CopyColliderSizeToHandle()
		{
			CapsuleCollider2D collider = (CapsuleCollider2D)base.target;
			Vector3 vector;
			Vector3 vector2;
			this.GetHandleVectorsInWorldSpace(collider, out vector, out vector2);
			CapsuleBoundsHandle arg_31_0 = this.m_BoundsHandle;
			float num = 0f;
			this.m_BoundsHandle.radius = num;
			arg_31_0.height = num;
			this.m_BoundsHandle.height = vector.magnitude;
			this.m_BoundsHandle.radius = vector2.magnitude * 0.5f;
		}

		protected override bool CopyHandleSizeToCollider()
		{
			CapsuleCollider2D capsuleCollider2D = (CapsuleCollider2D)base.target;
			Vector3 vector;
			Vector3 vector2;
			if (capsuleCollider2D.direction == CapsuleDirection2D.Horizontal)
			{
				vector = Vector3.up;
				vector2 = Vector3.right;
			}
			else
			{
				vector = Vector3.right;
				vector2 = Vector3.up;
			}
			Vector3 vector3 = Handles.matrix * (vector2 * this.m_BoundsHandle.height);
			Vector3 vector4 = Handles.matrix * (vector * this.m_BoundsHandle.radius * 2f);
			Matrix4x4 localToWorldMatrix = capsuleCollider2D.transform.localToWorldMatrix;
			Vector3 vector5 = base.ProjectOntoWorldPlane(localToWorldMatrix * vector).normalized * vector4.magnitude;
			Vector3 vector6 = base.ProjectOntoWorldPlane(localToWorldMatrix * vector2).normalized * vector3.magnitude;
			vector5 = base.ProjectOntoColliderPlane(vector5, localToWorldMatrix);
			vector6 = base.ProjectOntoColliderPlane(vector6, localToWorldMatrix);
			Vector2 size = capsuleCollider2D.size;
			capsuleCollider2D.size = localToWorldMatrix.inverse * (vector5 + vector6);
			return capsuleCollider2D.size != size;
		}

		protected override Quaternion GetHandleRotation()
		{
			Vector3 upwards;
			Vector3 vector;
			this.GetHandleVectorsInWorldSpace(base.target as CapsuleCollider2D, out upwards, out vector);
			return Quaternion.LookRotation(Vector3.forward, upwards);
		}

		private void GetHandleVectorsInWorldSpace(CapsuleCollider2D collider, out Vector3 handleHeightVector, out Vector3 handleDiameterVector)
		{
			Matrix4x4 localToWorldMatrix = collider.transform.localToWorldMatrix;
			Vector3 vector = base.ProjectOntoWorldPlane(localToWorldMatrix * (Vector3.right * collider.size.x));
			Vector3 vector2 = base.ProjectOntoWorldPlane(localToWorldMatrix * (Vector3.up * collider.size.y));
			if (collider.direction == CapsuleDirection2D.Horizontal)
			{
				handleDiameterVector = vector2;
				handleHeightVector = vector;
			}
			else
			{
				handleDiameterVector = vector;
				handleHeightVector = vector2;
			}
		}
	}
}
