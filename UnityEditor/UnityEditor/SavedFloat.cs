using System;
namespace UnityEditor
{
	internal class SavedFloat
	{
		private float m_Value;
		private string m_Name;
		public float value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				if (this.m_Value == value)
				{
					return;
				}
				this.m_Value = value;
				EditorPrefs.SetFloat(this.m_Name, value);
			}
		}
		public SavedFloat(string name, float value)
		{
			this.m_Name = name;
			this.m_Value = EditorPrefs.GetFloat(name, value);
		}
		public static implicit operator float(SavedFloat s)
		{
			return s.value;
		}
	}
}
