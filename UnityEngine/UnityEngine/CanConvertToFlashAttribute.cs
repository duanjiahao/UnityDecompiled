using System;
using System.Diagnostics;
namespace UnityEngine
{
	[Conditional("UNITY_FLASH")]
	internal class CanConvertToFlashAttribute : Attribute
	{
		public CanConvertToFlashAttribute()
		{
		}
		public CanConvertToFlashAttribute(params string[] members)
		{
		}
	}
}
