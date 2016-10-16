using System;
using UnityEngine;

namespace UnityEditor
{
	internal struct QuaternionCurveTangentCalculation
	{
		private AnimationCurve eulerX;

		private AnimationCurve eulerY;

		private AnimationCurve eulerZ;

		public AnimationCurve GetCurve(int index)
		{
			if (index == 0)
			{
				return this.eulerX;
			}
			if (index == 1)
			{
				return this.eulerY;
			}
			return this.eulerZ;
		}

		public void SetCurve(int index, AnimationCurve curve)
		{
			if (index == 0)
			{
				this.eulerX = curve;
			}
			else if (index == 1)
			{
				this.eulerY = curve;
			}
			else
			{
				this.eulerZ = curve;
			}
		}

		private Vector3 EvaluateEulerCurvesDirectly(float time)
		{
			return new Vector3(this.eulerX.Evaluate(time), this.eulerY.Evaluate(time), this.eulerZ.Evaluate(time));
		}

		public float CalculateLinearTangent(int fromIndex, int toIndex, int componentIndex)
		{
			AnimationCurve curve = this.GetCurve(componentIndex);
			return this.CalculateLinearTangent(curve[fromIndex], curve[toIndex], componentIndex);
		}

		public float CalculateLinearTangent(Keyframe from, Keyframe to, int component)
		{
			float num = 0.01f;
			Vector3 vector = this.EvaluateEulerCurvesDirectly(to.time);
			Vector3 euler = this.EvaluateEulerCurvesDirectly(from.time);
			Quaternion a = Quaternion.Euler(vector);
			Quaternion b = Quaternion.Euler(euler);
			Quaternion q = Quaternion.Slerp(a, b, num);
			Vector3 eulerFromQuaternion = QuaternionCurveTangentCalculation.GetEulerFromQuaternion(q, vector);
			switch (component)
			{
			case 0:
				return (eulerFromQuaternion.x - vector.x) / num / -(to.time - from.time);
			case 1:
				return (eulerFromQuaternion.y - vector.y) / num / -(to.time - from.time);
			case 2:
				return (eulerFromQuaternion.z - vector.z) / num / -(to.time - from.time);
			default:
				return 0f;
			}
		}

		public float CalculateSmoothTangent(int index, int component)
		{
			AnimationCurve curve = this.GetCurve(component);
			if (curve.length < 2)
			{
				return 0f;
			}
			if (index <= 0)
			{
				return this.CalculateLinearTangent(curve[0], curve[1], component);
			}
			if (index >= curve.length - 1)
			{
				return this.CalculateLinearTangent(curve[curve.length - 1], curve[curve.length - 2], component);
			}
			float time = curve[index - 1].time;
			float time2 = curve[index].time;
			float time3 = curve[index + 1].time;
			Vector3 euler = this.EvaluateEulerCurvesDirectly(time);
			Vector3 vector = this.EvaluateEulerCurvesDirectly(time2);
			Vector3 euler2 = this.EvaluateEulerCurvesDirectly(time3);
			Quaternion quaternion = Quaternion.Euler(euler);
			Quaternion quaternion2 = Quaternion.Euler(vector);
			Quaternion quaternion3 = Quaternion.Euler(euler2);
			if (quaternion.x * quaternion2.x + quaternion.y * quaternion2.y + quaternion.z * quaternion2.z + quaternion.w * quaternion2.w < 0f)
			{
				quaternion = new Quaternion(-quaternion.x, -quaternion.y, -quaternion.z, -quaternion.w);
			}
			if (quaternion3.x * quaternion2.x + quaternion3.y * quaternion2.y + quaternion3.z * quaternion2.z + quaternion3.w * quaternion2.w < 0f)
			{
				quaternion3 = new Quaternion(-quaternion3.x, -quaternion3.y, -quaternion3.z, -quaternion3.w);
			}
			Quaternion quaternion4 = default(Quaternion);
			float dx = time2 - time;
			float dx2 = time3 - time2;
			for (int i = 0; i < 4; i++)
			{
				float dy = quaternion2[i] - quaternion[i];
				float dy2 = quaternion3[i] - quaternion2[i];
				float num = QuaternionCurveTangentCalculation.SafeDeltaDivide(dy, dx);
				float num2 = QuaternionCurveTangentCalculation.SafeDeltaDivide(dy2, dx2);
				quaternion4[i] = 0.5f * num + 0.5f * num2;
			}
			float num3 = Mathf.Abs(time3 - time) * 0.01f;
			Quaternion q = new Quaternion(quaternion2.x - quaternion4.x * num3, quaternion2.y - quaternion4.y * num3, quaternion2.z - quaternion4.z * num3, quaternion2.w - quaternion4.w * num3);
			Quaternion q2 = new Quaternion(quaternion2.x + quaternion4.x * num3, quaternion2.y + quaternion4.y * num3, quaternion2.z + quaternion4.z * num3, quaternion2.w + quaternion4.w * num3);
			Vector3 eulerFromQuaternion = QuaternionCurveTangentCalculation.GetEulerFromQuaternion(q, vector);
			Vector3 eulerFromQuaternion2 = QuaternionCurveTangentCalculation.GetEulerFromQuaternion(q2, vector);
			return ((eulerFromQuaternion2 - eulerFromQuaternion) / (num3 * 2f))[component];
		}

