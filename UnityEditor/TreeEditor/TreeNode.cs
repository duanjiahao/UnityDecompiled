using System;
using UnityEngine;
namespace TreeEditor
{
	[Serializable]
	public class TreeNode
	{
		public TreeSpline spline;
		public int seed;
		public float animSeed;
		public bool visible;
		public int triStart;
		public int triEnd;
		public int vertStart;
		public int vertEnd;
		public float capRange;
		public float breakOffset;
		public float size;
		public float scale;
		public float offset;
		public float baseAngle;
		public float angle;
		public float pitch;
		public Quaternion rotation;
		public Matrix4x4 matrix;
		public int parentID;
		public int groupID;
		[NonSerialized]
		internal TreeNode parent;
		[NonSerialized]
		internal TreeGroup group;
		[SerializeField]
		private int _uniqueID = -1;
		public int uniqueID
		{
			get
			{
				return this._uniqueID;
			}
			set
			{
				if (this._uniqueID == -1)
				{
					this._uniqueID = value;
				}
			}
		}
		public TreeNode()
		{
			this.spline = null;
			this.parentID = 0;
			this.groupID = 0;
			this.parent = null;
			this.group = null;
			this.seed = 1234;
			this.breakOffset = 1f;
			this.visible = true;
			this.animSeed = 0f;
			this.scale = 1f;
			this.rotation = Quaternion.identity;
			this.matrix = Matrix4x4.identity;
		}
		public float GetScale()
		{
			float num = 1f;
			if (this.parent != null)
			{
				num = this.parent.GetScale();
			}
			return this.scale * num;
		}
		public float GetSurfaceAngleAtTime(float time)
		{
			if (this.spline == null)
			{
				return 0f;
			}
			Vector3 positionAtTime = this.spline.GetPositionAtTime(time);
			float radiusAtTime = this.group.GetRadiusAtTime(this, time, false);
			float num;
			if (time < 0.5f)
			{
				float magnitude = (this.spline.GetPositionAtTime(time + 0.01f) - positionAtTime).magnitude;
				float y = this.group.GetRadiusAtTime(this, time + 0.01f, false) - radiusAtTime;
				num = Mathf.Atan2(y, magnitude);
			}
			else
			{
				float magnitude2 = (positionAtTime - this.spline.GetPositionAtTime(time - 0.01f)).magnitude;
				float y2 = radiusAtTime - this.group.GetRadiusAtTime(this, time - 0.01f, false);
				num = Mathf.Atan2(y2, magnitude2);
			}
			return num * 57.29578f;
		}
		public float GetRadiusAtTime(float time)
		{
			return this.group.GetRadiusAtTime(this, time, false);
		}
		public void GetPropertiesAtTime(float time, out Vector3 pos, out Quaternion rot, out float rad)
		{
			if (this.spline == null)
			{
				pos = Vector3.zero;
				rot = Quaternion.identity;
			}
			else
			{
				pos = this.spline.GetPositionAtTime(time);
				rot = this.spline.GetRotationAtTime(time);
			}
			rad = this.group.GetRadiusAtTime(this, time, false);
		}
		public Matrix4x4 GetLocalMatrixAtTime(float time)
		{
			Vector3 zero = Vector3.zero;
			Quaternion identity = Quaternion.identity;
			float num = 0f;
			this.GetPropertiesAtTime(time, out zero, out identity, out num);
			return Matrix4x4.TRS(zero, identity, Vector3.one);
		}
	}
}
