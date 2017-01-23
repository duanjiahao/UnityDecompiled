using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class RectTransform : Transform
	{
		public delegate void ReapplyDrivenProperties(RectTransform driven);

		public enum Edge
		{
			Left,
			Right,
			Top,
			Bottom
		}

		public enum Axis
		{
			Horizontal,
			Vertical
		}

		public static event RectTransform.ReapplyDrivenProperties reapplyDrivenProperties
		{
			add
			{
				RectTransform.ReapplyDrivenProperties reapplyDrivenProperties = RectTransform.reapplyDrivenProperties;
				RectTransform.ReapplyDrivenProperties reapplyDrivenProperties2;
				do
				{
					reapplyDrivenProperties2 = reapplyDrivenProperties;
					reapplyDrivenProperties = Interlocked.CompareExchange<RectTransform.ReapplyDrivenProperties>(ref RectTransform.reapplyDrivenProperties, (RectTransform.ReapplyDrivenProperties)Delegate.Combine(reapplyDrivenProperties2, value), reapplyDrivenProperties);
				}
				while (reapplyDrivenProperties != reapplyDrivenProperties2);
			}
			remove
			{
				RectTransform.ReapplyDrivenProperties reapplyDrivenProperties = RectTransform.reapplyDrivenProperties;
				RectTransform.ReapplyDrivenProperties reapplyDrivenProperties2;
				do
				{
					reapplyDrivenProperties2 = reapplyDrivenProperties;
					reapplyDrivenProperties = Interlocked.CompareExchange<RectTransform.ReapplyDrivenProperties>(ref RectTransform.reapplyDrivenProperties, (RectTransform.ReapplyDrivenProperties)Delegate.Remove(reapplyDrivenProperties2, value), reapplyDrivenProperties);
				}
				while (reapplyDrivenProperties != reapplyDrivenProperties2);
			}
		}

		public Rect rect
		{
			get
			{
				Rect result;
				this.INTERNAL_get_rect(out result);
				return result;
			}
		}

		public Vector2 anchorMin
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_anchorMin(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_anchorMin(ref value);
			}
		}

		public Vector2 anchorMax
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_anchorMax(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_anchorMax(ref value);
			}
		}

		public Vector3 anchoredPosition3D
		{
			get
			{
				Vector2 anchoredPosition = this.anchoredPosition;
				return new Vector3(anchoredPosition.x, anchoredPosition.y, base.localPosition.z);
			}
			set
			{
				this.anchoredPosition = new Vector2(value.x, value.y);
				Vector3 localPosition = base.localPosition;
				localPosition.z = value.z;
				base.localPosition = localPosition;
			}
		}

		public Vector2 anchoredPosition
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_anchoredPosition(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_anchoredPosition(ref value);
			}
		}

		public Vector2 sizeDelta
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_sizeDelta(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_sizeDelta(ref value);
			}
		}

		public Vector2 pivot
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_pivot(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_pivot(ref value);
			}
		}

		internal extern Object drivenByObject
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal extern DrivenTransformProperties drivenProperties
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector2 offsetMin
		{
			get
			{
				return this.anchoredPosition - Vector2.Scale(this.sizeDelta, this.pivot);
			}
			set
			{
				Vector2 vector = value - (this.anchoredPosition - Vector2.Scale(this.sizeDelta, this.pivot));
				this.sizeDelta -= vector;
				this.anchoredPosition += Vector2.Scale(vector, Vector2.one - this.pivot);
			}
		}

		public Vector2 offsetMax
		{
			get
			{
				return this.anchoredPosition + Vector2.Scale(this.sizeDelta, Vector2.one - this.pivot);
			}
			set
			{
				Vector2 vector = value - (this.anchoredPosition + Vector2.Scale(this.sizeDelta, Vector2.one - this.pivot));
				this.sizeDelta += vector;
				this.anchoredPosition += Vector2.Scale(vector, this.pivot);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rect(out Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_anchorMin(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_anchorMin(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_anchorMax(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_anchorMax(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_anchoredPosition(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_anchoredPosition(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_sizeDelta(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_sizeDelta(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_pivot(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_pivot(ref Vector2 value);

		[RequiredByNativeCode]
		internal static void SendReapplyDrivenProperties(RectTransform driven)
		{
			if (RectTransform.reapplyDrivenProperties != null)
			{
				RectTransform.reapplyDrivenProperties(driven);
			}
		}

		public void GetLocalCorners(Vector3[] fourCornersArray)
		{
			if (fourCornersArray == null || fourCornersArray.Length < 4)
			{
				Debug.LogError("Calling GetLocalCorners with an array that is null or has less than 4 elements.");
			}
			else
			{
				Rect rect = this.rect;
				float x = rect.x;
				float y = rect.y;
				float xMax = rect.xMax;
				float yMax = rect.yMax;
				fourCornersArray[0] = new Vector3(x, y, 0f);
				fourCornersArray[1] = new Vector3(x, yMax, 0f);
				fourCornersArray[2] = new Vector3(xMax, yMax, 0f);
				fourCornersArray[3] = new Vector3(xMax, y, 0f);
			}
		}

		public void GetWorldCorners(Vector3[] fourCornersArray)
		{
			if (fourCornersArray == null || fourCornersArray.Length < 4)
			{
				Debug.LogError("Calling GetWorldCorners with an array that is null or has less than 4 elements.");
			}
			else
			{
				this.GetLocalCorners(fourCornersArray);
				Transform transform = base.transform;
				for (int i = 0; i < 4; i++)
				{
					fourCornersArray[i] = transform.TransformPoint(fourCornersArray[i]);
				}
			}
		}

		internal Rect GetRectInParentSpace()
		{
			Rect rect = this.rect;
			Vector2 a = this.offsetMin + Vector2.Scale(this.pivot, rect.size);
			Transform parent = base.transform.parent;
			if (parent)
			{
				RectTransform component = parent.GetComponent<RectTransform>();
				if (component)
				{
					a += Vector2.Scale(this.anchorMin, component.rect.size);
				}
			}
			rect.x += a.x;
			rect.y += a.y;
			return rect;
		}

		public void SetInsetAndSizeFromParentEdge(RectTransform.Edge edge, float inset, float size)
		{
			int index = (edge != RectTransform.Edge.Top && edge != RectTransform.Edge.Bottom) ? 0 : 1;
			bool flag = edge == RectTransform.Edge.Top || edge == RectTransform.Edge.Right;
			float value = (float)((!flag) ? 0 : 1);
			Vector2 vector = this.anchorMin;
			vector[index] = value;
			this.anchorMin = vector;
			vector = this.anchorMax;
			vector[index] = value;
			this.anchorMax = vector;
			Vector2 sizeDelta = this.sizeDelta;
			sizeDelta[index] = size;
			this.sizeDelta = sizeDelta;
			Vector2 anchoredPosition = this.anchoredPosition;
			anchoredPosition[index] = ((!flag) ? (inset + size * this.pivot[index]) : (-inset - size * (1f - this.pivot[index])));
			this.anchoredPosition = anchoredPosition;
		}

		public void SetSizeWithCurrentAnchors(RectTransform.Axis axis, float size)
		{
			Vector2 sizeDelta = this.sizeDelta;
			sizeDelta[(int)axis] = size - this.GetParentSize()[(int)axis] * (this.anchorMax[(int)axis] - this.anchorMin[(int)axis]);
			this.sizeDelta = sizeDelta;
		}

		private Vector2 GetParentSize()
		{
			RectTransform rectTransform = base.parent as RectTransform;
			Vector2 result;
			if (!rectTransform)
			{
				result = Vector2.zero;
			}
			else
			{
				result = rectTransform.rect.size;
			}
			return result;
		}
	}
}
