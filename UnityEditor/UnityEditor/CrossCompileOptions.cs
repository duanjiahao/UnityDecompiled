using System;

namespace UnityEditor
{
	[Flags]
	internal enum CrossCompileOptions
	{
		Dynamic = 0,
		FastICall = 1,
		Static = 2,
		Debugging = 4,
		ExplicitNullChecks = 8,
		LoadSymbols = 16
	}
}
