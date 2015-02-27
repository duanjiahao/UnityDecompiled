using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public struct LayerMask
	{
		private int m_Mask;
		public int value
		{
			get
			{
				return this.m_Mask;
			}
			set
			{
				this.m_Mask = value;
			}
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string LayerToName(int layer);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int NameToLayer(string layerName);
		public static int GetMask(params string[] layerNames)
		{
			int num = 0;
			for (int i = 0; i < layerNames.Length; i++)
			{
				string layerName = layerNames[i];
				int num2 = LayerMask.NameToLayer(layerName);
				if (num2 != 0)
				{
					num |= 1 << num2;
				}
			}
			return num;
		}
		public static implicit operator int(LayerMask mask)
		{
			return mask.m_Mask;
		}
		public static implicit operator LayerMask(int intVal)
		{
			LayerMask result;
			result.m_Mask = intVal;
			return result;
		}
	}
}
