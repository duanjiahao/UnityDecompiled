using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class QuadTree<T> where T : IBounds
	{
		private QuadTreeNode<T> m_Root = null;

		private Rect m_Rectangle;

		private Vector2 m_ScreenSpaceOffset = Vector2.zero;

		public Vector2 screenSpaceOffset
		{
			get
			{
				return this.m_ScreenSpaceOffset;
			}
			set
			{
				this.m_ScreenSpaceOffset = value;
			}
		}

		public Rect rectangle
		{
			get
			{
				return this.m_Rectangle;
			}
		}

		public int Count
		{
			get
			{
				return this.m_Root.CountItemsIncludingChildren();
			}
		}

		public QuadTree()
		{
			this.Clear();
		}

		public void Clear()
		{
			this.SetSize(new Rect(0f, 0f, 1f, 1f));
		}

		public void SetSize(Rect rectangle)
		{
			this.m_Root = null;
			this.m_Rectangle = rectangle;
			this.m_Root = new QuadTreeNode<T>(this.m_Rectangle);
		}

		public void Insert(List<T> items)
		{
			foreach (T current in items)
			{
				this.Insert(current);
			}
		}

		public void Insert(T item)
		{
			this.m_Root.Insert(item);
		}

		public void Remove(T item)
		{
			this.m_Root.Remove(item);
		}

		public List<T> IntersectsWith(Rect area)
		{
			area.x -= this.m_ScreenSpaceOffset.x;
			area.y -= this.m_ScreenSpaceOffset.y;
			return this.m_Root.IntersectsWith(area);
		}

		public List<T> ContainedBy(Rect area)
		{
			area.x -= this.m_ScreenSpaceOffset.x;
			area.y -= this.m_ScreenSpaceOffset.y;
			return this.m_Root.ContainedBy(area);
		}

		public List<T> Elements()
		{
			return this.m_Root.GetElementsIncludingChildren();
		}

		public void DebugDraw()
		{
			this.m_Root.DebugDraw(this.m_ScreenSpaceOffset);
		}
	}
}
