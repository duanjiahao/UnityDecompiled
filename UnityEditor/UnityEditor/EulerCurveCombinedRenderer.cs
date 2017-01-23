using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class EulerCurveCombinedRenderer
	{
		private const int kSegmentResolution = 40;

		private const float epsilon = 0.001f;

		private AnimationCurve quaternionX;

		private AnimationCurve quaternionY;

		private AnimationCurve quaternionZ;

		private AnimationCurve quaternionW;

		private AnimationCurve eulerX;

		private AnimationCurve eulerY;

		private AnimationCurve eulerZ;

		private SortedDictionary<float, Vector3> points;

		private float cachedEvaluationTime = float.PositiveInfinity;

		private Vector3 cachedEvaluationValue;

		private float cachedRangeStart = float.PositiveInfinity;

		private float cachedRangeEnd = float.NegativeInfinity;

		private Vector3 refEuler;

		private float m_CustomRangeStart = 0f;

		private float m_CustomRangeEnd = 0f;

		private WrapMode preWrapMode = WrapMode.Once;

		private WrapMode postWrapMode = WrapMode.Once;

		private float rangeStart
		{
			get
			{
				return (this.m_CustomRangeStart != 0f || this.m_CustomRangeEnd != 0f || this.eulerX.length <= 0) ? this.m_CustomRangeStart : this.eulerX.keys[0].time;
			}
		}

		private float rangeEnd
		{
			get
			{
				return (this.m_CustomRangeStart != 0f || this.m_CustomRangeEnd != 0f || this.eulerX.length <= 0) ? this.m_CustomRangeEnd : this.eulerX.keys[this.eulerX.length - 1].time;
			}
		}

		public EulerCurveCombinedRenderer(AnimationCurve quaternionX, AnimationCurve quaternionY, AnimationCurve quaternionZ, AnimationCurve quaternionW, AnimationCurve eulerX, AnimationCurve eulerY, AnimationCurve eulerZ)
		{
			this.quaternionX = ((quaternionX != null) ? quaternionX : new AnimationCurve());
			this.quaternionY = ((quaternionY != null) ? quaternionY : new AnimationCurve());
			this.quaternionZ = ((quaternionZ != null) ? quaternionZ : new AnimationCurve());
			this.quaternionW = ((quaternionW != null) ? quaternionW : new AnimationCurve());
			this.eulerX = ((eulerX != null) ? eulerX : new AnimationCurve());
			this.eulerY = ((eulerY != null) ? eulerY : new AnimationCurve());
			this.eulerZ = ((eulerZ != null) ? eulerZ : new AnimationCurve());
		}

		public AnimationCurve GetCurveOfComponent(int component)
		{
			AnimationCurve result;
			switch (component)
			{
			case 0:
				result = this.eulerX;
				break;
			case 1:
				result = this.eulerY;
				break;
			case 2:
				result = this.eulerZ;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		public float RangeStart()
		{
			return this.rangeStart;
		}

		public float RangeEnd()
		{
			return this.rangeEnd;
		}

		public WrapMode PreWrapMode()
		{
			return this.preWrapMode;
		}

		public WrapMode PostWrapMode()
		{
			return this.postWrapMode;
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

		private Vector3 GetValues(float time, bool keyReference)
		{
			if (this.quaternionX == null)
			{
				Debug.LogError("X curve is null!");
			}
			if (this.quaternionY == null)
			{
				Debug.LogError("Y curve is null!");
			}
			if (this.quaternionZ == null)
			{
				Debug.LogError("Z curve is null!");
			}
			if (this.quaternionW == null)
			{
				Debug.LogError("W curve is null!");
			}
			if (this.quaternionX.length != 0 && this.quaternionY.length != 0 && this.quaternionZ.length != 0 && this.quaternionW.length != 0)
			{
				Quaternion q = this.EvaluateQuaternionCurvesDirectly(time);
				if (keyReference)
				{
					this.refEuler = this.EvaluateEulerCurvesDirectly(time);
				}
				this.refEuler = QuaternionCurveTangentCalculation.GetEulerFromQuaternion(q, this.refEuler);
			}
			else
			{
				this.refEuler = this.EvaluateEulerCurvesDirectly(time);
			}
			return this.refEuler;
		}

		private Quaternion EvaluateQuaternionCurvesDirectly(float time)
		{
			return new Quaternion(this.quaternionX.Evaluate(time), this.quaternionY.Evaluate(time), this.quaternionZ.Evaluate(time), this.quaternionW.Evaluate(time));
		}

		private Vector3 EvaluateEulerCurvesDirectly(float time)
		{
			return new Vector3(this.eulerX.Evaluate(time), this.eulerY.Evaluate(time), this.eulerZ.Evaluate(time));
		}

		private void CalculateCurves(float minTime, float maxTime)
		{
			this.points = new SortedDictionary<float, Vector3>();
			float[,] array = NormalCurveRenderer.CalculateRanges(minTime, maxTime, this.rangeStart, this.rangeEnd, this.preWrapMode, this.postWrapMode);
			for (int i = 0; i < array.GetLength(0); i++)
			{
				this.AddPoints(array[i, 0], array[i, 1]);
			}
		}

		private void AddPoints(float minTime, float maxTime)
		{
			AnimationCurve animationCurve = this.quaternionX;
			if (animationCurve.length == 0)
			{
				animationCurve = this.eulerX;
			}
			if (animationCurve.length != 0)
			{
				if (animationCurve[0].time >= minTime)
				{
					Vector3 values = this.GetValues(animationCurve[0].time, true);
					this.points[this.rangeStart] = values;
					this.points[animationCurve[0].time] = values;
				}
				if (animationCurve[animationCurve.length - 1].time <= maxTime)
				{
					Vector3 values2 = this.GetValues(animationCurve[animationCurve.length - 1].time, true);
					this.points[animationCurve[animationCurve.length - 1].time] = values2;
					this.points[this.rangeEnd] = values2;
				}
				for (int i = 0; i < animationCurve.length - 1; i++)
				{
					if (animationCurve[i + 1].time >= minTime && animationCurve[i].time <= maxTime)
					{
						float num = animationCurve[i].time;
						this.points[num] = this.GetValues(num, true);
						for (float num2 = 1f; num2 <= 20f; num2 += 1f)
						{
							num = Mathf.Lerp(animationCurve[i].time, animationCurve[i + 1].time, (num2 - 0.001f) / 40f);
							this.points[num] = this.GetValues(num, false);
						}
						num = animationCurve[i + 1].time;
						this.points[num] = this.GetValues(num, true);
						for (float num3 = 1f; num3 <= 20f; num3 += 1f)
						{
							num = Mathf.Lerp(animationCurve[i].time, animationCurve[i + 1].time, 1f - (num3 - 0.001f) / 40f);
							this.points[num] = this.GetValues(num, false);
						}
					}
				}
			}
		}

		public float EvaluateCurveDeltaSlow(float time, int component)
		{
			float result;
			if (this.quaternionX == null)
			{
				result = 0f;
			}
			else
			{
				result = (this.EvaluateCurveSlow(time + 0.001f, component) - this.EvaluateCurveSlow(time - 0.001f, component)) / 0.002f;
			}
			return result;
		}

		public float EvaluateCurveSlow(float time, int component)
		{
			float result;
			if (this.GetCurveOfComponent(component).length == 1)
			{
				result = this.GetCurveOfComponent(component)[0].value;
			}
			else if (time == this.cachedEvaluationTime)
			{
				result = this.cachedEvaluationValue[component];
			}
			else
			{
				if (time < this.cachedRangeStart || time > this.cachedRangeEnd)
				{
					this.CalculateCurves(this.rangeStart, this.rangeEnd);
					this.cachedRangeStart = float.NegativeInfinity;
					this.cachedRangeEnd = float.PositiveInfinity;
				}
				float[] array = new float[this.points.Count];
				Vector3[] array2 = new Vector3[this.points.Count];
				int num = 0;
				foreach (KeyValuePair<float, Vector3> current in this.points)
				{
					array[num] = current.Key;
					array2[num] = current.Value;
					num++;
				}
				for (int i = 0; i < array.Length - 1; i++)
				{
					if (time < array[i + 1])
					{
						float t = Mathf.InverseLerp(array[i], array[i + 1], time);
						this.cachedEvaluationValue = Vector3.Lerp(array2[i], array2[i + 1], t);
						this.cachedEvaluationTime = time;
						result = this.cachedEvaluationValue[component];
						return result;
					}
				}
				if (array2.Length > 0)
				{
					result = array2[array2.Length - 1][component];
				}
				else
				{
					Debug.LogError("List of euler curve points is empty, probably caused by lack of euler curve key synching");
					result = 0f;
				}
			}
			return result;
		}

		public void DrawCurve(float minTime, float maxTime, Color color, Matrix4x4 transform, int component, Color wrapColor)
		{
			if (minTime < this.cachedRangeStart || maxTime > this.cachedRangeEnd)
			{
				this.CalculateCurves(minTime, maxTime);
				if (minTime <= this.rangeStart && maxTime >= this.rangeEnd)
				{
					this.cachedRangeStart = float.NegativeInfinity;
					this.cachedRangeEnd = float.PositiveInfinity;
				}
				else
				{
					this.cachedRangeStart = minTime;
					this.cachedRangeEnd = maxTime;
				}
			}
			List<Vector3> list = new List<Vector3>();
			foreach (KeyValuePair<float, Vector3> current in this.points)
			{
				list.Add(new Vector3(current.Key, current.Value[component]));
			}
			NormalCurveRenderer.DrawCurveWrapped(minTime, maxTime, this.rangeStart, this.rangeEnd, this.preWrapMode, this.postWrapMode, color, transform, list.ToArray(), wrapColor);
		}

		public Bounds GetBounds(float minTime, float maxTime, int component)
		{
			this.CalculateCurves(minTime, maxTime);
			float num = float.PositiveInfinity;
			float num2 = float.NegativeInfinity;
			foreach (KeyValuePair<float, Vector3> current in this.points)
			{
				if (current.Value[component] > num2)
				{
					num2 = current.Value[component];
				}
				if (current.Value[component] < num)
				{
					num = current.Value[component];
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
