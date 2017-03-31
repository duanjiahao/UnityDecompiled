using System;

namespace UnityEditor
{
	internal interface ParticleEffectUIOwner
	{
		Editor customEditor
		{
			get;
		}

		void Repaint();
	}
}
