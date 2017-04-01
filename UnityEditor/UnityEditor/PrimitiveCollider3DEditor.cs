using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	internal abstract class PrimitiveCollider3DEditor : Collider3DEditorBase
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

		protected abstract void CopyColliderPropertiesToHandle();

		protected abstract void CopyHandlePropertiesToCollider();

		protected Vector3 InvertScaleVector(Vector3 scaleVector)
		{
			for (int i = 0; i < 3; i++)
			{
				scaleVector[i] = ((scaleVector[i] != 0f) ? (1f / scaleVector[i]) : 0f);
			}
			return scaleVector;
		}

		protected virtual void OnSceneGUI()
		{
			if (base.editingCollider)
			{
				Collider collider = (Collider)base.target;
				if (!Mathf.Approximately(collider.transform.lossyScale.sqrMagnitude, 0f))
				{
					using (new Handles.DrawingScope(Matrix4x4.TRS(collider.transform.position, collider.transform.rotation, Vector3.one)))
					{
						this.CopyColliderPropertiesToHandle();
						this.boundsHandle.SetColor((!collider.enabled) ? Handles.s_ColliderHandleColorDisabled : Handles.s_ColliderHandleColor);
						EditorGUI.BeginChangeCheck();
						this.boundsHandle.DrawHandle();
						if (EditorGUI.EndChangeCheck())
						{
							Undo.RecordObject(collider, string.Format("Modify {0}", ObjectNames.NicifyVariableName(base.target.GetType().Name)));
							this.CopyHandlePropertiesToCollider();
						}
					}
				}
			}
		}

		protected Vector3 TransformColliderCenterToHandleSpace(Transform colliderTransform, Vector3 colliderCenter)
		{
			return Handles.inverseMatrix * (colliderTransform.localToWorldMatrix * colliderCenter);
		}

		protected Vector3 TransformHandleCenterToColliderSpace(Transform colliderTransform, Vector3 handleCenter)
		{
			return colliderTransform.localToWorldMatrix.inverse * (Handles.matrix * handleCenter);
		}
	}
}
