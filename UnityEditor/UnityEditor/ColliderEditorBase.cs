using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class ColliderEditorBase : Editor
	{
		public bool editingCollider
		{
			get
			{
				return EditMode.editMode == EditMode.SceneViewEditMode.Collider && EditMode.IsOwner(this);
			}
		}

		protected virtual void OnEditStart()
		{
		}

		protected virtual void OnEditEnd()
		{
		}

		public virtual void OnEnable()
		{
			EditMode.onEditModeStartDelegate = (EditMode.OnEditModeStartFunc)Delegate.Combine(EditMode.onEditModeStartDelegate, new EditMode.OnEditModeStartFunc(this.OnEditModeStart));
		}

		public virtual void OnDisable()
		{
			EditMode.onEditModeEndDelegate = (EditMode.OnEditModeStopFunc)Delegate.Remove(EditMode.onEditModeEndDelegate, new EditMode.OnEditModeStopFunc(this.OnEditModeEnd));
		}

		protected void ForceQuitEditMode()
		{
			EditMode.QuitEditMode();
		}

		protected void InspectorEditButtonGUI()
		{
			EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Collider, "Edit Collider", EditorGUIUtility.IconContent("EditCollider"), ColliderEditorBase.GetColliderBounds(base.target), this);
		}

		private static Bounds GetColliderBounds(UnityEngine.Object collider)
		{
			Bounds result;
			if (collider is Collider2D)
			{
				result = (collider as Collider2D).bounds;
			}
			else if (collider is Collider)
			{
				result = (collider as Collider).bounds;
			}
			else
			{
				result = default(Bounds);
			}
			return result;
		}

		protected void OnEditModeStart(Editor editor, EditMode.SceneViewEditMode mode)
		{
			if (mode == EditMode.SceneViewEditMode.Collider && editor == this)
			{
				this.OnEditStart();
			}
		}

		protected void OnEditModeEnd(Editor editor)
		{
			if (editor == this)
			{
				this.OnEditEnd();
			}
		}
	}
}
