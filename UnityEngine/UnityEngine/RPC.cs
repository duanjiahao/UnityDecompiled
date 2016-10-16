using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true), Obsolete("NetworkView RPC functions are deprecated. Refer to the new Multiplayer Networking system."), RequiredByNativeCode]
	public sealed class RPC : Attribute
	{
	}
}
