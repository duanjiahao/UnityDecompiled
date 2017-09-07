using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class QuadTreeNode<T> where T : IBounds
	{
		private Rect m_BoundingRect;

		private static Color m_DebugFillColor = new Color(1f, 1f, 1f, 0.01f);

		private static Color m_DebugWireColor = new Color(1f, 0f, 0f, 0.5f);

		private static Color m_DebugBoxFillColor = new Color(1f, 0f, 0f, 0.01f);

		private const float kSmallestAreaForQuadTreeNode = 10f;

		private List<T> m_Elements = new List<T>();

		private List<QuadTreeNode<T>> m_ChildrenNodes = new List<QuadTreeNode<T>>(4);

		public bool IsEmpty
		{
			get
			{
				return (this.m_BoundingRect.width == 0f && this.m_BoundingRect.height == 0f) || this.m_ChildrenNodes.Count == 0;
			}
		}

		public Rect BoundingRect
		{
			get
			{
				return this.m_BoundingRect;
			}
		}

		public QuadTreeNode(Rect r)
		{
			this.m_BoundingRect = r;
		}

		public int CountItemsIncludingChildren()
		{
			return this.Count(true);
		}

		public int CountLocalItems()
		{
			return this.Count(false);
		}

		private int Count(bool recursive)
		{
			int num = this.m_Elements.Count;
			if (recursive)
			{
				foreach (QuadTreeNode<T> current in this.m_ChildrenNodes)
				{
					num += current.Count(recursive);
				}
			}
			return num;
		}

		public List<T> GetElementsIncludingChildren()
		{
			return this.Elements(true);
		}

		public List<T> GetElements()
		{
			return this.Elements(false);
		}

		private List<T> Elements(bool recursive)
		{
			List<T> list = new List<T>();
			if (recursive)
			{
				foreach (QuadTreeNode<T> current in this.m_ChildrenNodes)
				{
					list.AddRange(current.Elements(recursive));
				}
			}
			list.AddRange(this.m_Elements);
			return list;
		}

		public List<T> IntersectsWith(Rect queryArea)
		{
			List<T> list = new List<T>();
			foreach (T current in this.m_Elements)
			{
				if (RectUtils.Intersects(current.boundingRect, queryArea))
				{
					list.Add(current);
				}
			}
			foreach (QuadTreeNode<T> current2 in this.m_ChildrenNodes)
			{
				if (!current2.IsEmpty)
				{
					if (RectUtils.Intersects(current2.BoundingRect, queryArea))
					{
						list.AddRange(current2.IntersectsWith(queryArea));
						break;
					}
				}
			}
			return list;
		}

		public List<T> ContainedBy(Rect queryArea)
		{
			List<T> list = new List<T>();
			foreach (T current in this.m_Elements)
			{
				if (RectUtils.Contains(current.boundingRect, queryArea) || queryArea.Overlaps(current.boundingRect))
				{
					list.Add(current);
				}
			}
			foreach (QuadTreeNode<T> current2 in this.m_ChildrenNodes)
			{
				if (!current2.IsEmpty)
				{
					if (RectUtils.Contains(current2.BoundingRect, queryArea))
					{
						list.AddRange(current2.ContainedBy(queryArea));
						break;
					}
					if (RectUtils.Contains(queryArea, current2.BoundingRect))
					{
						list.AddRange(current2.Elements(true));
					}
					else if (current2.BoundingRect.Overlaps(queryArea))
					{
						list.AddRange(current2.ContainedBy(queryArea));
					}
				}
			}
			return list;
		}

		public void Remove(T item)
		{
			this.m_Elements.Remove(item);
			foreach (QuadTreeNode<T> current in this.m_ChildrenNodes)
			{
				current.Remove(item);
			}
		}

		public void Insert(T item)
		{
			if (!RectUtils.Contains(this.m_BoundingRect, item.boundingRect))
			{
				Rect rect = default(Rect);
				if (!RectUtils.Intersection(item.boundingRect, this.m_BoundingRect, out rect))
				{
					return;
				}
			}
			if (this.m_ChildrenNodes.Count == 0)
			{
				this.Subdivide();
			}
			foreach (QuadTreeNode<T> current in this.m_ChildrenNodes)
			{
				if (RectUtils.Contains(current.BoundingRect, item.boundingRect))
				{
					current.Insert(item);
					return;
				}
			}
			this.m_Elements.Add(item);
		}

		private void Subdivide()
		{
			if (this.m_BoundingRect.height * this.m_BoundingRect.width > 10f)
			{
				float num = this.m_BoundingRect.width / 2f;
				float num2 = this.m_BoundingRect.height / 2f;
				this.m_ChildrenNodes.Add(new QuadTreeNode<T>(new Rect(this.m_BoundingRect.position.x, this.m_BoundingRect.position.y, num, num2)));
				this.m_ChildrenNodes.Add(new QuadTreeNode<T>(new Rect(this.m_BoundingRect.xMin, this.m_BoundingRect.yMin + num2, num, num2)));
				this.m_ChildrenNodes.Add(new QuadTreeNode<T>(new Rect(this.m_BoundingRect.xMin + num, this.m_BoundingRect.yMin, num, num2)));
				this.m_ChildrenNodes.Add(new QuadTreeNode<T>(new Rect(this.m_BoundingRect.xMin + num, this.m_BoundingRect.yMin + num2, num, num2)));
			}
		}

		public void DebugDraw(Vector2 offset)
		{
			HandleUtility.ApplyWireMaterial();
			Rect boundingRect = this.m_BoundingRect;
			boundingRect.x += offset.x;
			boundingRect.y += offset.y;
			Handles.DrawSolidRectangleWithOutline(boundingRect, QuadTreeNode<T>.m_DebugFillColor, QuadTreeNode<T>.m_DebugWireColor);
			foreach (QuadTreeNode<T> current in this.m_ChildrenNodes)
			{
				current.DebugDraw(offset);
			}
			foreach (IBounds bounds in this.Elements(false))
			{
				Rect boundingRect2 = bounds.boundingRect;
				boundingRect2.x += offset.x;
				boundingRect2.y += offset.y;
				Handles.DrawSolidRectangleWithOutline(boundingRect2, QuadTreeNode<T>.m_DebugBoxFillColor, Color.yellow);
			}
		}
	}
}
