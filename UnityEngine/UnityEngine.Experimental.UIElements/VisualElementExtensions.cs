using System;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace UnityEngine.Experimental.UIElements
{
	public static class VisualElementExtensions
	{
		public static Vector2 GlobalToBound(this VisualElement ele, Vector2 p)
		{
			Vector3 vector = ele.globalTransform.inverse.MultiplyPoint3x4(new Vector3(p.x, p.y, 0f));
			return new Vector2(vector.x - ele.position.position.x, vector.y - ele.position.position.y);
		}

		public static Vector2 LocalToGlobal(this VisualElement ele, Vector2 p)
		{
			Vector3 vector = ele.globalTransform.MultiplyPoint3x4(p + ele.position.position);
			return new Vector2(vector.x, vector.y);
		}

		public static Rect GlobalToBound(this VisualElement ele, Rect r)
		{
			Matrix4x4 inverse = ele.globalTransform.inverse;
			Vector2 a = inverse.MultiplyPoint3x4(r.position);
			r.position = a - ele.position.position;
			r.size = inverse.MultiplyPoint3x4(r.size);
			return r;
		}

		public static Rect LocalToGlobal(this VisualElement ele, Rect r)
		{
			Matrix4x4 globalTransform = ele.globalTransform;
			r.position = globalTransform.MultiplyPoint3x4(ele.position.position + r.position);
			r.size = globalTransform.MultiplyPoint3x4(r.size);
			return r;
		}

		public static Vector2 ChangeCoordinatesTo(this VisualElement src, VisualElement dest, Vector2 point)
		{
			return dest.GlobalToBound(src.LocalToGlobal(point));
		}

		public static Rect ChangeCoordinatesTo(this VisualElement src, VisualElement dest, Rect rect)
		{
			return dest.GlobalToBound(src.LocalToGlobal(rect));
		}

		public static void StretchToParentSize(this VisualElement elem)
		{
			elem.positionType = PositionType.Absolute;
			elem.positionLeft = 0f;
			elem.positionTop = 0f;
			elem.positionRight = 0f;
			elem.positionBottom = 0f;
		}

		public static T GetFirstOfType<T>(this VisualElement self) where T : class
		{
			T t = self as T;
			T result;
			if (t != null)
			{
				result = t;
			}
			else
			{
				result = self.GetFirstAncestorOfType<T>();
			}
			return result;
		}

		public static T GetFirstAncestorOfType<T>(this VisualElement self) where T : class
		{
			T result;
			for (VisualElement parent = self.parent; parent != null; parent = parent.parent)
			{
				T t = parent as T;
				if (t != null)
				{
					result = t;
					return result;
				}
			}
			result = (T)((object)null);
			return result;
		}
	}
}
