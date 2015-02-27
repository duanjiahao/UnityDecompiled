using System;
using System.Diagnostics;
namespace UnityEngine.Flash
{
	[NotConverted]
	public sealed class ActionScript
	{
		[Conditional("UNITY_FLASH")]
		public static void Import(string package)
		{
		}
		[Conditional("UNITY_FLASH")]
		public static void Statement(string formatString, params object[] arguments)
		{
		}
		public static T Expression<T>(string formatString, params object[] arguments)
		{
			throw new InvalidOperationException();
		}
	}
}
