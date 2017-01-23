using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class GUILayer : Behaviour
	{
		public GUIElement HitTest(Vector3 screenPosition)
		{
			return GUILayer.INTERNAL_CALL_HitTest(this, ref screenPosition);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GUIElement INTERNAL_CALL_HitTest(GUILayer self, ref Vector3 screenPosition);
	}
}
