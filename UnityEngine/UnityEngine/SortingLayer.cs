using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public struct SortingLayer
	{
		private int m_Id;

		public int id
		{
			get
			{
				return this.m_Id;
			}
		}

		public string name
		{
			get
			{
				return SortingLayer.IDToName(this.m_Id);
			}
		}

		public int value
		{
			get
			{
				return SortingLayer.GetLayerValueFromID(this.m_Id);
			}
		}

		public static SortingLayer[] layers
		{
			get
			{
				int[] sortingLayerIDsInternal = SortingLayer.GetSortingLayerIDsInternal();
				SortingLayer[] array = new SortingLayer[sortingLayerIDsInternal.Length];
				for (int i = 0; i < sortingLayerIDsInternal.Length; i++)
				{
					array[i].m_Id = sortingLayerIDsInternal[i];
				}
				return array;
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int[] GetSortingLayerIDsInternal();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetLayerValueFromID(int id);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetLayerValueFromName(string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int NameToID(string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string IDToName(int id);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsValid(int id);
	}
}
