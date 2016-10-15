using System;

namespace UnityEditorInternal
{
	[Flags]
	public enum InstrumentedAssemblyTypes
	{
		None = 0,
		System = 1,
		Unity = 2,
		Plugins = 4,
		Script = 8,
		All = 2147483647
	}
}
