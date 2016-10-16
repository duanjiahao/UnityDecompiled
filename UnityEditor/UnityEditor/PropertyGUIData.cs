using System;
using UnityEngine;

namespace UnityEditor
{
	internal struct PropertyGUIData
	{
		public SerializedProperty property;

		public Rect totalPosition;

		public bool wasBoldDefaultFont;

		public bool wasEnabled;

		public Color color;

		public PropertyGUIData(SerializedProperty property, Rect totalPosition, bool wasBoldDefaultFont, bool wasEnabled, Color color)
		{
			this.property = property;
			this.totalPosition = totalPosition;
			this.wasBoldDefaultFont = wasBoldDefaultFont;
			this.wasEnabled = wasEnabled;
			this.color = color;
		}
	}
}
