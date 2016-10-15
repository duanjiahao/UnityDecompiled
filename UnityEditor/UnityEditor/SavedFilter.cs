using System;

namespace UnityEditor
{
	[Serializable]
	internal class SavedFilter
	{
		public string m_Name;

		public int m_Depth;

		public float m_PreviewSize = -1f;

		public int m_ID;

		public SearchFilter m_Filter;

		public SavedFilter(string name, SearchFilter filter, int depth, float previewSize)
		{
			this.m_Name = name;
			this.m_Depth = depth;
			this.m_Filter = filter;
			this.m_PreviewSize = previewSize;
		}
	}
}
