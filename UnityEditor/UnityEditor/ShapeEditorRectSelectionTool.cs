using System;
using System.Threading;
using UnityEngine;

namespace UnityEditor
{
	internal class ShapeEditorRectSelectionTool
	{
		private Vector2 m_SelectStartPoint;

		private Vector2 m_SelectMousePoint;

		private bool m_RectSelecting;

		private int m_RectSelectionID;

		private const float k_MinSelectionSize = 6f;

		public event Action<Rect, ShapeEditor.SelectionType> RectSelect
		{
			add
			{
				Action<Rect, ShapeEditor.SelectionType> action = this.RectSelect;
				Action<Rect, ShapeEditor.SelectionType> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<Rect, ShapeEditor.SelectionType>>(ref this.RectSelect, (Action<Rect, ShapeEditor.SelectionType>)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action<Rect, ShapeEditor.SelectionType> action = this.RectSelect;
				Action<Rect, ShapeEditor.SelectionType> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<Rect, ShapeEditor.SelectionType>>(ref this.RectSelect, (Action<Rect, ShapeEditor.SelectionType>)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public event Action ClearSelection
		{
			add
			{
				Action action = this.ClearSelection;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref this.ClearSelection, (Action)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action action = this.ClearSelection;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref this.ClearSelection, (Action)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public bool isSelecting
		{
			get
			{
				return this.guiUtility.hotControl == this.m_RectSelectionID;
			}
		}

		private IGUIUtility guiUtility
		{
			get;
			set;
		}

		public ShapeEditorRectSelectionTool(IGUIUtility gu)
		{
			this.RectSelect = delegate(Rect i, ShapeEditor.SelectionType p)
			{
			};
			this.ClearSelection = delegate
			{
			};
			base..ctor();
			this.guiUtility = gu;
			this.m_RectSelectionID = this.guiUtility.GetPermanentControlID();
		}

		public void OnGUI()
		{
			Event current = Event.current;
			Handles.BeginGUI();
			Vector2 mousePosition = current.mousePosition;
			int rectSelectionID = this.m_RectSelectionID;
			switch (current.GetTypeForControl(rectSelectionID))
			{
			case EventType.MouseDown:
				if (HandleUtility.nearestControl == rectSelectionID && current.button == 0)
				{
					this.guiUtility.hotControl = rectSelectionID;
					this.m_SelectStartPoint = mousePosition;
				}
				break;
			case EventType.MouseUp:
				if (this.guiUtility.hotControl == rectSelectionID && current.button == 0)
				{
					this.guiUtility.hotControl = 0;
					this.guiUtility.keyboardControl = 0;
					if (this.m_RectSelecting)
					{
						this.m_SelectMousePoint = new Vector2(mousePosition.x, mousePosition.y);
						ShapeEditor.SelectionType arg = ShapeEditor.SelectionType.Normal;
						if (Event.current.control)
						{
							arg = ShapeEditor.SelectionType.Subtractive;
						}
						else if (Event.current.shift)
						{
							arg = ShapeEditor.SelectionType.Additive;
						}
						this.RectSelect(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint), arg);
						this.m_RectSelecting = false;
					}
					else
					{
						this.ClearSelection();
					}
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (this.guiUtility.hotControl == rectSelectionID)
				{
					if (!this.m_RectSelecting && (mousePosition - this.m_SelectStartPoint).magnitude > 6f)
					{
						this.m_RectSelecting = true;
					}
					if (this.m_RectSelecting)
					{
						this.m_SelectMousePoint = mousePosition;
						ShapeEditor.SelectionType arg2 = ShapeEditor.SelectionType.Normal;
						if (Event.current.control)
						{
							arg2 = ShapeEditor.SelectionType.Subtractive;
						}
						else if (Event.current.shift)
						{
							arg2 = ShapeEditor.SelectionType.Additive;
						}
						this.RectSelect(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint), arg2);
					}
					current.Use();
				}
				break;
			case EventType.Repaint:
				if (this.guiUtility.hotControl == rectSelectionID && this.m_RectSelecting)
				{
					EditorStyles.selectionRect.Draw(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint), GUIContent.none, false, false, false, false);
				}
				break;
			case EventType.Layout:
				if (!Tools.viewToolActive)
				{
					HandleUtility.AddDefaultControl(rectSelectionID);
				}
				break;
			}
			Handles.EndGUI();
		}
	}
}
