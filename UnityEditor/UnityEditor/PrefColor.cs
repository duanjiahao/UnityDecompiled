using System;
using System.Globalization;
using UnityEngine;

namespace UnityEditor
{
	internal class PrefColor : IPrefType
	{
		private string m_name;

		private Color m_color;

		private Color m_DefaultColor;

		private bool m_Loaded;

		public Color Color
		{
			get
			{
				this.Load();
				return this.m_color;
			}
			set
			{
				this.Load();
				this.m_color = value;
			}
		}

		public string Name
		{
			get
			{
				this.Load();
				return this.m_name;
			}
		}

		public PrefColor()
		{
			this.m_Loaded = true;
		}

		public PrefColor(string name, float defaultRed, float defaultGreen, float defaultBlue, float defaultAlpha)
		{
			this.m_name = name;
			this.m_color = (this.m_DefaultColor = new Color(defaultRed, defaultGreen, defaultBlue, defaultAlpha));
			Settings.Add(this);
			this.m_Loaded = false;
		}

		public void Load()
		{
			if (this.m_Loaded)
			{
				return;
			}
			this.m_Loaded = true;
			PrefColor prefColor = Settings.Get<PrefColor>(this.m_name, this);
			this.m_name = prefColor.Name;
			this.m_color = prefColor.Color;
		}

		public string ToUniqueString()
		{
			this.Load();
			return string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3};{4}", new object[]
			{
				this.m_name,
				this.Color.r,
				this.Color.g,
				this.Color.b,
				this.Color.a
			});
		}

		public void FromUniqueString(string s)
		{
			this.Load();
			string[] array = s.Split(new char[]
			{
				';'
			});
			if (array.Length != 5)
			{
				Debug.LogError("Parsing PrefColor failed");
				return;
			}
			this.m_name = array[0];
			array[1] = array[1].Replace(',', '.');
			array[2] = array[2].Replace(',', '.');
			array[3] = array[3].Replace(',', '.');
			array[4] = array[4].Replace(',', '.');
			float r;
			bool flag = float.TryParse(array[1], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out r);
			float g;
			flag &= float.TryParse(array[2], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out g);
			float b;
			flag &= float.TryParse(array[3], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out b);
			float a;
			flag &= float.TryParse(array[4], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out a);
			if (flag)
			{
				this.m_color = new Color(r, g, b, a);
			}
			else
			{
				Debug.LogError("Parsing PrefColor failed");
			}
		}

		internal void ResetToDefault()
		{
			this.Load();
			this.m_color = this.m_DefaultColor;
		}

		public static implicit operator Color(PrefColor pcolor)
		{
			return pcolor.Color;
		}
	}
}
