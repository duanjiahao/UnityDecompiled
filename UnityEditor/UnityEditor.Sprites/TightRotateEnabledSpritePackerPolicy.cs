using System;

namespace UnityEditor.Sprites
{
	internal class TightRotateEnabledSpritePackerPolicy : DefaultPackerPolicy
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
				return true;
			}
		}
	}
}
