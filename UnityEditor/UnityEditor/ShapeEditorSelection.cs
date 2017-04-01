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

		private ShapeEditor m_ShapeEditor;

		public int Count
		{
			get
			{
				return this.m_SelectedPoints.Count;
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
				Vector3 point = this.m_ShapeEditor.GetPointPosition(i);
				if (rect.Contains(point))
				{
					this.SelectPoint(i, type);
				}
			}
			this.m_ShapeEditor.Repaint();
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
