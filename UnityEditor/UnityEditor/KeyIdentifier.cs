using System;
using UnityEngine;

namespace UnityEditor
{
	internal class KeyIdentifier
	{
		public AnimationCurve curve;

		public int curveId;

		public int key;

		public EditorCurveBinding binding;

		public Keyframe keyframe
		{
			get
			{
				return this.curve[this.key];
			}
		}

		public KeyIdentifier(AnimationCurve _curve, int _curveId, int _keyIndex)
		{
			this.curve = _curve;
			this.curveId = _curveId;
			this.key = _keyIndex;
		}

		public KeyIdentifier(AnimationCurve _curve, int _curveId, int _keyIndex, EditorCurveBinding _binding)
		{
			this.curve = _curve;
			this.curveId = _curveId;
			this.key = _keyIndex;
			this.binding = _binding;
		}
	}
}
