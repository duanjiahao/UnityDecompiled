using System;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class RectUtils
	{
		public static bool Contains(Rect a, Rect b)
		{
			return a.xMin <= b.xMin && a.xMax >= b.xMax && a.yMin <= b.yMin && a.yMax >= b.yMax;
		}

		public static Rect Encompass(Rect a, Rect b)
		{
			Rect result = a;
			result.xMin = Math.Min(a.xMin, b.xMin);
			result.yMin = Math.Min(a.yMin, b.yMin);
			result.xMax = Math.Max(a.xMax, b.xMax);
			result.yMax = Math.Max(a.yMax, b.yMax);
			return result;
		}

		public static Rect Inflate(Rect a, float factor)
		{
			return RectUtils.Inflate(a, factor, factor);
		}

		public static Rect Inflate(Rect a, float factorX, float factorY)
		{
			float num = a.width * factorX;
			float num2 = a.height * factorY;
			float num3 = (num - a.width) / 2f;
			float num4 = (num2 - a.height) / 2f;
			Rect result = a;
			result.xMin -= num3;
			result.yMin -= num4;
			result.xMax += num3;
			result.yMax += num4;
			return result;
		}

		public static bool Intersects(Rect r1, Rect r2)
		{
			return r1.Overlaps(r2) || r2.Overlaps(r1);
		}

		public static bool Intersection(Rect r1, Rect r2, out Rect intersection)
		{
			bool result;
			if (!r1.Overlaps(r2) && !r2.Overlaps(r1))
			{
				intersection = new Rect(0f, 0f, 0f, 0f);
				result = false;
			}
			else
			{
				float num = Mathf.Max(r1.xMin, r2.xMin);
				float num2 = Mathf.Max(r1.yMin, r2.yMin);
				float num3 = Mathf.Min(r1.xMax, r2.xMax);
				float num4 = Mathf.Min(r1.yMax, r2.yMax);
				intersection = new Rect(num, num2, num3 - num, num4 - num2);
				result = true;
			}
			return result;
		}

		public static bool IntersectsSegment(Rect rect, Vector2 p1, Vector2 p2)
		{
			float num = Mathf.Min(p1.x, p2.x);
			float num2 = Mathf.Max(p1.x, p2.x);
			if (num2 > rect.xMax)
			{
				num2 = rect.xMax;
			}
			if (num < rect.xMin)
			{
				num = rect.xMin;
			}
			bool result;
			if (num > num2)
			{
				result = false;
			}
			else
			{
				float num3 = Mathf.Min(p1.y, p2.y);
				float num4 = Mathf.Max(p1.y, p2.y);
				float num5 = p2.x - p1.x;
				if (Mathf.Abs(num5) > 1E-07f)
				{
					float num6 = (p2.y - p1.y) / num5;
					float num7 = p1.y - num6 * p1.x;
					num3 = num6 * num + num7;
					num4 = num6 * num2 + num7;
				}
				if (num3 > num4)
				{
					float num8 = num4;
					num4 = num3;
					num3 = num8;
				}
				if (num4 > rect.yMax)
				{
					num4 = rect.yMax;
				}
				if (num3 < rect.yMin)
				{
					num3 = rect.yMin;
				}
				result = (num3 <= num4);
			}
			return result;
		}

		public static Rect OffsetX(Rect r, float offsetX)
		{
			return RectUtils.Offset(r, offsetX, 0f);
		}

		public static Rect Offset(Rect r, float offsetX, float offsetY)
		{
			Rect result = r;
			result.xMin += offsetX;
			result.yMin += offsetY;
			return result;
		}

		public static Rect Offset(Rect a, Rect b)
		{
			Rect result = a;
			result.xMin += b.xMin;
			result.yMin += b.yMin;
			return result;
		}

		public static Rect Move(Rect r, Vector2 delta)
		{
			Rect result = r;
			result.xMin += delta.x;
			result.yMin += delta.y;
			result.xMax += delta.x;
			result.yMax += delta.y;
			return result;
		}
	}
}
