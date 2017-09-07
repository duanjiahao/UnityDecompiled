using System;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	public class SphereBoundsHandle : PrimitiveBoundsHandle
	{
		public float radius
		{
			get
			{
				Vector3 size = base.GetSize();
				float num = 0f;
				for (int i = 0; i < 3; i++)
				{
					if (base.IsAxisEnabled(i))
					{
						num = Mathf.Max(num, Mathf.Abs(size[i]));
					}
				}
				return num * 0.5f;
			}
			set
			{
				base.SetSize(2f * value * Vector3.one);
			}
		}

		[Obsolete("Use parameterless constructor instead.")]
		public SphereBoundsHandle(int controlIDHint) : base(controlIDHint)
		{
		}

		public SphereBoundsHandle()
		{
		}

		protected override void DrawWireframe()
		{
			bool flag = base.IsAxisEnabled(PrimitiveBoundsHandle.Axes.X);
			bool flag2 = base.IsAxisEnabled(PrimitiveBoundsHandle.Axes.Y);
			bool flag3 = base.IsAxisEnabled(PrimitiveBoundsHandle.Axes.Z);
			if (flag && flag2)
			{
				Handles.DrawWireArc(base.center, Vector3.forward, Vector3.up, 360f, this.radius);
			}
			if (flag && flag3)
			{
				Handles.DrawWireArc(base.center, Vector3.up, Vector3.right, 360f, this.radius);
			}
			if (flag2 && flag3)
			{
				Handles.DrawWireArc(base.center, Vector3.right, Vector3.forward, 360f, this.radius);
			}
			if (flag && !flag2 && !flag3)
			{
				Handles.DrawLine(Vector3.right * this.radius, Vector3.left * this.radius);
			}
			if (!flag && flag2 && !flag3)
			{
				Handles.DrawLine(Vector3.up * this.radius, Vector3.down * this.radius);
			}
			if (!flag && !flag2 && flag3)
			{
				Handles.DrawLine(Vector3.forward * this.radius, Vector3.back * this.radius);
			}
		}

		protected override Bounds OnHandleChanged(PrimitiveBoundsHandle.HandleDirection handle, Bounds boundsOnClick, Bounds newBounds)
		{
			Vector3 max = newBounds.max;
			Vector3 min = newBounds.min;
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
			float num2 = 0.5f * (max[num] - min[num]);
			for (int i = 0; i < 3; i++)
			{
				if (i != num)
				{
					min[i] = base.center[i] - num2;
					max[i] = base.center[i] + num2;
				}
			}
			return new Bounds((max + min) * 0.5f, max - min);
		}
	}
}
