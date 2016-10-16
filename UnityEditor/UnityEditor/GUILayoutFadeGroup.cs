using System;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class GUILayoutFadeGroup : GUILayoutGroup
	{
		public float fadeValue;

		public bool wasGUIEnabled;

		public Color guiColor;

		public override void CalcHeight()
		{
			base.CalcHeight();
			this.minHeight *= this.fadeValue;
			this.maxHeight *= this.fadeValue;
		}
	}
}
