using System;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	public class CapsuleBoundsHandle : PrimitiveBoundsHandle
	{
		public enum HeightAxis
		{
			X,
			Y,
			Z
		}

		private const int k_DirectionX = 0;

		private const int k_DirectionY = 1;

		private const int k_DirectionZ = 2;

		private static readonly Vector3[] s_HeightAxes = new Vector3[]
		{
			Vector3.right,
			Vector3.up,
			Vector3.forward
		};

		private static readonly int[] s_NextAxis = new int[]
		{
			1,
			2,
			0
		};

		private int m_HeightAxis = 1;

		public CapsuleBoundsHandle.HeightAxis heightAxis
		{
			get
			{
				return (CapsuleBoundsHandle.HeightAxis)this.m_HeightAxis;
			}
			set
			{
				if (this.m_HeightAxis != (int)value)
				{
					Vector3 size = Vector3.one * this.radius * 2f;
					size[(int)value] = base.GetSize()[this.m_HeightAxis];
					this.m_HeightAxis = (int)value;
					base.SetSize(size);
				}
			}
		}

		public float height
		{
			get
			{
				return base.IsAxisEnabled(this.m_HeightAxis) ? Mathf.Max(base.GetSize()[this.m_HeightAxis], 2f * this.radius) : 0f;
			}
			set
			{
				value = Mathf.Max(Mathf.Abs(value), 2f * this.radius);
				if (this.height != value)
				{
					Vector3 size = base.GetSize();
					size[this.m_HeightAxis] = value;
					base.SetSize(size);
				}
			}
		}

		public float radius
		{
			get
			{
				int index;
				float result;
				if (this.GetRadiusAxis(out index) || base.IsAxisEnabled(this.m_HeightAxis))
				{
					result = 0.5f * base.GetSize()[index];
				}
				else
				{
					result = 0f;
				}
				return result;
			}
			set
			{
				Vector3 size = base.GetSize();
				float num = 2f * value;
				for (int i = 0; i < 3; i++)
				{
					size[i] = ((i != this.m_HeightAxis) ? num : Mathf.Max(size[i], num));
				}
				base.SetSize(size);
			}
		}

		[Obsolete("Use parameterless constructor instead.")]
		public CapsuleBoundsHandle(int controlIDHint) : base(controlIDHint)
		{
		}

		public CapsuleBoundsHandle()
		{
		}

		protected override void DrawWireframe()
		{
			CapsuleBoundsHandle.HeightAxis vector3Axis = CapsuleBoundsHandle.HeightAxis.Y;
			CapsuleBoundsHandle.HeightAxis vector3Axis2 = CapsuleBoundsHandle.HeightAxis.Z;
			CapsuleBoundsHandle.HeightAxis heightAxis = this.heightAxis;
			if (heightAxis != CapsuleBoundsHandle.HeightAxis.Y)
			{
				if (heightAxis == CapsuleBoundsHandle.HeightAxis.Z)
				{
					vector3Axis = CapsuleBoundsHandle.HeightAxis.X;
					vector3Axis2 = CapsuleBoundsHandle.HeightAxis.Y;
				}
			}
			else
			{
				vector3Axis = CapsuleBoundsHandle.HeightAxis.Z;
				vector3Axis2 = CapsuleBoundsHandle.HeightAxis.X;
			}
			bool flag = base.IsAxisEnabled((int)this.heightAxis);
			bool flag2 = base.IsAxisEnabled((int)vector3Axis);
			bool flag3 = base.IsAxisEnabled((int)vector3Axis2);
			Vector3 vector = CapsuleBoundsHandle.s_HeightAxes[this.m_HeightAxis];
			Vector3 vector2 = CapsuleBoundsHandle.s_HeightAxes[CapsuleBoundsHandle.s_NextAxis[this.m_HeightAxis]];
			Vector3 vector3 = CapsuleBoundsHandle.s_HeightAxes[CapsuleBoundsHandle.s_NextAxis[CapsuleBoundsHandle.s_NextAxis[this.m_HeightAxis]]];
			float radius = this.radius;
			float height = this.height;
			Vector3 vector4 = base.center + vector * (height * 0.5f - radius);
			Vector3 vector5 = base.center - vector * (height * 0.5f - radius);
			if (flag)
			{
				if (flag3)
				{
					Handles.DrawWireArc(vector4, vector2, vector3, 180f, radius);
					Handles.DrawWireArc(vector5, vector2, vector3, -180f, radius);
					Handles.DrawLine(vector4 + vector3 * radius, vector5 + vector3 * radius);
					Handles.DrawLine(vector4 - vector3 * radius, vector5 - vector3 * radius);
				}
				if (flag2)
				{
					Handles.DrawWireArc(vector4, vector3, vector2, -180f, radius);
					Handles.DrawWireArc(vector5, vector3, vector2, 180f, radius);
					Handles.DrawLine(vector4 + vector2 * radius, vector5 + vector2 * radius);
					Handles.DrawLine(vector4 - vector2 * radius, vector5 - vector2 * radius);
				}
			}
			if (flag2 && flag3)
			{
				Handles.DrawWireArc(vector4, vector, vector2, 360f, radius);
				Handles.DrawWireArc(vector5, vector, vector2, -360f, radius);
			}
		}

		protected override Bounds OnHandleChanged(PrimitiveBoundsHandle.HandleDirection handle, Bounds boundsOnClick, Bounds newBounds)
		{
			int num = 0;
			switch (handle)
			{
			case PrimitiveBoundsHandle.HandleDirection.PositiveY:
			case PrimitiveBoundsHandle.HandleDirection.NegativeY:
				num = 1;
				break;
			case PrimitiveBoundsHandle.HandleDirection.PositiveZ:
			case PrimitiveBoundsHandle.HandleDirection.NegativeZ:
				num = 2;
				break;
			}
			Vector3 max = newBounds.max;
			Vector3 min = newBounds.min;
			if (num == this.m_HeightAxis)
			{
				int index;
				this.GetRadiusAxis(out index);
				float num2 = max[index] - min[index];
				float num3 = max[this.m_HeightAxis] - min[this.m_HeightAxis];
				if (num3 < num2)
				{
					if (handle == PrimitiveBoundsHandle.HandleDirection.PositiveX || handle == PrimitiveBoundsHandle.HandleDirection.PositiveY || handle == PrimitiveBoundsHandle.HandleDirection.PositiveZ)
					{
						max[this.m_HeightAxis] = min[this.m_HeightAxis] + num2;
					}
					else
					{
						min[this.m_HeightAxis] = max[this.m_HeightAxis] - num2;
					}
				}
			}
			else
			{
				max[this.m_HeightAxis] = boundsOnClick.center[this.m_HeightAxis] + 0.5f * boundsOnClick.size[this.m_HeightAxis];
				min[this.m_HeightAxis] = boundsOnClick.center[this.m_HeightAxis] - 0.5f * boundsOnClick.size[this.m_HeightAxis];
				float num4 = 0.5f * (max[num] - min[num]);
				float a = 0.5f * (max[this.m_HeightAxis] - min[this.m_HeightAxis]);
				for (int i = 0; i < 3; i++)
				{
					if (i != num)
					{
						float num5 = (i != this.m_HeightAxis) ? num4 : Mathf.Max(a, num4);
						min[i] = base.center[i] - num5;
						max[i] = base.center[i] + num5;
					}
				}
			}
			return new Bounds((max + min) * 0.5f, max - min);
		}

		private bool GetRadiusAxis(out int radiusAxis)
		{
			radiusAxis = CapsuleBoundsHandle.s_NextAxis[this.m_HeightAxis];
			bool result;
			if (!base.IsAxisEnabled(radiusAxis))
			{
				radiusAxis = CapsuleBoundsHandle.s_NextAxis[radiusAxis];
				result = false;
			}
			else
			{
				result = base.IsAxisEnabled(CapsuleBoundsHandle.s_NextAxis[radiusAxis]);
			}
			return result;
		}
	}
}
