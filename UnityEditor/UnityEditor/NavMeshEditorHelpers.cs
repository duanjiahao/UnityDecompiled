using System;
using UnityEngine;

namespace UnityEditor
{
	internal class NavMeshEditorHelpers
	{
		public static void DrawAgentDiagram(Rect rect, float agentRadius, float agentHeight, float agentClimb, float agentSlope)
		{
			if (Event.current.type == EventType.Repaint)
			{
				float num = 0.35f;
				float num2 = 20f;
				float num3 = 10f;
				float num4 = rect.height - (num2 + num3);
				float num5 = Mathf.Min(num4 / (agentHeight + agentRadius * 2f * num), num4 / (agentRadius * 2f));
				float num6 = agentHeight * num5;
				float num7 = agentRadius * num5;
				float num8 = agentClimb * num5;
				float num9 = rect.xMin + rect.width * 0.5f;
				float num10 = rect.yMax - num3 - num7 * num;
				Vector3[] array = new Vector3[40];
				Vector3[] array2 = new Vector3[20];
				Vector3[] array3 = new Vector3[20];
				for (int i = 0; i < 20; i++)
				{
					float f = (float)i / 19f * 3.14159274f;
					float num11 = Mathf.Cos(f);
					float num12 = Mathf.Sin(f);
					array[i] = new Vector3(num9 + num11 * num7, num10 - num6 - num12 * num7 * num, 0f);
					array[i + 20] = new Vector3(num9 - num11 * num7, num10 + num12 * num7 * num, 0f);
					array2[i] = new Vector3(num9 - num11 * num7, num10 - num6 + num12 * num7 * num, 0f);
					array3[i] = new Vector3(num9 - num11 * num7, num10 - num8 + num12 * num7 * num, 0f);
				}
				Color color = Handles.color;
				float xMin = rect.xMin;
				float num13 = num10 - num8;
				float num14 = num9 - num4 * 0.75f;
				float y = num10;
				float num15 = num9 + num4 * 0.75f;
				float num16 = num10;
				float num17 = num15;
				float num18 = num16;
				float num19 = Mathf.Min(rect.xMax - num15, num6);
				num17 += Mathf.Cos(agentSlope * 0.0174532924f) * num19;
				num18 -= Mathf.Sin(agentSlope * 0.0174532924f) * num19;
				Vector3[] points = new Vector3[]
				{
					new Vector3(xMin, num10, 0f),
					new Vector3(num17, num10, 0f)
				};
				Vector3[] points2 = new Vector3[]
				{
					new Vector3(xMin, num13, 0f),
					new Vector3(num14, num13, 0f),
					new Vector3(num14, y, 0f),
					new Vector3(num15, num16, 0f),
					new Vector3(num17, num18, 0f)
				};
				Handles.color = ((!EditorGUIUtility.isProSkin) ? new Color(1f, 1f, 1f, 0.5f) : new Color(0f, 0f, 0f, 0.5f));
				Handles.DrawAAPolyLine(2f, points);
				Handles.color = ((!EditorGUIUtility.isProSkin) ? new Color(0f, 0f, 0f, 0.5f) : new Color(1f, 1f, 1f, 0.5f));
				Handles.DrawAAPolyLine(3f, points2);
				Handles.color = Color.Lerp(new Color(0f, 0.75f, 1f, 1f), new Color(0.5f, 0.5f, 0.5f, 0.5f), 0.2f);
				Handles.DrawAAConvexPolygon(array);
				Handles.color = new Color(0f, 0f, 0f, 0.5f);
				Handles.DrawAAPolyLine(2f, array3);
				Handles.color = new Color(1f, 1f, 1f, 0.4f);
				Handles.DrawAAPolyLine(2f, array2);
				Vector3[] points3 = new Vector3[]
				{
					new Vector3(num9, num10 - num6, 0f),
					new Vector3(num9 + num7, num10 - num6, 0f)
				};
				Handles.color = new Color(0f, 0f, 0f, 0.5f);
				Handles.DrawAAPolyLine(2f, points3);
				GUI.Label(new Rect(num9 + num7 + 5f, num10 - num6 * 0.5f - 10f, 150f, 20f), string.Format("H = {0}", agentHeight));
				GUI.Label(new Rect(num9, num10 - num6 - num7 * num - 15f, 150f, 20f), string.Format("R = {0}", agentRadius));
				GUI.Label(new Rect((xMin + num14) * 0.5f - 20f, num13 - 15f, 150f, 20f), string.Format("{0}", agentClimb));
				GUI.Label(new Rect(num15 + 20f, num16 - 15f, 150f, 20f), string.Format("{0}°", agentSlope));
				Handles.color = color;
			}
		}
	}
}
