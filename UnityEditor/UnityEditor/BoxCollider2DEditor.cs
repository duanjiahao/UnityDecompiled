using System;
using UnityEditor.AnimatedValues;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(BoxCollider2D))]
	internal class BoxCollider2DEditor : Collider2DEditorBase
	{
		private SerializedProperty m_Size;

		private SerializedProperty m_EdgeRadius;

		private SerializedProperty m_UsedByComposite;

		private readonly AnimBool m_ShowCompositeRedundants = new AnimBool();

		private readonly BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();

		protected override GUIContent editModeButton
		{
			get
			{
				return PrimitiveBoundsHandle.editModeButton;
			}
		}

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Size = base.serializedObject.FindProperty("m_Size");
			this.m_EdgeRadius = base.serializedObject.FindProperty("m_EdgeRadius");
			this.m_BoundsHandle.axes = (PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y);
			this.m_UsedByComposite = base.serializedObject.FindProperty("m_UsedByComposite");
			this.m_AutoTiling = base.serializedObject.FindProperty("m_AutoTiling");
			this.m_ShowCompositeRedundants.value = !this.m_UsedByComposite.boolValue;
			this.m_ShowCompositeRedundants.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		public override void OnDisable()
		{
			base.OnDisable();
			this.m_ShowCompositeRedundants.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			bool flag = !base.CanEditCollider();
			if (flag)
			{
				EditorGUILayout.HelpBox(Collider2DEditorBase.Styles.s_ColliderEditDisableHelp.text, MessageType.Info);
				if (base.editingCollider)
				{
					EditMode.QuitEditMode();
				}
			}
			else
			{
				base.InspectorEditButtonGUI();
			}
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
			this.m_ShowCompositeRedundants.target = !this.m_UsedByComposite.boolValue;
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowCompositeRedundants.faded))
			{
				EditorGUILayout.PropertyField(this.m_EdgeRadius, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			base.serializedObject.ApplyModifiedProperties();
			base.FinalizeInspectorGUI();
		}

		protected virtual void OnSceneGUI()
		{
			if (base.editingCollider)
			{
				BoxCollider2D boxCollider2D = (BoxCollider2D)base.target;
				if (!Mathf.Approximately(boxCollider2D.transform.lossyScale.sqrMagnitude, 0f))
				{
					Matrix4x4 matrix4x = boxCollider2D.transform.localToWorldMatrix;
					matrix4x.SetRow(0, Vector4.Scale(matrix4x.GetRow(0), new Vector4(1f, 1f, 0f, 1f)));
					matrix4x.SetRow(1, Vector4.Scale(matrix4x.GetRow(1), new Vector4(1f, 1f, 0f, 1f)));
					matrix4x.SetRow(2, new Vector4(0f, 0f, 1f, boxCollider2D.transform.position.z));
					if (boxCollider2D.usedByComposite && boxCollider2D.composite != null)
					{
						Vector3 pos = boxCollider2D.composite.transform.rotation * boxCollider2D.composite.offset;
						pos.z = 0f;
						matrix4x = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one) * matrix4x;
					}
					using (new Handles.DrawingScope(matrix4x))
					{
						this.m_BoundsHandle.center = boxCollider2D.offset;
						this.m_BoundsHandle.size = boxCollider2D.size;
						this.m_BoundsHandle.SetColor((!boxCollider2D.enabled) ? Handles.s_ColliderHandleColorDisabled : Handles.s_ColliderHandleColor);
						EditorGUI.BeginChangeCheck();
						this.m_BoundsHandle.DrawHandle();
						if (EditorGUI.EndChangeCheck())
						{
							Undo.RecordObject(boxCollider2D, string.Format("Modify {0}", ObjectNames.NicifyVariableName(base.target.GetType().Name)));
							Vector2 size = boxCollider2D.size;
							boxCollider2D.size = this.m_BoundsHandle.size;
							if (boxCollider2D.size != size)
							{
								boxCollider2D.offset = this.m_BoundsHandle.center;
							}
						}
					}
				}
			}
		}
	}
}
