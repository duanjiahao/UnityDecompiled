using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Sprites;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(PolygonCollider2D))]
	internal class PolygonCollider2DEditor : Collider2DEditorBase
	{
		private readonly PolygonEditorUtility m_PolyUtility = new PolygonEditorUtility();

		private SerializedProperty m_Points;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Points = base.serializedObject.FindProperty("m_Points");
			this.m_AutoTiling = base.serializedObject.FindProperty("m_AutoTiling");
			this.m_Points.isExpanded = false;
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
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
				base.BeginColliderInspector();
			}
			base.OnInspectorGUI();
			if (base.targets.Length == 1)
			{
				EditorGUI.BeginDisabledGroup(base.editingCollider);
				EditorGUILayout.PropertyField(this.m_Points, true, new GUILayoutOption[0]);
				EditorGUI.EndDisabledGroup();
			}
			base.EndColliderInspector();
			base.FinalizeInspectorGUI();
			EditorGUILayout.EndVertical();
			this.HandleDragAndDrop(GUILayoutUtility.GetLastRect());
		}

		private void HandleDragAndDrop(Rect targetRect)
		{
			if (Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragUpdated)
			{
				if (targetRect.Contains(Event.current.mousePosition))
				{
					using (IEnumerator<UnityEngine.Object> enumerator = (from obj in DragAndDrop.objectReferences
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
								foreach (PolygonCollider2D current2 in from target in base.targets
								select target as PolygonCollider2D)
								{
									Vector2[][] array;
									UnityEditor.Sprites.SpriteUtility.GenerateOutlineFromSprite(sprite, 0.25f, 200, true, out array);
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
			}
		}

		protected override void OnEditStart()
		{
			if (!(base.target == null))
			{
				this.m_PolyUtility.StartEditing(base.target as Collider2D);
			}
		}

		protected override void OnEditEnd()
		{
			this.m_PolyUtility.StopEditing();
		}

		public void OnSceneGUI()
		{
			if (base.editingCollider)
			{
				this.m_PolyUtility.OnSceneGUI();
			}
		}
	}
}
