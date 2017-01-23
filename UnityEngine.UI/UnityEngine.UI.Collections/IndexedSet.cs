using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI.Collections
{
	internal class IndexedSet<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private readonly List<T> m_List = new List<T>();

		private Dictionary<T, int> m_Dictionary = new Dictionary<T, int>();

		public int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public T this[int index]
		{
			get
			{
				return this.m_List[index];
			}
			set
			{
				T key = this.m_List[index];
				this.m_Dictionary.Remove(key);
				this.m_List[index] = value;
				this.m_Dictionary.Add(key, index);
			}
		}

		public void Add(T item)
		{
			this.m_List.Add(item);
			this.m_Dictionary.Add(item, this.m_List.Count - 1);
		}

		public bool AddUnique(T item)
		{
			bool result;
			if (this.m_Dictionary.ContainsKey(item))
			{
				result = false;
			}
			else
			{
				this.m_List.Add(item);
				this.m_Dictionary.Add(item, this.m_List.Count - 1);
				result = true;
			}
			return result;
		}

		public bool Remove(T item)
		{
			int index = -1;
			bool result;
			if (!this.m_Dictionary.TryGetValue(item, out index))
			{
				result = false;
			}
			else
			{
				this.RemoveAt(index);
				result = true;
			}
			return result;
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Clear()
		{
			this.m_List.Clear();
			this.m_Dictionary.Clear();
		}

		public bool Contains(T item)
		{
			return this.m_Dictionary.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.m_List.CopyTo(array, arrayIndex);
		}

		public int IndexOf(T item)
		{
			int result = -1;
			this.m_Dictionary.TryGetValue(item, out result);
			return result;
		}

		public void Insert(int index, T item)
		{
			throw new NotSupportedException("Random Insertion is semantically invalid, since this structure does not guarantee ordering.");
		}

		public void RemoveAt(int index)
		{
			T key = this.m_List[index];
			this.m_Dictionary.Remove(key);
			if (index == this.m_List.Count - 1)
			{
				this.m_List.RemoveAt(index);
			}
			else
			{
				int index2 = this.m_List.Count - 1;
				T t = this.m_List[index2];
				this.m_List[index] = t;
				this.m_Dictionary[t] = index;
				this.m_List.RemoveAt(index2);
			}
		}

		public void RemoveAll(Predicate<T> match)
		{
			int i = 0;
			while (i < this.m_List.Count)
			{
				T t = this.m_List[i];
				if (match(t))
				{
					this.Remove(t);
				}
				else
				{
					i++;
				}
			}
		}

		public void Sort(Comparison<T> sortLayoutFunction)
		{
			this.m_List.Sort(sortLayoutFunction);
			for (int i = 0; i < this.m_List.Count; i++)
			{
				T key = this.m_List[i];
				this.m_Dictionary[key] = i;
			}
		}
	}
}
