using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	internal abstract class PrimitiveCollider2DEditor : Collider2DEditorBase
	{
		protected abstract PrimitiveBoundsHandle boundsHandle
		{
			get;
		}

		protected override GUIContent editModeButton
		{
			get
			{
				return PrimitiveBoundsHandle.editModeButton;
			}
		}

		protected abstract void CopyColliderSizeToHandle();

		protected abstract bool CopyHandleSizeToCollider();

		protected virtual Quaternion GetHandleRotation()
		{
			return Quaternion.identity;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			this.boundsHandle.axes = (PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y);
		}

		protected virtual void OnSceneGUI()
		{
			if (base.editingCollider)
			{
				Collider2D collider2D = (Collider2D)base.target;
				if (!Mathf.Approximately(collider2D.transform.lossyScale.sqrMagnitude, 0f))
				{
					using (new Handles.DrawingScope(Matrix4x4.TRS(collider2D.transform.position, this.GetHandleRotation(), Vector3.one)))
					{
						Matrix4x4 localToWorldMatrix = collider2D.transform.localToWorldMatrix;
						this.boundsHandle.center = this.ProjectOntoWorldPlane(Handles.inverseMatrix * (localToWorldMatrix * collider2D.offset));
						this.CopyColliderSizeToHandle();
						this.boundsHandle.SetColor((!collider2D.enabled) ? Handles.s_ColliderHandleColorDisabled : Handles.s_ColliderHandleColor);
						EditorGUI.BeginChangeCheck();
						this.boundsHandle.DrawHandle();
						if (EditorGUI.EndChangeCheck())
						{
							Undo.RecordObject(collider2D, string.Format("Modify {0}", ObjectNames.NicifyVariableName(base.target.GetType().Name)));
							if (this.CopyHandleSizeToCollider())
							{
								collider2D.offset = localToWorldMatrix.inverse * this.ProjectOntoColliderPlane(Handles.matrix * this.boundsHandle.center, localToWorldMatrix);
							}
						}
					}
				}
			}
		}

		protected Vector3 ProjectOntoColliderPlane(Vector3 worldVector, Matrix4x4 colliderTransformMatrix)
		{
			Plane plane = new Plane(Vector3.Cross(colliderTransformMatrix * Vector3.right, colliderTransformMatrix * Vector3.up), Vector3.zero);
			Ray ray = new Ray(worldVector, Vector3.forward);
			float distance;
			Vector3 point;
			if (plane.Raycast(ray, out distance))
			{
				point = ray.GetPoint(distance);
			}
			else
			{
				ray.direction = Vector3.back;
				plane.Raycast(ray, out distance);
				point = ray.GetPoint(distance);
			}
			return point;
		}

		protected Vector3 ProjectOntoWorldPlane(Vector3 worldVector)
		{
			worldVector.z = 0f;
			return worldVector;
		}
	}
}
