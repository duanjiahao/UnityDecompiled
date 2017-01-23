using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor
{
	public sealed class ArrayUtility
	{
		public static void Add<T>(ref T[] array, T item)
		{
			Array.Resize<T>(ref array, array.Length + 1);
			array[array.Length - 1] = item;
		}

		public static bool ArrayEquals<T>(T[] lhs, T[] rhs)
		{
			bool result;
			if (lhs.Length != rhs.Length)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < lhs.Length; i++)
				{
					if (!lhs[i].Equals(rhs[i]))
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}

		public static void AddRange<T>(ref T[] array, T[] items)
		{
			int num = array.Length;
			Array.Resize<T>(ref array, array.Length + items.Length);
			for (int i = 0; i < items.Length; i++)
			{
				array[num + i] = items[i];
			}
		}

		public static void Insert<T>(ref T[] array, int index, T item)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(array);
			arrayList.Insert(index, item);
			array = (arrayList.ToArray(typeof(T)) as T[]);
		}

		public static void Remove<T>(ref T[] array, T item)
		{
			List<T> list = new List<T>(array);
			list.Remove(item);
			array = list.ToArray();
		}

		public static List<T> FindAll<T>(T[] array, Predicate<T> match)
		{
			List<T> list = new List<T>(array);
			return list.FindAll(match);
		}

		public static T Find<T>(T[] array, Predicate<T> match)
		{
			List<T> list = new List<T>(array);
			return list.Find(match);
		}

		public static int FindIndex<T>(T[] array, Predicate<T> match)
		{
			List<T> list = new List<T>(array);
			return list.FindIndex(match);
		}

		public static int IndexOf<T>(T[] array, T value)
		{
			List<T> list = new List<T>(array);
			return list.IndexOf(value);
		}

		public static int LastIndexOf<T>(T[] array, T value)
		{
			List<T> list = new List<T>(array);
			return list.LastIndexOf(value);
		}

		public static void RemoveAt<T>(ref T[] array, int index)
		{
			List<T> list = new List<T>(array);
			list.RemoveAt(index);
			array = list.ToArray();
		}

		public static bool Contains<T>(T[] array, T item)
		{
			List<T> list = new List<T>(array);
			return list.Contains(item);
		}

		public static void Clear<T>(ref T[] array)
		{
			Array.Clear(array, 0, array.Length);
			Array.Resize<T>(ref array, 0);
		}
	}
}
