using System;

namespace UnityEditor
{
	public struct SupportedRenderingFeatures
	{
		[Flags]
		public enum ReflectionProbe
		{
			None = 0,
			Rotation = 1
		}

		public SupportedRenderingFeatures.ReflectionProbe reflectionProbe;

		private static SupportedRenderingFeatures s_Active = default(SupportedRenderingFeatures);

		public static SupportedRenderingFeatures active
		{
			get
			{
				return SupportedRenderingFeatures.s_Active;
			}
			set
			{
				SupportedRenderingFeatures.s_Active = value;
			}
		}

		public static SupportedRenderingFeatures Default
		{
			get
			{
				return default(SupportedRenderingFeatures);
			}
		}
	}
}
