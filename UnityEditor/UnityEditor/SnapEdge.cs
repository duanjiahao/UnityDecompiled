using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class SnapEdge
	{
		public enum EdgeDir
		{
			Left,
			Right,
			CenterX,
			Up,
			Down,
			CenterY,
			None
		}

		private const float kSnapDist = 0f;

		public UnityEngine.Object m_Object;

		public float pos;

		public float start;

		public float end;

		public float startDragPos;

		public float startDragStart;

		public SnapEdge.EdgeDir dir;

		internal SnapEdge(UnityEngine.Object win, SnapEdge.EdgeDir _d, float _p, float _s, float _e)
		{
			this.dir = _d;
			this.m_Object = win;
			this.pos = _p;
			this.start = _s;
			this.end = _e;
		}

		public override string ToString()
		{
			if (this.m_Object != null)
			{
				return string.Concat(new object[]
				{
					"Edge: ",
					this.dir,
					" of ",
					this.m_Object.name,
					"    pos: ",
					this.pos,
					" (",
					this.start,
					" - ",
					this.end,
					")"
				});
			}
			return "Edge: " + this.dir + " of NULL - something is wrong!";
		}

		internal static SnapEdge.EdgeDir OppositeEdge(SnapEdge.EdgeDir dir)
		{
			switch (dir)
			{
			case SnapEdge.EdgeDir.Left:
				return SnapEdge.EdgeDir.Right;
			case SnapEdge.EdgeDir.Right:
				return SnapEdge.EdgeDir.Left;
			case SnapEdge.EdgeDir.CenterX:
				return SnapEdge.EdgeDir.CenterX;
			case SnapEdge.EdgeDir.Up:
				return SnapEdge.EdgeDir.Down;
			case SnapEdge.EdgeDir.Down:
				return SnapEdge.EdgeDir.Up;
			}
			return SnapEdge.EdgeDir.CenterY;
		}

		private int EdgeCoordinateIndex()
		{
			if (this.dir == SnapEdge.EdgeDir.Left || this.dir == SnapEdge.EdgeDir.Right || this.dir == SnapEdge.EdgeDir.CenterX)
			{
				return 0;
			}
			return 1;
		}

		internal static Vector2 Snap(List<SnapEdge> sourceEdges, List<SnapEdge> edgesToSnapAgainst, List<KeyValuePair<SnapEdge, SnapEdge>>[] activeEdges)
		{
			Vector2 zero = Vector2.zero;
			float num = 10f;
			activeEdges[0].Clear();
			activeEdges[1].Clear();
			float[] array = new float[]
			{
				num,
				num
			};
			float[] array2 = new float[2];
			foreach (SnapEdge current in sourceEdges)
			{
				int num2 = current.EdgeCoordinateIndex();
				SnapEdge.Snap(current, edgesToSnapAgainst, ref array[num2], ref array2[num2], activeEdges[num2]);
			}
			zero.x = array2[0];
			zero.y = array2[1];
			return zero;
		}

		private static bool EdgeInside(SnapEdge edge, List<SnapEdge> frustum)
		{
			foreach (SnapEdge current in frustum)
			{
				if (!SnapEdge.ShouldEdgesSnap(edge, current))
				{
					return false;
				}
			}
			return true;
		}

		private static bool ShouldEdgesSnap(SnapEdge a, SnapEdge b)
		{
			return ((a.dir == SnapEdge.EdgeDir.CenterX || a.dir == SnapEdge.EdgeDir.CenterY) && a.dir == b.dir) || (a.dir == SnapEdge.OppositeEdge(b.dir) && (a.start <= b.end && a.end >= b.start));
		}

		internal static void Snap(SnapEdge edge, List<SnapEdge> otherEdges, ref float maxDist, ref float snapVal, List<KeyValuePair<SnapEdge, SnapEdge>> activeEdges)
		{
			foreach (SnapEdge current in otherEdges)
			{
				if (SnapEdge.ShouldEdgesSnap(edge, current))
				{
					float num = Mathf.Abs(current.pos - edge.pos);
					if (num < maxDist)
					{
						maxDist = num;
						snapVal = current.pos - edge.pos;
						activeEdges.Clear();
						activeEdges.Add(new KeyValuePair<SnapEdge, SnapEdge>(edge, current));
					}
					else if (num == maxDist)
					{
						activeEdges.Add(new KeyValuePair<SnapEdge, SnapEdge>(edge, current));
					}
				}
			}
		}

		internal void ApplyOffset(Vector2 offset, bool windowMove)
		{
			offset = ((this.dir != SnapEdge.EdgeDir.Left && this.dir != SnapEdge.EdgeDir.Right) ? new Vector2(offset.y, offset.x) : offset);
			if (windowMove)
			{
				this.pos += offset.x;
			}
			else
			{
				this.pos = offset.x + this.startDragPos;
			}
			this.start += offset.y;
			this.end += offset.y;
		}
	}
}
