using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ManipulationToolUtility
	{
		public delegate void HandleDragChange(string handleName, bool dragging);

		public static ManipulationToolUtility.HandleDragChange handleDragChange;

		public static Vector3 minDragDifference
		{
			get;
			set;
		}

		public static void SetMinDragDifferenceForPos(Vector3 position)
		{
			ManipulationToolUtility.minDragDifference = Vector3.one * (HandleUtility.GetHandleSize(position) / 80f);
		}

		public static void SetMinDragDifferenceForPos(Vector3 position, float multiplier)
		{
			ManipulationToolUtility.minDragDifference = Vector3.one * (HandleUtility.GetHandleSize(position) * multiplier / 80f);
		}

		public static void DisableMinDragDifference()
		{
			ManipulationToolUtility.minDragDifference = Vector3.zero;
		}

		public static void DisableMinDragDifferenceForAxis(int axis)
		{
			Vector2 v = ManipulationToolUtility.minDragDifference;
			v[axis] = 0f;
			ManipulationToolUtility.minDragDifference = v;
		}

		public static void DisableMinDragDifferenceBasedOnSnapping(Vector3 positionBeforeSnapping, Vector3 positionAfterSnapping)
		{
			for (int i = 0; i < 3; i++)
			{
				if (positionBeforeSnapping[i] != positionAfterSnapping[i])
				{
					ManipulationToolUtility.DisableMinDragDifferenceForAxis(i);
				}
			}
		}

		public static void BeginDragging(string handleName)
		{
			if (ManipulationToolUtility.handleDragChange != null)
			{
				ManipulationToolUtility.handleDragChange(handleName, true);
			}
		}

		public static void EndDragging(string handleName)
		{
			if (ManipulationToolUtility.handleDragChange != null)
			{
				ManipulationToolUtility.handleDragChange(handleName, false);
			}
		}

		public static void DetectDraggingBasedOnMouseDownUp(string handleName, EventType typeBefore)
		{
			if (typeBefore == EventType.MouseDrag && Event.current.type != EventType.MouseDrag)
			{
				ManipulationToolUtility.BeginDragging(handleName);
			}
			else if (typeBefore == EventType.MouseUp && Event.current.type != EventType.MouseUp)
			{
				ManipulationToolUtility.EndDragging(handleName);
			}
		}
	}
}
