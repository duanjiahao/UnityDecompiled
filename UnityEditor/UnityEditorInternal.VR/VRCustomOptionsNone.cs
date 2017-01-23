using System;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	internal class VRCustomOptionsNone : VRCustomOptions
	{
		public override void Draw(Rect rect)
		{
		}

		public override float GetHeight()
		{
			return 0f;
		}
	}
}
