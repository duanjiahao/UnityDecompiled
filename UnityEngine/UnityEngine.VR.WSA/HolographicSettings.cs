using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.VR.WSA
{
	public sealed class HolographicSettings
	{
		public static extern bool IsLatentFramePresentation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static void SetFocusPointForFrame(Vector3 position)
		{
			HolographicSettings.InternalSetFocusPointForFrame(position);
		}

		private static void InternalSetFocusPointForFrame(Vector3 position)
		{
			HolographicSettings.INTERNAL_CALL_InternalSetFocusPointForFrame(ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalSetFocusPointForFrame(ref Vector3 position);

		public static void SetFocusPointForFrame(Vector3 position, Vector3 normal)
		{
			HolographicSettings.InternalSetFocusPointForFrameWithNormal(position, normal);
		}

		private static void InternalSetFocusPointForFrameWithNormal(Vector3 position, Vector3 normal)
		{
			HolographicSettings.INTERNAL_CALL_InternalSetFocusPointForFrameWithNormal(ref position, ref normal);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalSetFocusPointForFrameWithNormal(ref Vector3 position, ref Vector3 normal);

		public static void SetFocusPointForFrame(Vector3 position, Vector3 normal, Vector3 velocity)
		{
			HolographicSettings.InternalSetFocusPointForFrameWithNormalVelocity(position, normal, velocity);
		}

		private static void InternalSetFocusPointForFrameWithNormalVelocity(Vector3 position, Vector3 normal, Vector3 velocity)
		{
			HolographicSettings.INTERNAL_CALL_InternalSetFocusPointForFrameWithNormalVelocity(ref position, ref normal, ref velocity);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalSetFocusPointForFrameWithNormalVelocity(ref Vector3 position, ref Vector3 normal, ref Vector3 velocity);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ActivateLatentFramePresentation(bool activated);
	}
}
