using System;
using System.Text;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct PropertyName
	{
		internal int id;

		internal int conflictIndex;

		public PropertyName(string name)
		{
			this = new PropertyName(PropertyNameUtils.PropertyNameFromString(name));
		}

		public PropertyName(PropertyName other)
		{
			this.id = other.id;
			this.conflictIndex = other.conflictIndex;
		}

		public PropertyName(int id)
		{
			this.id = id;
			this.conflictIndex = 0;
		}

		public static bool IsNullOrEmpty(PropertyName prop)
		{
			return prop.id == 0;
		}

		public static bool operator ==(PropertyName lhs, PropertyName rhs)
		{
			return lhs.id == rhs.id;
		}

		public static bool operator !=(PropertyName lhs, PropertyName rhs)
		{
			return lhs.id != rhs.id;
		}

		public override int GetHashCode()
		{
			return this.id;
		}

		public override bool Equals(object other)
		{
			return other is PropertyName && this == (PropertyName)other;
		}

		public static implicit operator PropertyName(string name)
		{
			return new PropertyName(name);
		}

		public static implicit operator PropertyName(int id)
		{
			return new PropertyName(id);
		}

		public override string ToString()
		{
			int num = PropertyNameUtils.ConflictCountForID(this.id);
			string text = string.Format("{0}:{1}", PropertyNameUtils.StringFromPropertyName(this), this.id);
			if (num > 0)
			{
				StringBuilder stringBuilder = new StringBuilder(text);
				stringBuilder.Append(" conflicts with ");
				for (int i = 0; i < num; i++)
				{
					if (i != this.conflictIndex)
					{
						StringBuilder arg_84_0 = stringBuilder;
						string arg_84_1 = "\"{0}\"";
						PropertyName propertyName = new PropertyName(this.id);
						propertyName.conflictIndex = i;
						arg_84_0.AppendFormat(arg_84_1, PropertyNameUtils.StringFromPropertyName(propertyName));
					}
				}
				text = stringBuilder.ToString();
			}
			return text;
		}
	}
}
