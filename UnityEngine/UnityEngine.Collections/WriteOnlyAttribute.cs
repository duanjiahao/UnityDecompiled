using System;
using UnityEngine.Scripting;

namespace UnityEngine.Collections
{
	[AttributeUsage(AttributeTargets.Field), RequiredByNativeCode]
	internal class WriteOnlyAttribute : Attribute
	{
	}
}