		public static Vector3[] GetEquivalentEulerAngles(Quaternion quat)
		{
			Vector3 eulerAngles = quat.eulerAngles;
			return new Vector3[]
			{
				eulerAngles,
				new Vector3(180f - eulerAngles.x, eulerAngles.y + 180f, eulerAngles.z + 180f)
			};
		}

		public static Vector3 GetEulerFromQuaternion(Quaternion q, Vector3 refEuler)
		{
			Vector3[] equivalentEulerAngles = QuaternionCurveTangentCalculation.GetEquivalentEulerAngles(q);
			for (int i = 0; i < equivalentEulerAngles.Length; i++)
			{
				equivalentEulerAngles[i] = new Vector3(Mathf.Repeat(equivalentEulerAngles[i].x - refEuler.x + 180f, 360f) + refEuler.x - 180f, Mathf.Repeat(equivalentEulerAngles[i].y - refEuler.y + 180f, 360f) + refEuler.y - 180f, Mathf.Repeat(equivalentEulerAngles[i].z - refEuler.z + 180f, 360f) + refEuler.z - 180f);
				float num = Mathf.Repeat(equivalentEulerAngles[i].x, 360f);
				if (Mathf.Abs(num - 90f) < 1f)
				{
					float num2 = equivalentEulerAngles[i].z - equivalentEulerAngles[i].y;
					float num3 = refEuler.z - refEuler.y;
					float num4 = num2 - num3;
					equivalentEulerAngles[i].z = refEuler.z + num4 * 0.5f;
					equivalentEulerAngles[i].y = refEuler.y - num4 * 0.5f;
				}
				if (Mathf.Abs(num - 270f) < 1f)
				{
					float num5 = equivalentEulerAngles[i].z + equivalentEulerAngles[i].y;
					float num6 = refEuler.z + refEuler.y;
					float num7 = num5 - num6;
					equivalentEulerAngles[i].z = refEuler.z + num7 * 0.5f;
					equivalentEulerAngles[i].y = refEuler.y + num7 * 0.5f;
				}
			}
			Vector3 result = equivalentEulerAngles[0];
			float num8 = (equivalentEulerAngles[0] - refEuler).sqrMagnitude;
			for (int j = 1; j < equivalentEulerAngles.Length; j++)
			{
				float sqrMagnitude = (equivalentEulerAngles[j] - refEuler).sqrMagnitude;
				if (sqrMagnitude < num8)
				{
					num8 = sqrMagnitude;
					result = equivalentEulerAngles[j];
				}
			}
			return result;
		}

		public static float SafeDeltaDivide(float dy, float dx)
		{
			if (dx == 0f)
			{
				return 0f;
			}
			return dy / dx;
		}

		public void UpdateTangentsFromMode(int componentIndex)
		{
			AnimationCurve curve = this.GetCurve(componentIndex);
			for (int i = 0; i < curve.length; i++)
			{
				this.UpdateTangentsFromMode(i, componentIndex);
			}
		}

		public void UpdateTangentsFromMode(int index, int componentIndex)
		{
			AnimationCurve curve = this.GetCurve(componentIndex);
			if (index < 0 || index >= curve.length)
			{
				return;
			}
			Keyframe key = curve[index];
			if (CurveUtility.GetKeyTangentMode(key, 0) == TangentMode.Linear && index >= 1)
			{
				key.inTangent = this.CalculateLinearTangent(index, index - 1, componentIndex);
				curve.MoveKey(index, key);
			}
			if (CurveUtility.GetKeyTangentMode(key, 1) == TangentMode.Linear && index + 1 < curve.length)
			{
				key.outTangent = this.CalculateLinearTangent(index, index + 1, componentIndex);
				curve.MoveKey(index, key);
			}
			if (CurveUtility.GetKeyTangentMode(key, 0) == TangentMode.Smooth || CurveUtility.GetKeyTangentMode(key, 1) == TangentMode.Smooth)
			{
				float num = this.CalculateSmoothTangent(index, componentIndex);
				key.outTangent = num;
				key.inTangent = num;
				curve.MoveKey(index, key);
			}
		}

		public static void UpdateTangentsFromMode(AnimationCurve curve, AnimationClip clip, EditorCurveBinding curveBinding)
		{
			CurveUtility.UpdateTangentsFromMode(curve);
		}
	}
}
