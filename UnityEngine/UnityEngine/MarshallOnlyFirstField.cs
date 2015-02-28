using System;
using System.Diagnostics;
namespace UnityEngine
{
	[Conditional("UNITY_WINRT")]
	internal class MarshallOnlyFirstField : Attribute
	{
	}
}
