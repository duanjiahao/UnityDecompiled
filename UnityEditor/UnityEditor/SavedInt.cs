using System;

namespace UnityEditor
{
	internal class SavedInt
	{
		private int m_Value;

		private string m_Name;

		private bool m_Loaded;

		public int value
		{
			get
			{
				this.Load();
				return this.m_Value;
			}
			set
			{
				this.Load();
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
			this.m_Loaded = false;
		}

		private void Load()
		{
			if (this.m_Loaded)
			{
				return;
			}
			this.m_Loaded = true;
			this.m_Value = EditorPrefs.GetInt(this.m_Name, this.value);
		}

		public static implicit operator int(SavedInt s)
		{
			return s.value;
		}
	}
}
