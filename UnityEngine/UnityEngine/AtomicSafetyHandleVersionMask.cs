using System;

namespace UnityEngine
{
	[Flags]
	internal enum AtomicSafetyHandleVersionMask
	{
		Read = 1,
		Write = 2,
		ReadAndWrite = 3,
		WriteInv = -3,
		ReadInv = -2,
		ReadAndWriteInv = -4
	}
}
