using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class DisallowMultipleComponent : Attribute
	{
	}
}
