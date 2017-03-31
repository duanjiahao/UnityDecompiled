using System;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	internal class VRCustomOptionsNone : VRCustomOptions
	{
		public override Rect Draw(Rect rect)
		{
			return rect;
		}

		public override float GetHeight()
		{
			return 0f;
		}
	}
}
