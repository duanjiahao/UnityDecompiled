using System;
using UnityEngine;

namespace UnityEditor
{
	internal class WebTemplate
	{
		public string m_Path;

		public string m_Name;

		public Texture2D m_Thumbnail;

		public string[] m_CustomKeys;

		public string[] CustomKeys
		{
			get
			{
				return this.m_CustomKeys;
			}
		}

		public override bool Equals(object other)
		{
			return other is WebTemplate && other.ToString().Equals(this.ToString());
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.m_Path.GetHashCode();
		}

		public override string ToString()
		{
			return this.m_Path;
		}

		public GUIContent ToGUIContent(Texture2D defaultIcon)
		{
			return new GUIContent(this.m_Name, (!(this.m_Thumbnail == null)) ? this.m_Thumbnail : defaultIcon);
		}
	}
}
