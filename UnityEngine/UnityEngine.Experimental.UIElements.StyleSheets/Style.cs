using System;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	public struct Style<T>
	{
		internal int specificity;

		public T value;

		private static readonly Style<T> defaultStyle = default(Style<T>);

		public static Style<T> nil
		{
			get
			{
				return Style<T>.defaultStyle;
			}
		}

		public Style(T value)
		{
			this.value = value;
			this.specificity = 0;
		}

		internal Style(T value, int specifity)
		{
			this.value = value;
			this.specificity = specifity;
		}

		public T GetSpecifiedValueOrDefault(T defaultValue)
		{
			if (this.specificity > 0)
			{
				defaultValue = this.value;
			}
			return defaultValue;
		}

		public static implicit operator T(Style<T> sp)
		{
			return sp.value;
		}

		internal void Apply(Style<T> other, StylePropertyApplyMode mode)
		{
			if (mode != StylePropertyApplyMode.Copy)
			{
				if (mode != StylePropertyApplyMode.CopyIfMoreSpecific)
				{
					if (mode == StylePropertyApplyMode.CopyIfNotInline)
					{
						if (this.specificity < 2147483647)
						{
							this.value = other.value;
							this.specificity = other.specificity;
						}
					}
				}
				else if (other.specificity >= this.specificity)
				{
					this.value = other.value;
					this.specificity = other.specificity;
				}
			}
			else
			{
				this.value = other.value;
				this.specificity = other.specificity;
			}
		}

		public static implicit operator Style<T>(T value)
		{
			return new Style<T>(value);
		}

		public static Style<T> Create(T value)
		{
			return new Style<T>(value, 2147483647);
		}

		public override string ToString()
		{
			return string.Format("[StyleProperty<{2}>: specifity={0}, value={1}]", this.specificity, this.value, typeof(T).Name);
		}
	}
}
