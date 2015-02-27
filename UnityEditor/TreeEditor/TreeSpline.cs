using System;
using UnityEditor;
using UnityEngine;
namespace TreeEditor
{
	[Serializable]
	public class TreeSpline
	{
		public SplineNode[] nodes = new SplineNode[0];
		public float tension = 0.5f;
		public TreeSpline()
		{
		}
		public TreeSpline(TreeSpline o)
		{
			this.nodes = new SplineNode[o.nodes.Length];
			for (int i = 0; i < o.nodes.Length; i++)
			{
				this.nodes[i] = new SplineNode(o.nodes[i]);
			}
			this.tension = o.tension;
		}
		private void OnDisable()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
			}
		}
		public void Reset()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
			}
			this.nodes = new SplineNode[0];
		}
		public int GetNodeCount()
		{
			return this.nodes.Length;
		}
		public void SetNodeCount(int c)
		{
			if (c < this.nodes.Length)
			{
				SplineNode[] array = new SplineNode[c];
				for (int i = 0; i < c; i++)
				{
					array[i] = this.nodes[i];
				}
				for (int j = c; j < this.nodes.Length; j++)
				{
				}
				this.nodes = array;
			}
		}
		public void RemoveNode(int c)
		{
			if (c < 0 || c >= this.nodes.Length)
			{
				return;
			}
			SplineNode[] array = new SplineNode[this.nodes.Length - 1];
			int num = 0;
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (i != c)
				{
					array[num] = this.nodes[i];
					num++;
				}
			}
			this.nodes = array;
		}
		public SplineNode[] GetNodes()
		{
			return this.nodes;
		}
		public void AddPoint(Vector3 pos, float timeInSeconds)
		{
			SplineNode[] array = new SplineNode[this.nodes.Length + 1];
			for (int i = 0; i < this.nodes.Length; i++)
			{
				array[i] = this.nodes[i];
			}
			this.nodes = array;
			SplineNode splineNode = new SplineNode(pos, timeInSeconds);
			this.nodes[this.nodes.Length - 1] = splineNode;
		}
		public float GetApproximateLength()
		{
			if (this.nodes.Length < 2)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 1; i < this.nodes.Length; i++)
			{
				float magnitude = (this.nodes[i - 1].point - this.nodes[i].point).magnitude;
				num += magnitude;
			}
			return num;
		}
		public void UpdateTime()
		{
			if (this.nodes.Length < 2)
			{
				return;
			}
			float approximateLength = this.GetApproximateLength();
			float num = 0f;
			this.nodes[0].time = num;
			for (int i = 1; i < this.nodes.Length; i++)
			{
				float magnitude = (this.nodes[i - 1].point - this.nodes[i].point).magnitude;
				num += magnitude;
				this.nodes[i].time = num / approximateLength;
			}
		}
		public void UpdateRotations()
		{
			if (this.nodes.Length < 2)
			{
				return;
			}
			Matrix4x4 m = Matrix4x4.identity;
			this.nodes[0].rot = Quaternion.identity;
			this.nodes[0].tangent = new Vector3(0f, 1f, 0f);
			this.nodes[0].normal = new Vector3(0f, 0f, 1f);
			for (int i = 1; i < this.nodes.Length; i++)
			{
				Vector3 vector;
				if (i == this.nodes.Length - 1)
				{
					vector = this.nodes[i].point - this.nodes[i - 1].point;
				}
				else
				{
					float d = Vector3.Distance(this.nodes[i].point, this.nodes[i - 1].point);
					float d2 = Vector3.Distance(this.nodes[i].point, this.nodes[i + 1].point);
					vector = (this.nodes[i].point - this.nodes[i - 1].point) / d + (this.nodes[i + 1].point - this.nodes[i].point) / d2;
				}
				vector.Normalize();
				m.SetColumn(1, vector);
				if (Mathf.Abs(Vector3.Dot(vector, m.GetColumn(0))) > 0.9999f)
				{
					m.SetColumn(0, new Vector3(0f, 1f, 0f));
				}
				Vector3 normalized = Vector3.Cross(m.GetColumn(0), vector).normalized;
				m.SetColumn(2, normalized);
				m = MathUtils.OrthogonalizeMatrix(m);
				this.nodes[i].rot = MathUtils.QuaternionFromMatrix(m);
				this.nodes[i].normal = m.GetColumn(2);
				this.nodes[i].tangent = m.GetColumn(1);
				if (Quaternion.Dot(this.nodes[i].rot, this.nodes[i - 1].rot) < 0f)
				{
					this.nodes[i].rot.x = -this.nodes[i].rot.x;
					this.nodes[i].rot.y = -this.nodes[i].rot.y;
					this.nodes[i].rot.z = -this.nodes[i].rot.z;
					this.nodes[i].rot.w = -this.nodes[i].rot.w;
				}
			}
		}
		private Quaternion GetRotationInternal(int idxFirstpoint, float t)
		{
			float num = t * t;
			float num2 = num * t;
			Quaternion rot = this.nodes[Mathf.Max(idxFirstpoint - 1, 0)].rot;
			Quaternion rot2 = this.nodes[idxFirstpoint].rot;
			Quaternion rot3 = this.nodes[idxFirstpoint + 1].rot;
			Quaternion rot4 = this.nodes[Mathf.Min(idxFirstpoint + 2, this.nodes.Length - 1)].rot;
			Quaternion quaternion = new Quaternion(this.tension * (rot3.x - rot.x), this.tension * (rot3.y - rot.y), this.tension * (rot3.z - rot.z), this.tension * (rot3.w - rot.w));
			Quaternion quaternion2 = new Quaternion(this.tension * (rot4.x - rot2.x), this.tension * (rot4.y - rot2.y), this.tension * (rot4.z - rot2.z), this.tension * (rot4.w - rot2.w));
			float num3 = 2f * num2 - 3f * num + 1f;
			float num4 = -2f * num2 + 3f * num;
			float num5 = num2 - 2f * num + t;
			float num6 = num2 - num;
			Quaternion result = default(Quaternion);
			result.x = num3 * rot2.x + num4 * rot3.x + num5 * quaternion.x + num6 * quaternion2.x;
			result.y = num3 * rot2.y + num4 * rot3.y + num5 * quaternion.y + num6 * quaternion2.y;
			result.z = num3 * rot2.z + num4 * rot3.z + num5 * quaternion.z + num6 * quaternion2.z;
			result.w = num3 * rot2.w + num4 * rot3.w + num5 * quaternion.w + num6 * quaternion2.w;
			float num7 = Mathf.Sqrt(result.x * result.x + result.y * result.y + result.z * result.z + result.w * result.w);
			result.x /= num7;
			result.y /= num7;
			result.z /= num7;
			result.w /= num7;
			return result;
		}
		private Vector3 GetPositionInternal(int idxFirstpoint, float t)
		{
			float num = t * t;
			float num2 = num * t;
			Vector3 point = this.nodes[Mathf.Max(idxFirstpoint - 1, 0)].point;
			Vector3 point2 = this.nodes[idxFirstpoint].point;
			Vector3 point3 = this.nodes[idxFirstpoint + 1].point;
			Vector3 point4 = this.nodes[Mathf.Min(idxFirstpoint + 2, this.nodes.Length - 1)].point;
			Vector3 a = this.tension * (point3 - point);
			Vector3 a2 = this.tension * (point4 - point2);
			float d = 2f * num2 - 3f * num + 1f;
			float d2 = -2f * num2 + 3f * num;
			float d3 = num2 - 2f * num + t;
			float d4 = num2 - num;
			return d * point2 + d2 * point3 + d3 * a + d4 * a2;
		}
		public Quaternion GetRotationAtTime(float timeParam)
		{
			if (this.nodes.Length < 2)
			{
				return Quaternion.identity;
			}
			if (timeParam <= this.nodes[0].time)
			{
				return this.nodes[0].rot;
			}
			if (timeParam >= this.nodes[this.nodes.Length - 1].time)
			{
				return this.nodes[this.nodes.Length - 1].rot;
			}
			int i;
			for (i = 1; i < this.nodes.Length; i++)
			{
				if (this.nodes[i].time > timeParam)
				{
					break;
				}
			}
			int num = i - 1;
			float t = (timeParam - this.nodes[num].time) / (this.nodes[num + 1].time - this.nodes[num].time);
			return this.GetRotationInternal(num, t);
		}
		public Vector3 GetPositionAtTime(float timeParam)
		{
			if (this.nodes.Length < 2)
			{
				return Vector3.zero;
			}
			if (timeParam <= this.nodes[0].time)
			{
				return this.nodes[0].point;
			}
			if (timeParam >= this.nodes[this.nodes.Length - 1].time)
			{
				return this.nodes[this.nodes.Length - 1].point;
			}
			int i;
			for (i = 1; i < this.nodes.Length; i++)
			{
				if (this.nodes[i].time > timeParam)
				{
					break;
				}
			}
			int num = i - 1;
			float t = (timeParam - this.nodes[num].time) / (this.nodes[num + 1].time - this.nodes[num].time);
			return this.GetPositionInternal(num, t);
		}
	}
}
