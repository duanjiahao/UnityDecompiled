using System;
using Unity.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Collections
{
	[NativeEnum(Name = "NativeCollection::Allocator"), UsedByNativeCode]
	public enum Allocator
	{
		Invalid,
		None,
		Temp,
		TempJob,
		Persistent
	}
}
