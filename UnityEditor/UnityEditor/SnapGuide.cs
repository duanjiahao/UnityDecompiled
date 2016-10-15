using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class SnapGuide
	{
		public float value;

		public List<Vector3> lineVertices = new List<Vector3>();

		public bool safe = true;

		public SnapGuide(float value, params Vector3[] vertices) : this(value, true, vertices)
		{
		}

		public SnapGuide(float value, bool safe, params Vector3[] vertices)
		{
			this.value = value;
			this.lineVertices.AddRange(vertices);
			this.safe = safe;
		}

		public void Draw()
		{
			Handles.color = ((!this.safe) ? new Color(1f, 0.5f, 0f, 1f) : new Color(0f, 0.5f, 1f, 1f));
			for (int i = 0; i < this.lineVertices.Count; i += 2)
			{
				Vector3 vector = this.lineVertices[i];
				Vector3 vector2 = this.lineVertices[i + 1];
				if (!(vector == vector2))
				{
					Vector3 a = (vector2 - vector).normalized * 0.05f;
					vector -= a * HandleUtility.GetHandleSize(vector);
					vector2 += a * HandleUtility.GetHandleSize(vector2);
					Handles.DrawLine(vector, vector2);
				}
			}
		}
	}
}
