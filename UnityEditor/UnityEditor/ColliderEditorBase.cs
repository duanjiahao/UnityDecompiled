using System;
using UnityEngine;
namespace UnityEditor
{
	internal class ColliderEditorBase : Editor
	{
		private const float k_EditColliderbuttonWidth = 22f;
		private const float k_EditColliderbuttonHeight = 22f;
		private const float k_SpaceBetweenLabelAndButton = 5f;
		protected static ColliderEditorBase s_ActiveColliderEditor;
		private static GUIStyle s_EditColliderButtonStyle;
		private Tool m_PreviousTool;
		private bool m_EditingCollider;
		public bool editingCollider
		{
			get
			{
				if (Tools.current != Tool.None && this.m_EditingCollider)
				{
					this.editingCollider = false;
				}
				return this.m_EditingCollider;
			}
			set
			{
				if (!this.m_EditingCollider && value)
				{
					if (ColliderEditorBase.s_ActiveColliderEditor != null)
					{
						ColliderEditorBase.s_ActiveColliderEditor.editingCollider = false;
					}
					ColliderEditorBase.s_ActiveColliderEditor = this;
					this.m_PreviousTool = Tools.current;
					Tools.current = Tool.None;
					this.OnEditStart();
				}
				else
				{
					if (this.m_EditingCollider && !value)
					{
						Tools.current = ((this.m_PreviousTool == Tool.None) ? Tool.Move : this.m_PreviousTool);
						ColliderEditorBase.s_ActiveColliderEditor = null;
						this.OnEditEnd();
					}
				}
				this.m_EditingCollider = value;
			}
		}
		protected virtual void OnEditStart()
		{
		}
		protected virtual void OnEditEnd()
		{
		}
		protected void InspectorEditButtonGUI()
		{
			if (ColliderEditorBase.s_EditColliderButtonStyle == null)
			{
				ColliderEditorBase.s_EditColliderButtonStyle = new GUIStyle("Button");
				ColliderEditorBase.s_EditColliderButtonStyle.padding = new RectOffset(0, 0, 0, 0);
				ColliderEditorBase.s_EditColliderButtonStyle.margin = new RectOffset(0, 0, 0, 0);
			}
			Rect controlRect = EditorGUILayout.GetControlRect(true, 22f, new GUILayoutOption[0]);
			Rect position = new Rect(controlRect.xMin + EditorGUIUtility.labelWidth, controlRect.yMin, 22f, 22f);
			GUIContent content = new GUIContent("Edit Collider");
			Vector2 vector = GUI.skin.label.CalcSize(content);
			Rect position2 = new Rect(position.xMax + 5f, controlRect.yMin + (controlRect.height - vector.y) * 0.5f, vector.x, controlRect.height);
			EditorGUI.BeginChangeCheck();
			this.editingCollider = GUI.Toggle(position, this.editingCollider, EditorGUIUtility.IconContent("EditCollider"), ColliderEditorBase.s_EditColliderButtonStyle);
			GUI.Label(position2, "Edit Collider");
			if (EditorGUI.EndChangeCheck())
			{
				this.EditModeChanged();
			}
		}
		private void EditModeChanged()
		{
			if (this.editingCollider && SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null && !ColliderEditorBase.ColliderSeenByCamera(SceneView.lastActiveSceneView.camera, this.target))
			{
				SceneView.lastActiveSceneView.Frame(ColliderEditorBase.GetColliderBounds(this.target));
			}
			SceneView.RepaintAll();
		}
		private static bool ColliderSeenByCamera(Camera camera, UnityEngine.Object collider)
		{
			return ColliderEditorBase.AnyPointSeenByCamera(camera, ColliderEditorBase.GetColliderVertices(collider));
		}
		private static Vector3[] GetColliderVertices(UnityEngine.Object collider)
		{
			return ColliderEditorBase.BoundsToVertices(ColliderEditorBase.GetColliderBounds(collider));
		}
		private static Bounds GetColliderBounds(UnityEngine.Object collider)
		{
			if (collider is Collider2D)
			{
				return (collider as Collider2D).bounds;
			}
			if (collider is Collider)
			{
				return (collider as Collider).bounds;
			}
			return default(Bounds);
		}
		private static Vector3[] BoundsToVertices(Bounds bounds)
		{
			return new Vector3[]
			{
				new Vector3(bounds.min.x, bounds.min.y, bounds.min.z),
				new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
				new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
				new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
				new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
				new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
				new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
				new Vector3(bounds.max.x, bounds.max.y, bounds.max.z)
			};
		}
		private static bool AnyPointSeenByCamera(Camera camera, Vector3[] points)
		{
			for (int i = 0; i < points.Length; i++)
			{
				Vector3 point = points[i];
				if (ColliderEditorBase.PointSeenByCamera(camera, point))
				{
					return true;
				}
			}
			return false;
		}
		private static bool PointSeenByCamera(Camera camera, Vector3 point)
		{
			Vector3 vector = camera.WorldToViewportPoint(point);
			return vector.x > 0f && vector.x < 1f && vector.y > 0f && vector.y < 1f;
		}
	}
}
