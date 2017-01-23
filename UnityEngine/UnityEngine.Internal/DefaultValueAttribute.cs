using System;

namespace UnityEngine.Internal
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
	[Serializable]
	public class DefaultValueAttribute : Attribute
	{
		private object DefaultValue;

		public object Value
		{
			get
			{
				return this.DefaultValue;
			}
		}

		public DefaultValueAttribute(string value)
		{
			this.DefaultValue = value;
		}

		public override bool Equals(object obj)
		{
			DefaultValueAttribute defaultValueAttribute = obj as DefaultValueAttribute;
			bool result;
			if (defaultValueAttribute == null)
			{
				result = false;
			}
			else if (this.DefaultValue == null)
			{
				result = (defaultValueAttribute.Value == null);
			}
			else
			{
				result = this.DefaultValue.Equals(defaultValueAttribute.Value);
			}
			return result;
		}

		public override int GetHashCode()
		{
			int hashCode;
			if (this.DefaultValue == null)
			{
				hashCode = base.GetHashCode();
			}
			else
			{
				hashCode = this.DefaultValue.GetHashCode();
			}
			return hashCode;
		}
	}
}
