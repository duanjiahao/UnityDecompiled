using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.VR.WSA.Persistence
{
	public sealed class WorldAnchorStore
	{
		public delegate void GetAsyncDelegate(WorldAnchorStore store);

		private static WorldAnchorStore s_Instance = null;

		private IntPtr m_NativePtr = IntPtr.Zero;

		public int anchorCount
		{
			get
			{
				return WorldAnchorStore.GetAnchorCount_Internal(this.m_NativePtr);
			}
		}

		private WorldAnchorStore(IntPtr nativePtr)
		{
			this.m_NativePtr = nativePtr;
		}

		public static void GetAsync(WorldAnchorStore.GetAsyncDelegate onCompleted)
		{
			if (WorldAnchorStore.s_Instance != null)
			{
				onCompleted(WorldAnchorStore.s_Instance);
			}
			else
			{
				WorldAnchorStore.GetAsync_Internal(onCompleted);
			}
		}

		public bool Save(string id, WorldAnchor anchor)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id must not be null or empty", "id");
			}
			if (anchor == null)
			{
				throw new ArgumentNullException("anchor");
			}
			return WorldAnchorStore.Save_Internal(this.m_NativePtr, id, anchor);
		}

		public WorldAnchor Load(string id, GameObject go)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id must not be null or empty", "id");
			}
			if (go == null)
			{
				throw new ArgumentNullException("anchor");
			}
			WorldAnchor worldAnchor = go.GetComponent<WorldAnchor>();
			bool flag = worldAnchor != null;
			if (worldAnchor == null)
			{
				worldAnchor = go.AddComponent<WorldAnchor>();
			}
			WorldAnchor result;
			if (WorldAnchorStore.Load_Internal(this.m_NativePtr, id, worldAnchor))
			{
				result = go.GetComponent<WorldAnchor>();
			}
			else
			{
				if (!flag)
				{
					UnityEngine.Object.DestroyImmediate(worldAnchor);
				}
				result = null;
			}
			return result;
		}

		public bool Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id must not be null or empty", "id");
			}
			return WorldAnchorStore.Delete_Internal(this.m_NativePtr, id);
		}

		public void Clear()
		{
			WorldAnchorStore.Clear_Internal(this.m_NativePtr);
		}

		public int GetAllIds(string[] ids)
		{
			if (ids == null)
			{
				throw new ArgumentNullException("ids");
			}
			int result;
			if (ids.Length > 0)
			{
				result = WorldAnchorStore.GetAllIds_Internal(this.m_NativePtr, ids);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public string[] GetAllIds()
		{
			string[] array = new string[this.anchorCount];
			this.GetAllIds(array);
			return array;
		}

		[RequiredByNativeCode]
		private static void InvokeGetAsyncDelegate(WorldAnchorStore.GetAsyncDelegate handler, IntPtr nativePtr)
		{
			WorldAnchorStore.s_Instance = new WorldAnchorStore(nativePtr);
			handler(WorldAnchorStore.s_Instance);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAsync_Internal(WorldAnchorStore.GetAsyncDelegate onCompleted);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Save_Internal(IntPtr context, string id, WorldAnchor anchor);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Load_Internal(IntPtr context, string id, WorldAnchor anchor);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Delete_Internal(IntPtr context, string id);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Clear_Internal(IntPtr context);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAnchorCount_Internal(IntPtr context);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAllIds_Internal(IntPtr context, string[] ids);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destruct_Internal(IntPtr context);
	}
}
