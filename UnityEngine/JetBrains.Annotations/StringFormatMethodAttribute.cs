using System;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class StringFormatMethodAttribute : Attribute
	{
		public string FormatParameterName
		{
			get;
			private set;
		}

		public StringFormatMethodAttribute(string formatParameterName)
		{
			this.FormatParameterName = formatParameterName;
		}
	}
}
