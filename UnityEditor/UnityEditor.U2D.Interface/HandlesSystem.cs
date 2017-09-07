using System;
using UnityEngine;
using UnityEngine.U2D.Interface;

namespace UnityEditor.U2D.Interface
{
	internal class HandlesSystem : IHandles
	{
		private static IHandles m_System;

		public Color color
		{
			get
			{
				return Handles.color;
			}
			set
			{
				Handles.color = value;
			}
		}

		public Matrix4x4 matrix
		{
			get
			{
				return Handles.matrix;
			}
			set
			{
				Handles.matrix = value;
			}
		}

		public static void SetSystem(IHandles system)
		{
			HandlesSystem.m_System = system;
		}

		public static IHandles GetSystem()
		{
			if (HandlesSystem.m_System == null)
			{
				HandlesSystem.m_System = new HandlesSystem();
			}
			return HandlesSystem.m_System;
		}

		public Vector3[] MakeBezierPoints(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int division)
		{
			return Handles.MakeBezierPoints(startPosition, endPosition, startTangent, endTangent, division);
		}

		public void DrawAAPolyLine(ITexture2D lineTex, float width, params Vector3[] points)
		{
			Handles.DrawAAPolyLine(lineTex, width, points);
		}

		public void DrawAAPolyLine(ITexture2D lineTex, params Vector3[] points)
		{
			Handles.DrawAAPolyLine(lineTex, points);
		}

		public void DrawLine(Vector3 p1, Vector3 p2)
		{
			Handles.DrawLine(p1, p2);
		}

		public void SetDiscSectionPoints(Vector3[] dest, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
		{
			Handles.SetDiscSectionPoints(dest, center, normal, from, angle, radius);
		}
	}
}
