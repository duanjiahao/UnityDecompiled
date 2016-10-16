using System;

namespace UnityEditor
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public sealed class MenuItem : Attribute
	{
		public string menuItem;

		public bool validate;

		public int priority;

		public MenuItem(string itemName) : this(itemName, false)
		{
		}

		public MenuItem(string itemName, bool isValidateFunction) : this(itemName, isValidateFunction, (!itemName.StartsWith("GameObject/Create Other")) ? 1000 : 10)
		{
		}

		public MenuItem(string itemName, bool isValidateFunction, int priority) : this(itemName, isValidateFunction, priority, false)
		{
		}

		internal MenuItem(string itemName, bool isValidateFunction, int priority, bool internalMenu)
		{
			if (internalMenu)
			{
				this.menuItem = "internal:" + itemName;
			}
			else
			{
				this.menuItem = itemName;
			}
			this.validate = isValidateFunction;
			this.priority = priority;
		}
	}
}
