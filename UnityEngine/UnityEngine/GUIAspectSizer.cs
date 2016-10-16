using System;

namespace UnityEngine
{
	internal sealed class GUIAspectSizer : GUILayoutEntry
	{
		private float aspect;

		public GUIAspectSizer(float aspect, GUILayoutOption[] options) : base(0f, 0f, 0f, 0f, GUIStyle.none)
		{
			this.aspect = aspect;
			this.ApplyOptions(options);
		}

		public override void CalcHeight()
		{
			this.minHeight = (this.maxHeight = this.rect.width / this.aspect);
		}
	}
}
