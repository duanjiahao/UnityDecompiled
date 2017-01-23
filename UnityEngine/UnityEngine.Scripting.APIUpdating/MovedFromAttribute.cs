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

		public MovedFromAttribute(string sourceNamespace)
		{
			this.Namespace = sourceNamespace;
		}
	}
}
