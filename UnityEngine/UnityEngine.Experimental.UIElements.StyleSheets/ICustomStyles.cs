using System;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	public interface ICustomStyles
	{
		void ApplyCustomProperty(string propertyName, ref Style<float> target);

		void ApplyCustomProperty(string propertyName, ref Style<int> target);

		void ApplyCustomProperty(string propertyName, ref Style<bool> target);

		void ApplyCustomProperty(string propertyName, ref Style<Color> target);

		void ApplyCustomProperty<T>(string propertyName, ref Style<T> target) where T : UnityEngine.Object;

		void ApplyCustomProperty(string propertyName, ref Style<string> target);
	}
}
