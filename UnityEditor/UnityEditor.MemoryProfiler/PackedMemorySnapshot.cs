using System;
using UnityEngine;

namespace UnityEditor.MemoryProfiler
{
	[Serializable]
	public class PackedMemorySnapshot
	{
		[SerializeField]
		internal PackedNativeType[] m_NativeTypes = null;

		[SerializeField]
		internal PackedNativeUnityEngineObject[] m_NativeObjects = null;

		[SerializeField]
		internal PackedGCHandle[] m_GcHandles = null;

		[SerializeField]
		internal Connection[] m_Connections = null;

		[SerializeField]
		internal MemorySection[] m_ManagedHeapSections = null;

		[SerializeField]
		internal MemorySection[] m_ManagedStacks = null;

		[SerializeField]
		internal TypeDescription[] m_TypeDescriptions = null;

		[SerializeField]
		internal VirtualMachineInformation m_VirtualMachineInformation = default(VirtualMachineInformation);

		public PackedNativeType[] nativeTypes
		{
			get
			{
				return this.m_NativeTypes;
			}
		}

		public PackedNativeUnityEngineObject[] nativeObjects
		{
			get
			{
				return this.m_NativeObjects;
			}
		}

		public PackedGCHandle[] gcHandles
		{
			get
			{
				return this.m_GcHandles;
			}
		}

		public Connection[] connections
		{
			get
			{
				return this.m_Connections;
			}
		}

		public MemorySection[] managedHeapSections
		{
			get
			{
				return this.m_ManagedHeapSections;
			}
		}

		public TypeDescription[] typeDescriptions
		{
			get
			{
				return this.m_TypeDescriptions;
			}
		}

		public VirtualMachineInformation virtualMachineInformation
		{
			get
			{
				return this.m_VirtualMachineInformation;
			}
		}

		internal PackedMemorySnapshot()
		{
		}
	}
}
