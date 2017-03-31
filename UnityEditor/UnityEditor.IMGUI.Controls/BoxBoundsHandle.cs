using System;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	public class BoxBoundsHandle : PrimitiveBoundsHandle
	{
		public Vector3 size
		{
			get
			{
				return base.GetSize();
			}
			set
			{
				base.SetSize(value);
			}
		}

		public BoxBoundsHandle(int controlIDHint) : base(controlIDHint)
		{
		}

		protected override void DrawWireframe()
		{
			Handles.DrawWireCube(base.center, this.size);
		}
	}
}
