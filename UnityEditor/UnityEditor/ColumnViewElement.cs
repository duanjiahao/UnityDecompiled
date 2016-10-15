using System;

namespace UnityEditor
{
	internal class ColumnViewElement
	{
		public string name;

		public object value;

		public ColumnViewElement(string name, object value)
		{
			this.name = name;
			this.value = value;
		}
	}
}
