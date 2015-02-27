using System;
using UnityEngine;
namespace UnityEditor
{
	internal class KeyIdentifier
	{
		public CurveRenderer renderer;
		public int curveId;
		public int key;
		public AnimationCurve curve
		{
			get
			{
				return this.renderer.GetCurve();
			}
		}
		public Keyframe keyframe
		{
			get
			{
				return this.curve[this.key];
			}
		}
		public KeyIdentifier(CurveRenderer _renderer, int _curveId, int _keyIndex)
		{
			this.renderer = _renderer;
			this.curveId = _curveId;
			this.key = _keyIndex;
		}
	}
}
