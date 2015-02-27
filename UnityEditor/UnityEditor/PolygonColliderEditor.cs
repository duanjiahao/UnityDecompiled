using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Sprites;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(PolygonCollider2D))]
	internal class PolygonColliderEditor : Collider2DEditorBase
	{
		private PolygonEditorUtility m_PolyUtility = new PolygonEditorUtility();
		private bool m_ShowColliderInfo;
		public override void OnEnable()
		{
			base.OnEnable();
		}
		public override void OnInspectorGUI()
		{
			this.HandleDragAndDrop();
			base.BeginColliderInspector();
			this.ColliderInfoGUI();
			base.EndColliderInspector();
			base.CheckColliderErrorState();
		}
		private void HandleDragAndDrop()
		{
			if (Event.current.type != EventType.DragPerform && Event.current.type != EventType.DragUpdated)
			{
				return;
			}
			using (IEnumerator<UnityEngine.Object> enumerator = (
				from obj in DragAndDrop.objectReferences
				where obj is Sprite || obj is Texture2D
				select obj).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					UnityEngine.Object current = enumerator.Current;
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					if (Event.current.type == EventType.DragPerform)
					{
						Sprite sprite = (!(current is Sprite)) ? SpriteUtility.TextureToSprite(current as Texture2D) : (current as Sprite);
						foreach (PolygonCollider2D current2 in 
							from target in base.targets
							select target as PolygonCollider2D)
						{
							Vector2[][] array;
							DataUtility.GenerateOutlineFromSprite(sprite, 0.25f, 200, true, out array);
							current2.pathCount = array.Length;
							for (int i = 0; i < array.Length; i++)
							{
								current2.SetPath(i, array[i]);
							}
							this.m_PolyUtility.StopEditing();
							DragAndDrop.AcceptDrag();
						}
					}
					return;
				}
			}
			DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
		}
		private void ColliderInfoGUI()
		{
			EditorGUI.BeginDisabledGroup(base.targets.Length != 1);
			this.m_ShowColliderInfo = EditorGUILayout.Foldout(this.m_ShowColliderInfo, "Collider Info");
			if (this.m_ShowColliderInfo)
			{
				PolygonCollider2D polygonCollider2D = base.targets[0] as PolygonCollider2D;
				if (polygonCollider2D)
				{
					int totalPointCount = polygonCollider2D.GetTotalPointCount();
					string label = (!GUI.enabled) ? "---" : (string.Empty + totalPointCount);
					EditorGUI.indentLevel++;
					EditorGUILayout.LabelField("Vertices", label, new GUILayoutOption[0]);
					EditorGUI.indentLevel--;
				}
			}
			EditorGUI.EndDisabledGroup();
		}
		protected override void OnEditStart()
		{
			if (this.target == null)
			{
				return;
			}
			this.m_PolyUtility.StartEditing(this.target as Collider2D);
		}
		protected override void OnEditEnd()
		{
			this.m_PolyUtility.StopEditing();
		}
		public void OnSceneGUI()
		{
			if (!base.editingCollider)
			{
				return;
			}
			this.m_PolyUtility.OnSceneGUI();
		}
	}
}
