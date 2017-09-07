using System;

namespace UnityEngine
{
	[Flags]
	public enum ComputeBufferType
	{
		Default = 0,
		Raw = 1,
		Append = 2,
		Counter = 4,
		[Obsolete("Enum member DrawIndirect has been deprecated. Use IndirectArguments instead (UnityUpgradable) -> IndirectArguments", false)]
		DrawIndirect = 256,
		IndirectArguments = 256,
		[Obsolete("Enum member GPUMemory has been deprecated. All compute buffers now follow the behavior previously defined by this member.", false)]
		GPUMemory = 512
	}
}
