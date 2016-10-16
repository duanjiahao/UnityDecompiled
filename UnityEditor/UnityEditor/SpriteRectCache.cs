using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class SpriteRectCache : ScriptableObject
	{
		[SerializeField]
		public List<SpriteRect> m_Rects;

		public int Count
		{
			get
			{
				if (this.m_Rects != null)
				{
					return this.m_Rects.Count;
				}
				return 0;
			}
		}

		public SpriteRect RectAt(int i)
		{
			if (i >= this.Count)
			{
				return null;
			}
			return this.m_Rects[i];
		}

		public void AddRect(SpriteRect r)
		{
			if (this.m_Rects != null)
			{
				this.m_Rects.Add(r);
			}
		}

		public void RemoveRect(SpriteRect r)
		{
			if (this.m_Rects != null)
			{
				this.m_Rects.Remove(r);
			}
		}

		public void ClearAll()
		{
			if (this.m_Rects != null)
			{
				this.m_Rects.Clear();
			}
		}

		public int GetIndex(SpriteRect spriteRect)
		{
			if (this.m_Rects != null)
			{
				return this.m_Rects.FindIndex((SpriteRect p) => p.Equals(spriteRect));
			}
			return 0;
		}

		public bool Contains(SpriteRect spriteRect)
		{
			return this.m_Rects != null && this.m_Rects.Contains(spriteRect);
		}

		private void OnEnable()
		{
			if (this.m_Rects == null)
			{
				this.m_Rects = new List<SpriteRect>();
			}
		}
	}
}
