using System;

namespace UnityEngine.Scripting.APIUpdating
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate)]
	public class MovedFromAttribute : Attribute
	{
		public string Namespace
		{
			get;
			private set;
		}

		public bool IsInDifferentAssembly
		{
			get;
			private set;
		}

		public MovedFromAttribute(string sourceNamespace) : this(sourceNamespace, false)
		{
		}

		public MovedFromAttribute(string sourceNamespace, bool isInDifferentAssembly)
		{
			this.Namespace = sourceNamespace;
			this.IsInDifferentAssembly = isInDifferentAssembly;
		}
	}
}
