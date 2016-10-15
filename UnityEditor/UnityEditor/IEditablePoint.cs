using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal interface IEditablePoint
	{
		int Count
		{
			get;
		}

		Vector3 GetPosition(int idx);

		void SetPosition(int idx, Vector3 position);

		Color GetDefaultColor();

		Color GetSelectedColor();

		float GetPointScale();

		IEnumerable<Vector3> GetPositions();

		Vector3[] GetUnselectedPositions();

		Vector3[] GetSelectedPositions();
	}
}
