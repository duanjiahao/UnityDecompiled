using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal class ShapeEditorSelection : IEnumerable<int>, IEnumerable
	{
		private HashSet<int> m_SelectedPoints = new HashSet<int>();

		private Vector2 m_SelectStartPoint;

		private Vector2 m_SelectMousePoint;

		private bool m_RectSelecting;

		private ShapeEditor m_ShapeEditor;

		private readonly int k_RectSelectionID = GUIUtility.GetPermanentControlID();

		private const float k_MinSelectionSize = 6f;

		public int Count
		{
			get
			{
				return this.m_SelectedPoints.Count;
			}
		}

		public bool isSelecting
		{
			get
			{
				return GUIUtility.hotControl == this.k_RectSelectionID;
			}
		}

		public HashSet<int> indices
		{
			get
			{
				return this.m_SelectedPoints;
			}
		}

		public ShapeEditorSelection(ShapeEditor owner)
		{
			this.m_ShapeEditor = owner;
		}

		public bool Contains(int i)
		{
			return this.m_SelectedPoints.Contains(i);
		}

		public void DeleteSelection()
		{
			IOrderedEnumerable<int> orderedEnumerable = from x in this.m_SelectedPoints
			orderby x descending
			select x;
			foreach (int current in orderedEnumerable)
			{
				this.m_ShapeEditor.RemovePointAt(current);
			}
			if (this.m_ShapeEditor.activePoint >= this.m_ShapeEditor.GetPointsCount())
			{
				this.m_ShapeEditor.activePoint = this.m_ShapeEditor.GetPointsCount() - 1;
			}
			this.m_SelectedPoints.Clear();
		}

		public void MoveSelection(Vector3 delta)
		{
			if (delta.sqrMagnitude >= 1.401298E-45f)
			{
				foreach (int current in this.m_SelectedPoints)
				{
					this.m_ShapeEditor.SetPointPosition(current, this.m_ShapeEditor.GetPointPosition(current) + delta);
				}
			}
		}

		public void Clear()
		{
			this.m_SelectedPoints.Clear();
			if (this.m_ShapeEditor != null)
			{
				this.m_ShapeEditor.activePoint = -1;
			}
		}

		public void SelectPoint(int i, ShapeEditor.SelectionType type)
		{
			switch (type)
			{
			case ShapeEditor.SelectionType.Normal:
				this.m_SelectedPoints.Clear();
				this.m_ShapeEditor.activePoint = i;
				this.m_SelectedPoints.Add(i);
				break;
			case ShapeEditor.SelectionType.Additive:
				this.m_ShapeEditor.activePoint = i;
				this.m_SelectedPoints.Add(i);
				break;
			case ShapeEditor.SelectionType.Subtractive:
				this.m_ShapeEditor.activePoint = ((i <= 0) ? 0 : (i - 1));
				this.m_SelectedPoints.Remove(i);
				break;
			default:
				this.m_ShapeEditor.activePoint = i;
				break;
			}
			this.m_ShapeEditor.Repaint();
		}

		public void RectSelect(Rect rect, ShapeEditor.SelectionType type)
		{
			if (type == ShapeEditor.SelectionType.Normal)
			{
				this.m_SelectedPoints.Clear();
				this.m_ShapeEditor.activePoint = -1;
				type = ShapeEditor.SelectionType.Additive;
			}
			for (int i = 0; i < this.m_ShapeEditor.GetPointsCount(); i++)
			{
				Vector2 point = this.m_ShapeEditor.LocalToScreen(this.m_ShapeEditor.GetPointPosition(i));
				if (rect.Contains(point))
				{
					this.SelectPoint(i, type);
				}
			}
			this.m_ShapeEditor.Repaint();
		}

		public void OnGUI()
		{
			Event current = Event.current;
			Handles.BeginGUI();
			Vector2 mousePosition = current.mousePosition;
			int num = this.k_RectSelectionID;
			switch (current.GetTypeForControl(num))
			{
			case EventType.MouseDown:
				if (HandleUtility.nearestControl == num && current.button == 0)
				{
					GUIUtility.hotControl = num;
					this.m_SelectStartPoint = mousePosition;
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == num && current.button == 0)
				{
					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;
					if (this.m_RectSelecting)
					{
						this.m_SelectMousePoint = new Vector2(mousePosition.x, mousePosition.y);
						ShapeEditor.SelectionType type = ShapeEditor.SelectionType.Normal;
						if (Event.current.control)
						{
							type = ShapeEditor.SelectionType.Subtractive;
						}
						else if (Event.current.shift)
						{
							type = ShapeEditor.SelectionType.Additive;
						}
						this.RectSelect(EditorGUIExt.FromToRect(this.m_SelectMousePoint, this.m_SelectStartPoint), type);
						this.m_RectSelecting = false;
						current.Use();
					}
					else
					{
						this.m_SelectedPoints.Clear();
						this.m_ShapeEditor.activePoint = -1;
						this.m_ShapeEditor.Repaint();
						current.Use();
					}
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == num)
				{
					if (!this.m_RectSelecting && (mousePosition - this.m_SelectStartPoint).magnitude > 6f)
					{
						this.m_RectSelecting = true;
					}
					if (this.m_RectSelecting)
					{
						this.m_SelectMousePoint = new Vector2(mousePosition.x, mousePosition.y);
						ShapeEditor.SelectionType type2 = ShapeEditor.SelectionType.Normal;
						if (Event.current.control)
						{
							type2 = ShapeEditor.SelectionType.Subtractive;
						}
						else if (Event.current.shift)
						{
							type2 = ShapeEditor.SelectionType.Additive;
						}
						this.RectSelect(EditorGUIExt.FromToRect(this.m_SelectMousePoint, this.m_SelectStartPoint), type2);
					}
					current.Use();
				}
				break;
			case EventType.Repaint:
				if (GUIUtility.hotControl == num && this.m_RectSelecting)
				{
					EditorStyles.selectionRect.Draw(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint), GUIContent.none, false, false, false, false);
				}
				break;
			case EventType.Layout:
				if (!Tools.viewToolActive)
				{
					HandleUtility.AddDefaultControl(num);
				}
				break;
			}
			Handles.EndGUI();
		}

		public IEnumerator<int> GetEnumerator()
		{
			return this.m_SelectedPoints.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
