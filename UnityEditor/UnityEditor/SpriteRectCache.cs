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
				int result;
				if (this.m_Rects != null)
				{
					result = this.m_Rects.Count;
				}
				else
				{
					result = 0;
				}
				return result;
			}
		}

		public SpriteRect RectAt(int i)
		{
			SpriteRect result;
			if (i >= this.Count)
			{
				result = null;
			}
			else
			{
				result = this.m_Rects[i];
			}
			return result;
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
			int result;
			if (this.m_Rects != null)
			{
				result = this.m_Rects.FindIndex((SpriteRect p) => p.Equals(spriteRect));
			}
			else
			{
				result = 0;
			}
			return result;
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
