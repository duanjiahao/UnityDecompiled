using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	internal sealed class iOSDeviceRequirementGroup
	{
		private string m_VariantName;

		public int count
		{
			get
			{
				return iOSDeviceRequirementGroup.GetCountForVariantImpl(this.m_VariantName);
			}
		}

		public iOSDeviceRequirement this[int index]
		{
			get
			{
				string[] array;
				string[] array2;
				iOSDeviceRequirementGroup.GetDeviceRequirementForVariantNameImpl(this.m_VariantName, index, out array, out array2);
				iOSDeviceRequirement iOSDeviceRequirement = new iOSDeviceRequirement();
				for (int i = 0; i < array.Length; i++)
				{
					iOSDeviceRequirement.values.Add(array[i], array2[i]);
				}
				return iOSDeviceRequirement;
			}
			set
			{
				iOSDeviceRequirementGroup.SetOrAddDeviceRequirementForVariantNameImpl(this.m_VariantName, index, value.values.Keys.ToArray<string>(), value.values.Values.ToArray<string>());
			}
		}

		internal iOSDeviceRequirementGroup(string variantName)
		{
			this.m_VariantName = variantName;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDeviceRequirementForVariantNameImpl(string name, int index, out string[] keys, out string[] values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetOrAddDeviceRequirementForVariantNameImpl(string name, int index, string[] keys, string[] values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetCountForVariantImpl(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveAtImpl(string name, int index);

		public void RemoveAt(int index)
		{
			iOSDeviceRequirementGroup.RemoveAtImpl(this.m_VariantName, index);
		}

		public void Add(iOSDeviceRequirement requirement)
		{
			iOSDeviceRequirementGroup.SetOrAddDeviceRequirementForVariantNameImpl(this.m_VariantName, -1, requirement.values.Keys.ToArray<string>(), requirement.values.Values.ToArray<string>());
		}
	}
}
