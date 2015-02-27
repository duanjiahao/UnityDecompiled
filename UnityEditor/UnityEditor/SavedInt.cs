using System;
namespace UnityEditor
{
	internal class SavedInt
	{
		private int m_Value;
		private string m_Name;
		public int value
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
				EditorPrefs.SetInt(this.m_Name, value);
			}
		}
		public SavedInt(string name, int value)
		{
			this.m_Name = name;
			this.m_Value = EditorPrefs.GetInt(name, value);
		}
		public static implicit operator int(SavedInt s)
		{
			return s.value;
		}
	}
}
