using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(CapsuleCollider))]
	internal class CapsuleColliderEditor : PrimitiveCollider3DEditor
	{
		private SerializedProperty m_Center;

		private SerializedProperty m_Radius;

		private SerializedProperty m_Height;

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
			this.m_Center = base.serializedObject.FindProperty("m_Center");
			this.m_Radius = base.serializedObject.FindProperty("m_Radius");
			this.m_Height = base.serializedObject.FindProperty("m_Height");
			this.m_Direction = base.serializedObject.FindProperty("m_Direction");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.InspectorEditButtonGUI();
			EditorGUILayout.PropertyField(this.m_IsTrigger, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Height, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Direction, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		protected override void CopyColliderPropertiesToHandle()
		{
			CapsuleCollider capsuleCollider = (CapsuleCollider)base.target;
			this.m_BoundsHandle.center = base.TransformColliderCenterToHandleSpace(capsuleCollider.transform, capsuleCollider.center);
			float num;
			Vector3 capsuleColliderHandleScale = this.GetCapsuleColliderHandleScale(capsuleCollider.transform.lossyScale, capsuleCollider.direction, out num);
			CapsuleBoundsHandle arg_5D_0 = this.m_BoundsHandle;
			float num2 = 0f;
			this.m_BoundsHandle.radius = num2;
			arg_5D_0.height = num2;
			this.m_BoundsHandle.height = capsuleCollider.height * Mathf.Abs(capsuleColliderHandleScale[capsuleCollider.direction]);
			this.m_BoundsHandle.radius = capsuleCollider.radius * num;
			int direction = capsuleCollider.direction;
			if (direction != 0)
			{
				if (direction != 1)
				{
					if (direction == 2)
					{
						this.m_BoundsHandle.heightAxis = CapsuleBoundsHandle.HeightAxis.Z;
					}
				}
				else
				{
					this.m_BoundsHandle.heightAxis = CapsuleBoundsHandle.HeightAxis.Y;
				}
			}
			else
			{
				this.m_BoundsHandle.heightAxis = CapsuleBoundsHandle.HeightAxis.X;
			}
		}

		protected override void CopyHandlePropertiesToCollider()
		{
			CapsuleCollider capsuleCollider = (CapsuleCollider)base.target;
			capsuleCollider.center = base.TransformHandleCenterToColliderSpace(capsuleCollider.transform, this.m_BoundsHandle.center);
			float num;
			Vector3 scaleVector = this.GetCapsuleColliderHandleScale(capsuleCollider.transform.lossyScale, capsuleCollider.direction, out num);
			scaleVector = base.InvertScaleVector(scaleVector);
			if (num != 0f)
			{
				capsuleCollider.radius = this.m_BoundsHandle.radius / num;
			}
			if (scaleVector[capsuleCollider.direction] != 0f)
			{
				capsuleCollider.height = this.m_BoundsHandle.height * Mathf.Abs(scaleVector[capsuleCollider.direction]);
			}
		}

		protected override void OnSceneGUI()
		{
			CapsuleCollider capsuleCollider = (CapsuleCollider)base.target;
			float num;
			this.GetCapsuleColliderHandleScale(capsuleCollider.transform.lossyScale, capsuleCollider.direction, out num);
			this.boundsHandle.axes = PrimitiveBoundsHandle.Axes.All;
			if (num == 0f)
			{
				int direction = capsuleCollider.direction;
				if (direction != 0)
				{
					if (direction != 1)
					{
						if (direction == 2)
						{
							this.boundsHandle.axes = PrimitiveBoundsHandle.Axes.Z;
						}
					}
					else
					{
						this.boundsHandle.axes = PrimitiveBoundsHandle.Axes.Y;
					}
				}
				else
				{
					this.boundsHandle.axes = PrimitiveBoundsHandle.Axes.X;
				}
			}
			base.OnSceneGUI();
		}

		private Vector3 GetCapsuleColliderHandleScale(Vector3 lossyScale, int capsuleDirection, out float radiusScaleFactor)
		{
			radiusScaleFactor = 0f;
			for (int i = 0; i < 3; i++)
			{
				if (i != capsuleDirection)
				{
					radiusScaleFactor = Mathf.Max(radiusScaleFactor, Mathf.Abs(lossyScale[i]));
				}
			}
			for (int j = 0; j < 3; j++)
			{
				if (j != capsuleDirection)
				{
					lossyScale[j] = Mathf.Sign(lossyScale[j]) * radiusScaleFactor;
				}
			}
			return lossyScale;
		}
	}
}
