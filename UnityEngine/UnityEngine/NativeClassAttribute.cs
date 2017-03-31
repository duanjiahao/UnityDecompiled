using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public sealed class NativeClassAttribute : Attribute
	{
		public string QualifiedNativeName
		{
			get;
			private set;
		}

		public NativeClassAttribute(string qualifiedCppName)
		{
			this.QualifiedNativeName = qualifiedCppName;
		}
	}
}
