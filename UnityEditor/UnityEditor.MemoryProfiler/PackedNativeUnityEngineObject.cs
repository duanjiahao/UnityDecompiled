using System;
using UnityEngine;

namespace UnityEditor.MemoryProfiler
{
	[Serializable]
	public struct PackedNativeUnityEngineObject
	{
		internal enum ObjectFlags
		{
			IsDontDestroyOnLoad = 1,
			IsPersistent,
			IsManager = 4
		}

		[SerializeField]
		internal string m_Name;

		[SerializeField]
		internal int m_InstanceId;

		[SerializeField]
		internal int m_Size;

		[SerializeField]
		internal int m_ClassId;

		[SerializeField]
		internal HideFlags m_HideFlags;

		[SerializeField]
		internal PackedNativeUnityEngineObject.ObjectFlags m_Flags;

		[SerializeField]
		internal long m_NativeObjectAddress;

		public bool isPersistent
		{
			get
			{
				return (this.m_Flags & PackedNativeUnityEngineObject.ObjectFlags.IsPersistent) != (PackedNativeUnityEngineObject.ObjectFlags)0;
			}
		}

		public bool isDontDestroyOnLoad
		{
			get
			{
				return (this.m_Flags & PackedNativeUnityEngineObject.ObjectFlags.IsDontDestroyOnLoad) != (PackedNativeUnityEngineObject.ObjectFlags)0;
			}
		}

		public bool isManager
		{
			get
			{
				return (this.m_Flags & PackedNativeUnityEngineObject.ObjectFlags.IsManager) != (PackedNativeUnityEngineObject.ObjectFlags)0;
			}
		}

		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		public int instanceId
		{
			get
			{
				return this.m_InstanceId;
			}
		}

		public int size
		{
			get
			{
				return this.m_Size;
			}
		}

		public int classId
		{
			get
			{
				return this.m_ClassId;
			}
		}

		public HideFlags hideFlags
		{
			get
			{
				return this.m_HideFlags;
			}
		}

		public long nativeObjectAddress
		{
			get
			{
				return this.m_NativeObjectAddress;
			}
		}
	}
}
