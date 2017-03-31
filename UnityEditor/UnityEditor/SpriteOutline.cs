using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class SpriteOutline
	{
		[SerializeField]
		public List<Vector2> m_Path = new List<Vector2>();

		public Vector2 this[int index]
		{
			get
			{
				return this.m_Path[index];
			}
			set
			{
				this.m_Path[index] = value;
			}
		}

		public int Count
		{
			get
			{
				return this.m_Path.Count;
			}
		}

		public void Add(Vector2 point)
		{
			this.m_Path.Add(point);
		}

		public void Insert(int index, Vector2 point)
		{
			this.m_Path.Insert(index, point);
		}

		public void RemoveAt(int index)
		{
			this.m_Path.RemoveAt(index);
		}

		public void AddRange(IEnumerable<Vector2> addRange)
		{
			this.m_Path.AddRange(addRange);
		}
	}
}
