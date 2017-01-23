using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public sealed class ContextMenu : Attribute
	{
		public readonly string menuItem;

		public readonly bool validate;

		public readonly int priority;

		public ContextMenu(string itemName) : this(itemName, false)
		{
		}

		public ContextMenu(string itemName, bool isValidateFunction) : this(itemName, isValidateFunction, 1000000)
		{
		}

		public ContextMenu(string itemName, bool isValidateFunction, int priority)
		{
			this.menuItem = itemName;
			this.validate = isValidateFunction;
			this.priority = priority;
		}
	}
}
