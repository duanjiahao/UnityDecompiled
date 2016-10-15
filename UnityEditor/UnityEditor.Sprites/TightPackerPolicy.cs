using System;

namespace UnityEditor.Sprites
{
	internal class TightPackerPolicy : DefaultPackerPolicy
	{
		protected override string TagPrefix
		{
			get
			{
				return "[RECT]";
			}
		}

		protected override bool AllowTightWhenTagged
		{
			get
			{
				return false;
			}
		}

		protected override bool AllowRotationFlipping
		{
			get
			{
				return false;
			}
		}
	}
}
