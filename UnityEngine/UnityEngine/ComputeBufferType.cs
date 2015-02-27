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
		DrawIndirect = 256
	}
}
