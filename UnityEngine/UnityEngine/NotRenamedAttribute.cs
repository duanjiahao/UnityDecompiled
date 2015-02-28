using System;
namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface), Obsolete("NotRenamedAttribute was used for the Flash buildtarget, which is not supported anymore starting Unity 5.0", true), NotConverted]
	public sealed class NotRenamedAttribute : Attribute
	{
	}
}
