using System;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	public static class HolographicEmulationHelpers
	{
		public const float k_DefaultBodyHeight = 1.776f;

		public const float k_DefaultHeadDiameter = 0.2319999f;

		public const float k_ForwardOffset = 0.0985f;

		public static Vector3 CalcExpectedCameraPosition(SimulatedHead head, SimulatedBody body)
		{
			Vector3 vector = body.position;
			vector.y += body.height - 1.776f;
			vector.y -= head.diameter / 2f;
			vector.y += 0.115999952f;
			Vector3 eulerAngles = head.eulerAngles;
			eulerAngles.y += body.rotation;
			Quaternion rotation = Quaternion.Euler(eulerAngles);
			vector += rotation * (0.0985f * Vector3.forward);
			return vector;
		}
	}
}
