using System;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	internal class StylePropertyAttribute : Attribute
	{
		internal string propertyName;

		internal StylePropertyID propertyID;

		internal StylePropertyAttribute(string propertyName, StylePropertyID propertyID)
		{
			this.propertyName = propertyName;
			this.propertyID = propertyID;
		}

		public StylePropertyAttribute(string propertyName) : this(propertyName, StylePropertyID.Custom)
		{
		}

		public StylePropertyAttribute() : this(string.Empty)
		{
		}
	}
}
