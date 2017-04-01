using System;
using UnityEngine.Scripting;

namespace UnityEngine.Collections
{
	[AttributeUsage(AttributeTargets.Struct), RequiredByNativeCode]
	internal class NativeContainerSupportsAtomicWriteAttribute : Attribute
	{
	}
}
