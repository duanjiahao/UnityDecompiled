using System;

namespace UnityEditor
{
	internal class SerializedMinMaxColor
	{
		public SerializedProperty maxColor;

		public SerializedProperty minColor;

		public SerializedProperty minMax;

		public SerializedMinMaxColor(SerializedModule m)
		{
			this.Init(m, "curve");
		}

		public SerializedMinMaxColor(SerializedModule m, string name)
		{
			this.Init(m, name);
		}

		private void Init(SerializedModule m, string name)
		{
			this.maxColor = m.GetProperty(name, "maxColor");
			this.minColor = m.GetProperty(name, "minColor");
			this.minMax = m.GetProperty(name, "minMax");
		}
	}
}
