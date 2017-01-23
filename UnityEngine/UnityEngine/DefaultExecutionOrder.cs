using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class), UsedByNativeCode]
	public class DefaultExecutionOrder : Attribute
	{
		public int order
		{
			get;
			private set;
		}

		public DefaultExecutionOrder(int order)
		{
			this.order = order;
		}
	}
}
