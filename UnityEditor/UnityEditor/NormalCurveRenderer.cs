using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class NormalCurveRenderer : CurveRenderer
	{
		private const float kSegmentWindowResolution = 1000f;

		private const int kMaximumSampleCount = 50;

		private const int kMaximumLoops = 100;

		private AnimationCurve m_Curve;

		private float m_CustomRangeStart;

		private float m_CustomRangeEnd;

		private WrapMode preWrapMode = WrapMode.Once;

		private WrapMode postWrapMode = WrapMode.Once;

		private float rangeStart
		{
			get
			{
				return (this.m_CustomRangeStart != 0f || this.m_CustomRangeEnd != 0f || this.m_Curve.length <= 0) ? this.m_CustomRangeStart : this.m_Curve.keys[0].time;
			}
		}

		private float rangeEnd
		{
			get
			{
				return (this.m_CustomRangeStart != 0f || this.m_CustomRangeEnd != 0f || this.m_Curve.length <= 0) ? this.m_CustomRangeEnd : this.m_Curve.keys[this.m_Curve.length - 1].time;
			}
		}

		public NormalCurveRenderer(AnimationCurve curve)
		{
			this.m_Curve = curve;
			if (this.m_Curve == null)
			{
				this.m_Curve = new AnimationCurve();
			}
		}

		public AnimationCurve GetCurve()
		{
			return this.m_Curve;
		}

		public float RangeStart()
		{
			return this.rangeStart;
		}

		public float RangeEnd()
		{
			return this.rangeEnd;
		}

		public void SetWrap(WrapMode wrap)
		{
			this.preWrapMode = wrap;
			this.postWrapMode = wrap;
		}

		public void SetWrap(WrapMode preWrap, WrapMode postWrap)
		{
			this.preWrapMode = preWrap;
			this.postWrapMode = postWrap;
		}

		public void SetCustomRange(float start, float end)
		{
			this.m_CustomRangeStart = start;
			this.m_CustomRangeEnd = end;
		}

		public float EvaluateCurveSlow(float time)
		{
			return this.m_Curve.Evaluate(time);
		}

		public float EvaluateCurveDeltaSlow(float time)
		{
			float num = 0.0001f;
			return (this.m_Curve.Evaluate(time + num) - this.m_Curve.Evaluate(time - num)) / (num * 2f);
		}

		private Vector3[] GetPoints(float minTime, float maxTime)
		{
			List<Vector3> list = new List<Vector3>();
			if (this.m_Curve.length == 0)
			{
				return list.ToArray();
			}
			list.Capacity = 1000 + this.m_Curve.length;
			float[,] array = NormalCurveRenderer.CalculateRanges(minTime, maxTime, this.rangeStart, this.rangeEnd, this.preWrapMode, this.postWrapMode);
			for (int i = 0; i < array.GetLength(0); i++)
			{
				this.AddPoints(ref list, array[i, 0], array[i, 1], minTime, maxTime);
			}
			if (list.Count > 0)
			{
				for (int j = 1; j < list.Count; j++)
				{
					if (list[j].x < list[j - 1].x)
					{
						list.RemoveAt(j);
						j--;
					}
				}
			}
			return list.ToArray();
		}

		public static float[,] CalculateRanges(float minTime, float maxTime, float rangeStart, float rangeEnd, WrapMode preWrapMode, WrapMode postWrapMode)
		{
			if (postWrapMode != preWrapMode)
			{
				float[,] expr_12 = new float[1, 2];
				expr_12[0, 0] = rangeStart;
				expr_12[0, 1] = rangeEnd;
				return expr_12;
			}
			if (preWrapMode == WrapMode.Loop)
			{
				if (maxTime - minTime > rangeEnd - rangeStart)
				{
					float[,] expr_3E = new float[1, 2];
					expr_3E[0, 0] = rangeStart;
					expr_3E[0, 1] = rangeEnd;
					return expr_3E;
				}
				minTime = Mathf.Repeat(minTime - rangeStart, rangeEnd - rangeStart) + rangeStart;
				maxTime = Mathf.Repeat(maxTime - rangeStart, rangeEnd - rangeStart) + rangeStart;
				if (minTime < maxTime)
				{
					float[,] expr_7D = new float[1, 2];
					expr_7D[0, 0] = minTime;
					expr_7D[0, 1] = maxTime;
					return expr_7D;
				}
				float[,] expr_97 = new float[2, 2];
				expr_97[0, 0] = rangeStart;
				expr_97[0, 1] = maxTime;
				expr_97[1, 0] = minTime;
				expr_97[1, 1] = rangeEnd;
				return expr_97;
			}
			else
			{
				if (preWrapMode == WrapMode.PingPong)
				{
					float[,] expr_CA = new float[1, 2];
					expr_CA[0, 0] = rangeStart;
					expr_CA[0, 1] = rangeEnd;
					return expr_CA;
				}
				float[,] expr_E4 = new float[1, 2];
				expr_E4[0, 0] = minTime;
				expr_E4[0, 1] = maxTime;
				return expr_E4;
			}
		}

		private static int GetSegmentResolution(float minTime, float maxTime, float keyTime, float nextKeyTime)
		{
			float num = maxTime - minTime;
			float num2 = nextKeyTime - keyTime;
			int value = Mathf.RoundToInt(1000f * (num2 / num));
			return Mathf.Clamp(value, 1, 50);
		}

		private void AddPoints(ref List<Vector3> points, float minTime, float maxTime, float visibleMinTime, float visibleMaxTime)
		{
			if (this.m_Curve[0].time >= minTime)
			{
				points.Add(new Vector3(this.rangeStart, this.m_Curve[0].value));
				points.Add(new Vector3(this.m_Curve[0].time, this.m_Curve[0].value));
			}
			for (int i = 0; i < this.m_Curve.length - 1; i++)
			{
				Keyframe keyframe = this.m_Curve[i];
				Keyframe keyframe2 = this.m_Curve[i + 1];
				if (keyframe2.time >= minTime && keyframe.time <= maxTime)
				{
					points.Add(new Vector3(keyframe.time, keyframe.value));
					int segmentResolution = NormalCurveRenderer.GetSegmentResolution(visibleMinTime, visibleMaxTime, keyframe.time, keyframe2.time);
					float num = Mathf.Lerp(keyframe.time, keyframe2.time, 0.001f / (float)segmentResolution);
					points.Add(new Vector3(num, this.m_Curve.Evaluate(num)));
					for (float num2 = 1f; num2 < (float)segmentResolution; num2 += 1f)
					{
						num = Mathf.Lerp(keyframe.time, keyframe2.time, num2 / (float)segmentResolution);
						points.Add(new Vector3(num, this.m_Curve.Evaluate(num)));
					}
					num = Mathf.Lerp(keyframe.time, keyframe2.time, 1f - 0.001f / (float)segmentResolution);
					points.Add(new Vector3(num, this.m_Curve.Evaluate(num)));
					num = keyframe2.time;
					points.Add(new Vector3(num, keyframe2.value));
				}
			}
			if (this.m_Curve[this.m_Curve.length - 1].time <= maxTime)
			{
				points.Add(new Vector3(this.m_Curve[this.m_Curve.length - 1].time, this.m_Curve[this.m_Curve.length - 1].value));
				points.Add(new Vector3(this.rangeEnd, this.m_Curve[this.m_Curve.length - 1].value));
			}
		}

		public void DrawCurve(float minTime, float maxTime, Color color, Matrix4x4 transform, Color wrapColor)
		{
			Vector3[] points = this.GetPoints(minTime, maxTime);
			NormalCurveRenderer.DrawCurveWrapped(minTime, maxTime, this.rangeStart, this.rangeEnd, this.preWrapMode, this.postWrapMode, color, transform, points, wrapColor);
		}

		public static void DrawPolyLine(Matrix4x4 transform, float minDistance, params Vector3[] points)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			Color c = Handles.color * new Color(1f, 1f, 1f, 0.75f);
			HandleUtility.ApplyWireMaterial();
			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(1);
			GL.Color(c);
			Vector3 vector = transform.MultiplyPoint(points[0]);
			for (int i = 1; i < points.Length; i++)
			{
				Vector3 vector2 = transform.MultiplyPoint(points[i]);
				if ((vector - vector2).magnitude > minDistance)
				{
					GL.Vertex(vector);
					GL.Vertex(vector2);
					vector = vector2;
				}
			}
			GL.End();
			GL.PopMatrix();
		}

		public static void DrawCurveWrapped(float minTime, float maxTime, float rangeStart, float rangeEnd, WrapMode preWrap, WrapMode postWrap, Color color, Matrix4x4 transform, Vector3[] points, Color wrapColor)
		{
			if (points.Length == 0)
			{
				return;
			}
			int num;
			int num2;
			if (rangeEnd - rangeStart != 0f)
			{
				num = Mathf.FloorToInt((minTime - rangeStart) / (rangeEnd - rangeStart));
				num2 = Mathf.CeilToInt((maxTime - rangeEnd) / (rangeEnd - rangeStart));
				if (num < -100)
				{
					preWrap = WrapMode.Once;
				}
				if (num2 > 100)
				{
					postWrap = WrapMode.Once;
				}
			}
			else
			{
				preWrap = WrapMode.Once;
				postWrap = WrapMode.Once;
				num = ((minTime >= rangeStart) ? 0 : -1);
				num2 = ((maxTime <= rangeEnd) ? 0 : 1);
			}
			int num3 = points.Length - 1;
			Handles.color = color;
			List<Vector3> list = new List<Vector3>();
			if (num <= 0 && num2 >= 0)
			{
				NormalCurveRenderer.DrawPolyLine(transform, 2f, points);
			}
			else
			{
				Handles.DrawPolyLine(points);
			}
			Handles.color = new Color(wrapColor.r, wrapColor.g, wrapColor.b, wrapColor.a * color.a);
			if (preWrap == WrapMode.Loop)
			{
				list = new List<Vector3>();
				for (int i = num; i < 0; i++)
				{
					for (int j = 0; j < points.Length; j++)
					{
						Vector3 vector = points[j];
						vector.x += (float)i * (rangeEnd - rangeStart);
						vector = transform.MultiplyPoint(vector);
						list.Add(vector);
					}
				}
				list.Add(transform.MultiplyPoint(points[0]));
				Handles.DrawPolyLine(list.ToArray());
			}
			else if (preWrap == WrapMode.PingPong)
			{
				list = new List<Vector3>();
				for (int k = num; k < 0; k++)
				{
					for (int l = 0; l < points.Length; l++)
					{
						if ((float)(k / 2) == (float)k / 2f)
						{
							Vector3 vector2 = points[l];
							vector2.x += (float)k * (rangeEnd - rangeStart);
							vector2 = transform.MultiplyPoint(vector2);
							list.Add(vector2);
						}
						else
						{
							Vector3 vector3 = points[num3 - l];
							vector3.x = -vector3.x + (float)(k + 1) * (rangeEnd - rangeStart) + rangeStart * 2f;
							vector3 = transform.MultiplyPoint(vector3);
							list.Add(vector3);
						}
					}
				}
				Handles.DrawPolyLine(list.ToArray());
			}
			else if (num < 0)
			{
				Handles.DrawPolyLine(new Vector3[]
				{
					transform.MultiplyPoint(new Vector3(minTime, points[0].y, 0f)),
					transform.MultiplyPoint(new Vector3(Mathf.Min(maxTime, points[0].x), points[0].y, 0f))
				});
			}
			if (postWrap == WrapMode.Loop)
			{
				list = new List<Vector3>();
				list.Add(transform.MultiplyPoint(points[num3]));
				for (int m = 1; m <= num2; m++)
				{
					for (int n = 0; n < points.Length; n++)
					{
						Vector3 vector4 = points[n];
						vector4.x += (float)m * (rangeEnd - rangeStart);
						vector4 = transform.MultiplyPoint(vector4);
						list.Add(vector4);
					}
				}
				Handles.DrawPolyLine(list.ToArray());
			}
			else if (postWrap == WrapMode.PingPong)
			{
				list = new List<Vector3>();
				for (int num4 = 1; num4 <= num2; num4++)
				{
					for (int num5 = 0; num5 < points.Length; num5++)
					{
						if ((float)(num4 / 2) == (float)num4 / 2f)
						{
							Vector3 vector5 = points[num5];
							vector5.x += (float)num4 * (rangeEnd - rangeStart);
							vector5 = transform.MultiplyPoint(vector5);
							list.Add(vector5);
						}
						else
						{
							Vector3 vector6 = points[num3 - num5];
							vector6.x = -vector6.x + (float)(num4 + 1) * (rangeEnd - rangeStart) + rangeStart * 2f;
							vector6 = transform.MultiplyPoint(vector6);
							list.Add(vector6);
						}
					}
				}
				Handles.DrawPolyLine(list.ToArray());
			}
			else if (num2 > 0)
			{
				Handles.DrawPolyLine(new Vector3[]
				{
					transform.MultiplyPoint(new Vector3(Mathf.Max(minTime, points[num3].x), points[num3].y, 0f)),
					transform.MultiplyPoint(new Vector3(maxTime, points[num3].y, 0f))
				});
			}
		}

		public Bounds GetBounds()
		{
			return this.GetBounds(this.rangeStart, this.rangeEnd);
		}

		public Bounds GetBounds(float minTime, float maxTime)
		{
			Vector3[] points = this.GetPoints(minTime, maxTime);
			float num = float.PositiveInfinity;
			float num2 = float.NegativeInfinity;
			Vector3[] array = points;
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 vector = array[i];
				if (vector.y > num2)
				{
					num2 = vector.y;
				}
				if (vector.y < num)
				{
					num = vector.y;
				}
			}
			if (num == float.PositiveInfinity)
			{
				num = 0f;
				num2 = 0f;
			}
			return new Bounds(new Vector3((maxTime + minTime) * 0.5f, (num2 + num) * 0.5f, 0f), new Vector3(maxTime - minTime, num2 - num, 0f));
		}
	}
}
