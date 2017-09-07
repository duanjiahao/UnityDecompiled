using System;
using System.Globalization;
using UnityEngine;

namespace UnityEditor
{
	internal class PrefColor : IPrefType
	{
		private string m_Name;

		private Color m_Color;

		private Color m_DefaultColor;

		private bool m_SeparateColors;

		private Color m_OptionalDarkColor;

		private Color m_OptionalDarkDefaultColor;

		private bool m_Loaded;

		public Color Color
		{
			get
			{
				this.Load();
				Color result;
				if (this.m_SeparateColors && EditorGUIUtility.isProSkin)
				{
					result = this.m_OptionalDarkColor;
				}
				else
				{
					result = this.m_Color;
				}
				return result;
			}
			set
			{
				this.Load();
				if (this.m_SeparateColors && EditorGUIUtility.isProSkin)
				{
					this.m_OptionalDarkColor = value;
				}
				else
				{
					this.m_Color = value;
				}
			}
		}

		public string Name
		{
			get
			{
				this.Load();
				return this.m_Name;
			}
		}

		public PrefColor()
		{
			this.m_Loaded = true;
		}

		public PrefColor(string name, float defaultRed, float defaultGreen, float defaultBlue, float defaultAlpha)
		{
			this.m_Name = name;
			this.m_Color = (this.m_DefaultColor = new Color(defaultRed, defaultGreen, defaultBlue, defaultAlpha));
			this.m_SeparateColors = false;
			this.m_OptionalDarkColor = (this.m_OptionalDarkDefaultColor = Color.clear);
			Settings.Add(this);
			this.m_Loaded = false;
		}

		public PrefColor(string name, float defaultRed, float defaultGreen, float defaultBlue, float defaultAlpha, float defaultRed2, float defaultGreen2, float defaultBlue2, float defaultAlpha2)
		{
			this.m_Name = name;
			this.m_Color = (this.m_DefaultColor = new Color(defaultRed, defaultGreen, defaultBlue, defaultAlpha));
			this.m_SeparateColors = true;
			this.m_OptionalDarkColor = (this.m_OptionalDarkDefaultColor = new Color(defaultRed2, defaultGreen2, defaultBlue2, defaultAlpha2));
			Settings.Add(this);
			this.m_Loaded = false;
		}

		public void Load()
		{
			if (!this.m_Loaded)
			{
				this.m_Loaded = true;
				PrefColor prefColor = Settings.Get<PrefColor>(this.m_Name, this);
				this.m_Name = prefColor.m_Name;
				this.m_Color = prefColor.m_Color;
				this.m_SeparateColors = prefColor.m_SeparateColors;
				this.m_OptionalDarkColor = prefColor.m_OptionalDarkColor;
			}
		}

		public static implicit operator Color(PrefColor pcolor)
		{
			return pcolor.Color;
		}

		public string ToUniqueString()
		{
			this.Load();
			string result;
			if (this.m_SeparateColors)
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3};{4};{5};{6};{7};{8}", new object[]
				{
					this.m_Name,
					this.m_Color.r,
					this.m_Color.g,
					this.m_Color.b,
					this.m_Color.a,
					this.m_OptionalDarkColor.r,
					this.m_OptionalDarkColor.g,
					this.m_OptionalDarkColor.b,
					this.m_OptionalDarkColor.a
				});
			}
			else
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3};{4}", new object[]
				{
					this.m_Name,
					this.m_Color.r,
					this.m_Color.g,
					this.m_Color.b,
					this.m_Color.a
				});
			}
			return result;
		}

		public void FromUniqueString(string s)
		{
			this.Load();
			string[] array = s.Split(new char[]
			{
				';'
			});
			if (array.Length != 5 && array.Length != 9)
			{
				Debug.LogError("Parsing PrefColor failed");
			}
			else
			{
				this.m_Name = array[0];
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
					this.m_Color = new Color(r, g, b, a);
				}
				else
				{
					Debug.LogError("Parsing PrefColor failed");
				}
				if (array.Length == 9)
				{
					this.m_SeparateColors = true;
					array[5] = array[5].Replace(',', '.');
					array[6] = array[6].Replace(',', '.');
					array[7] = array[7].Replace(',', '.');
					array[8] = array[8].Replace(',', '.');
					flag = float.TryParse(array[5], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out r);
					flag &= float.TryParse(array[6], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out g);
					flag &= float.TryParse(array[7], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out b);
					flag &= float.TryParse(array[8], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out a);
					if (flag)
					{
						this.m_OptionalDarkColor = new Color(r, g, b, a);
					}
					else
					{
						Debug.LogError("Parsing PrefColor failed");
					}
				}
				else
				{
					this.m_SeparateColors = false;
					this.m_OptionalDarkColor = Color.clear;
				}
			}
		}

		internal void ResetToDefault()
		{
			this.Load();
			this.m_Color = this.m_DefaultColor;
			this.m_OptionalDarkColor = this.m_OptionalDarkDefaultColor;
		}
	}
}
