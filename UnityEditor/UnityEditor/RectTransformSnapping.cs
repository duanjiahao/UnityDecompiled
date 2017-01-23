using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class RectTransformSnapping
	{
		internal const float kSnapThreshold = 0.05f;

		private static SnapGuideCollection[] s_SnapGuides = new SnapGuideCollection[]
		{
			new SnapGuideCollection(),
			new SnapGuideCollection()
		};

		private static float[] kSidesAndMiddle = new float[]
		{
			0f,
			0.5f,
			1f
		};

		private static Vector3[] s_Corners = new Vector3[4];

		internal static void OnGUI()
		{
			RectTransformSnapping.s_SnapGuides[0].OnGUI();
			RectTransformSnapping.s_SnapGuides[1].OnGUI();
		}

		internal static void DrawGuides()
		{
			if (!EditorGUI.actionKey)
			{
				RectTransformSnapping.s_SnapGuides[0].DrawGuides();
				RectTransformSnapping.s_SnapGuides[1].DrawGuides();
			}
		}

		private static Vector3 GetInterpolatedCorner(Vector3[] corners, int mainAxis, float alongMainAxis, float alongOtherAxis)
		{
			if (mainAxis != 0)
			{
				float num = alongMainAxis;
				alongMainAxis = alongOtherAxis;
				alongOtherAxis = num;
			}
			return corners[0] * (1f - alongMainAxis) * (1f - alongOtherAxis) + corners[1] * (1f - alongMainAxis) * alongOtherAxis + corners[3] * alongMainAxis * (1f - alongOtherAxis) + corners[2] * alongMainAxis * alongOtherAxis;
		}

		internal static void CalculatePivotSnapValues(Rect rect, Vector3 pivot, Quaternion rotation)
		{
			for (int i = 0; i < 2; i++)
			{
				RectTransformSnapping.s_SnapGuides[i].Clear();
				for (int j = 0; j < RectTransformSnapping.kSidesAndMiddle.Length; j++)
				{
					RectTransformSnapping.s_SnapGuides[i].AddGuide(new SnapGuide(RectTransformSnapping.kSidesAndMiddle[j], RectTransformSnapping.GetGuideLineForRect(rect, pivot, rotation, i, RectTransformSnapping.kSidesAndMiddle[j])));
				}
			}
		}

		internal static void CalculateAnchorSnapValues(Transform parentSpace, Transform self, RectTransform gui, int minmaxX, int minmaxY)
		{
			for (int i = 0; i < 2; i++)
			{
				RectTransformSnapping.s_SnapGuides[i].Clear();
				RectTransform component = parentSpace.GetComponent<RectTransform>();
				component.GetWorldCorners(RectTransformSnapping.s_Corners);
				for (int j = 0; j < RectTransformSnapping.kSidesAndMiddle.Length; j++)
				{
					float num = RectTransformSnapping.kSidesAndMiddle[j];
					RectTransformSnapping.s_SnapGuides[i].AddGuide(new SnapGuide(num, new Vector3[]
					{
						RectTransformSnapping.GetInterpolatedCorner(RectTransformSnapping.s_Corners, i, num, 0f),
						RectTransformSnapping.GetInterpolatedCorner(RectTransformSnapping.s_Corners, i, num, 1f)
					}));
				}
				IEnumerator enumerator = parentSpace.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Transform transform = (Transform)enumerator.Current;
						if (!(transform == self))
						{
							RectTransform component2 = transform.GetComponent<RectTransform>();
							if (component2)
							{
								RectTransformSnapping.s_SnapGuides[i].AddGuide(new SnapGuide(component2.anchorMin[i], new Vector3[0]));
								RectTransformSnapping.s_SnapGuides[i].AddGuide(new SnapGuide(component2.anchorMax[i], new Vector3[0]));
							}
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				int num2 = (i != 0) ? minmaxY : minmaxX;
				if (num2 == 0)
				{
					RectTransformSnapping.s_SnapGuides[i].AddGuide(new SnapGuide(gui.anchorMax[i], new Vector3[0]));
				}
				if (num2 == 1)
				{
					RectTransformSnapping.s_SnapGuides[i].AddGuide(new SnapGuide(gui.anchorMin[i], new Vector3[0]));
				}
			}
		}

		internal static void CalculateOffsetSnapValues(Transform parentSpace, Transform self, RectTransform parentRect, RectTransform rect, int xHandle, int yHandle)
		{
			for (int i = 0; i < 2; i++)
			{
				RectTransformSnapping.s_SnapGuides[i].Clear();
			}
			if (!(parentSpace == null))
			{
				for (int j = 0; j < 2; j++)
				{
					int num = (j != 0) ? yHandle : xHandle;
					if (num != 1)
					{
						List<SnapGuide> snapGuides = RectTransformSnapping.GetSnapGuides(parentSpace, self, parentRect, rect, j, num);
						foreach (SnapGuide current in snapGuides)
						{
							RectTransformSnapping.s_SnapGuides[j].AddGuide(current);
						}
					}
				}
			}
		}

		internal static void CalculatePositionSnapValues(Transform parentSpace, Transform self, RectTransform parentRect, RectTransform rect)
		{
			for (int i = 0; i < 2; i++)
			{
				RectTransformSnapping.s_SnapGuides[i].Clear();
			}
			if (!(parentSpace == null))
			{
				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < RectTransformSnapping.kSidesAndMiddle.Length; k++)
					{
						List<SnapGuide> snapGuides = RectTransformSnapping.GetSnapGuides(parentSpace, self, parentRect, rect, j, k);
						foreach (SnapGuide current in snapGuides)
						{
							current.value = RectTransformSnapping.GetGuideValueForRect(rect, current.value, j, RectTransformSnapping.kSidesAndMiddle[k]);
							RectTransformSnapping.s_SnapGuides[j].AddGuide(current);
						}
					}
				}
			}
		}

		private static List<SnapGuide> GetSnapGuides(Transform parentSpace, Transform self, RectTransform parentRect, RectTransform rect, int axis, int side)
		{
			List<SnapGuide> list = new List<SnapGuide>();
			if (parentRect != null)
			{
				float num = RectTransformSnapping.kSidesAndMiddle[side];
				float num2 = Mathf.Lerp(rect.anchorMin[axis], rect.anchorMax[axis], num);
				list.Add(new SnapGuide(num2 * parentRect.rect.size[axis], RectTransformSnapping.GetGuideLineForRect(parentRect, axis, num2)));
				float num3 = Mathf.Lerp(rect.anchorMin[axis], rect.anchorMax[axis], num);
				if (num != num3)
				{
					list.Add(new SnapGuide(num * parentRect.rect.size[axis], false, RectTransformSnapping.GetGuideLineForRect(parentRect, axis, num)));
				}
			}
			IEnumerator enumerator = parentSpace.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					if (!(transform == self))
					{
						RectTransform component = transform.GetComponent<RectTransform>();
						if (component)
						{
							if (side == 0)
							{
								bool safe = component.anchorMin[axis] == rect.anchorMin[axis];
								list.Add(new SnapGuide(component.GetRectInParentSpace().min[axis], safe, RectTransformSnapping.GetGuideLineForRect(component, axis, 0f)));
								safe = (component.anchorMax[axis] == rect.anchorMin[axis]);
								list.Add(new SnapGuide(component.GetRectInParentSpace().max[axis], safe, RectTransformSnapping.GetGuideLineForRect(component, axis, 1f)));
							}
							if (side == 2)
							{
								bool safe = component.anchorMax[axis] == rect.anchorMax[axis];
								list.Add(new SnapGuide(component.GetRectInParentSpace().max[axis], safe, RectTransformSnapping.GetGuideLineForRect(component, axis, 1f)));
								safe = (component.anchorMin[axis] == rect.anchorMax[axis]);
								list.Add(new SnapGuide(component.GetRectInParentSpace().min[axis], safe, RectTransformSnapping.GetGuideLineForRect(component, axis, 0f)));
							}
							if (side == 1)
							{
								bool safe = component.anchorMin[axis] - rect.anchorMin[axis] == -(component.anchorMax[axis] - rect.anchorMax[axis]);
								list.Add(new SnapGuide(component.GetRectInParentSpace().center[axis], safe, RectTransformSnapping.GetGuideLineForRect(component, axis, 0.5f)));
							}
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return list;
		}

		private static Vector3[] GetGuideLineForRect(RectTransform rect, int axis, float side)
		{
			Vector3[] array = new Vector3[2];
			array[0][1 - axis] = rect.rect.min[1 - axis];
			array[1][1 - axis] = rect.rect.max[1 - axis];
			array[0][axis] = Mathf.Lerp(rect.rect.min[axis], rect.rect.max[axis], side);
			array[1][axis] = array[0][axis];
			array[0] = rect.transform.TransformPoint(array[0]);
			array[1] = rect.transform.TransformPoint(array[1]);
			return array;
		}

		private static Vector3[] GetGuideLineForRect(Rect rect, Vector3 pivot, Quaternion rotation, int axis, float side)
		{
			Vector3[] array = new Vector3[2];
			array[0][1 - axis] = rect.min[1 - axis];
			array[1][1 - axis] = rect.max[1 - axis];
			array[0][axis] = Mathf.Lerp(rect.min[axis], rect.max[axis], side);
			array[1][axis] = array[0][axis];
			array[0] = rotation * array[0] + pivot;
			array[1] = rotation * array[1] + pivot;
			return array;
		}

		private static float GetGuideValueForRect(RectTransform rect, float value, int axis, float side)
		{
			RectTransform component = rect.transform.parent.GetComponent<RectTransform>();
			float num = (!component) ? 0f : component.rect.size[axis];
			float num2 = Mathf.Lerp(rect.anchorMin[axis], rect.anchorMax[axis], rect.pivot[axis]) * num;
			float num3 = rect.rect.size[axis] * (rect.pivot[axis] - side);
			return value - num2 + num3;
		}

		internal static Vector2 SnapToGuides(Vector2 value, Vector2 snapDistance)
		{
			return new Vector2(RectTransformSnapping.SnapToGuides(value.x, snapDistance.x, 0), RectTransformSnapping.SnapToGuides(value.y, snapDistance.y, 1));
		}

		internal static float SnapToGuides(float value, float snapDistance, int axis)
		{
			float result;
			if (EditorGUI.actionKey)
			{
				result = value;
			}
			else
			{
				SnapGuideCollection snapGuideCollection = (axis != 0) ? RectTransformSnapping.s_SnapGuides[1] : RectTransformSnapping.s_SnapGuides[0];
				result = snapGuideCollection.SnapToGuides(value, snapDistance);
			}
			return result;
		}
	}
}
