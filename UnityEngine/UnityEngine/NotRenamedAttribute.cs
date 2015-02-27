using System;
namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface), NotConverted]
	public sealed class NotRenamedAttribute : Attribute
	{
	}
}
