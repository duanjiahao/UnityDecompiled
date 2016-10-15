using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ChangedCurve
	{
		public AnimationCurve curve;

		public int curveId;

		public EditorCurveBinding binding;

		public ChangedCurve(AnimationCurve curve, int curveId, EditorCurveBinding binding)
		{
			this.curve = curve;
			this.curveId = curveId;
			this.binding = binding;
		}

		public override int GetHashCode()
		{
			int hashCode = this.curve.GetHashCode();
			return 33 * hashCode + this.binding.GetHashCode();
		}
	}
}
