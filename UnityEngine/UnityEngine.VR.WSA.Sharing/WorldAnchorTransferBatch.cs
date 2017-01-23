using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.VR.WSA.Sharing
{
	public sealed class WorldAnchorTransferBatch : IDisposable
	{
		public delegate void SerializationDataAvailableDelegate(byte[] data);

		public delegate void SerializationCompleteDelegate(SerializationCompletionReason completionReason);

		public delegate void DeserializationCompleteDelegate(SerializationCompletionReason completionReason, WorldAnchorTransferBatch deserializedTransferBatch);

		private IntPtr m_NativePtr = IntPtr.Zero;

		public int anchorCount
		{
			get
			{
				return WorldAnchorTransferBatch.GetAnchorCount_Internal(this.m_NativePtr);
			}
		}

		public WorldAnchorTransferBatch()
		{
			this.m_NativePtr = WorldAnchorTransferBatch.Create_Internal();
		}

		private WorldAnchorTransferBatch(IntPtr nativePtr)
		{
			this.m_NativePtr = nativePtr;
		}

		public static void ExportAsync(WorldAnchorTransferBatch transferBatch, WorldAnchorTransferBatch.SerializationDataAvailableDelegate onDataAvailable, WorldAnchorTransferBatch.SerializationCompleteDelegate onCompleted)
		{
			if (transferBatch == null)
			{
				throw new ArgumentNullException("transferBatch");
			}
			if (onDataAvailable == null)
			{
				throw new ArgumentNullException("onDataAvailable");
			}
			if (onCompleted == null)
			{
				throw new ArgumentNullException("onCompleted");
			}
			WorldAnchorTransferBatch.ExportAsync_Internal(transferBatch.m_NativePtr, onDataAvailable, onCompleted);
		}

		public static void ImportAsync(byte[] serializedData, WorldAnchorTransferBatch.DeserializationCompleteDelegate onComplete)
		{
			WorldAnchorTransferBatch.ImportAsync(serializedData, 0, serializedData.Length, onComplete);
		}

		public static void ImportAsync(byte[] serializedData, int offset, int length, WorldAnchorTransferBatch.DeserializationCompleteDelegate onComplete)
		{
			if (serializedData == null)
			{
				throw new ArgumentNullException("serializedData");
			}
			if (serializedData.Length < 1)
			{
				throw new ArgumentException("serializedData is empty!", "serializedData");
			}
			if (offset + length > serializedData.Length)
			{
				throw new ArgumentException("offset + length is greater that serializedData.Length!");
			}
			if (onComplete == null)
			{
				throw new ArgumentNullException("onComplete");
			}
			WorldAnchorTransferBatch.ImportAsync_Internal(serializedData, offset, length, onComplete);
		}

		public bool AddWorldAnchor(string id, WorldAnchor anchor)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id is null or empty!", "id");
			}
			if (anchor == null)
			{
				throw new ArgumentNullException("anchor");
			}
			return WorldAnchorTransferBatch.AddWorldAnchor_Internal(this.m_NativePtr, id, anchor);
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
				result = WorldAnchorTransferBatch.GetAllIds_Internal(this.m_NativePtr, ids);
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

		public WorldAnchor LockObject(string id, GameObject go)
		{
			WorldAnchor worldAnchor = go.GetComponent<WorldAnchor>();
			bool flag = worldAnchor != null;
			if (worldAnchor == null)
			{
				worldAnchor = go.AddComponent<WorldAnchor>();
			}
			WorldAnchor result;
			if (WorldAnchorTransferBatch.LoadAnchor_Internal(this.m_NativePtr, id, worldAnchor))
			{
				result = worldAnchor;
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

		~WorldAnchorTransferBatch()
		{
			if (this.m_NativePtr != IntPtr.Zero)
			{
				WorldAnchorTransferBatch.DisposeThreaded_Internal(this.m_NativePtr);
				this.m_NativePtr = IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			if (this.m_NativePtr != IntPtr.Zero)
			{
				WorldAnchorTransferBatch.Dispose_Internal(this.m_NativePtr);
				this.m_NativePtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		[RequiredByNativeCode]
		private static void InvokeWorldAnchorSerializationDataAvailableDelegate(WorldAnchorTransferBatch.SerializationDataAvailableDelegate onSerializationDataAvailable, byte[] data)
		{
			onSerializationDataAvailable(data);
		}

		[RequiredByNativeCode]
		private static void InvokeWorldAnchorSerializationCompleteDelegate(WorldAnchorTransferBatch.SerializationCompleteDelegate onSerializationComplete, SerializationCompletionReason completionReason)
		{
			onSerializationComplete(completionReason);
		}

		[RequiredByNativeCode]
		private static void InvokeWorldAnchorDeserializationCompleteDelegate(WorldAnchorTransferBatch.DeserializationCompleteDelegate onDeserializationComplete, SerializationCompletionReason completionReason, IntPtr nativePtr)
		{
			WorldAnchorTransferBatch deserializedTransferBatch = new WorldAnchorTransferBatch(nativePtr);
			onDeserializationComplete(completionReason, deserializedTransferBatch);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExportAsync_Internal(IntPtr transferBatch, WorldAnchorTransferBatch.SerializationDataAvailableDelegate onDataAvailable, WorldAnchorTransferBatch.SerializationCompleteDelegate onComplete);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ImportAsync_Internal(byte[] serializedData, int offset, int length, WorldAnchorTransferBatch.DeserializationCompleteDelegate onComplete);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddWorldAnchor_Internal(IntPtr context, string id, WorldAnchor anchor);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAnchorCount_Internal(IntPtr context);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAllIds_Internal(IntPtr context, string[] ids);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool LoadAnchor_Internal(IntPtr context, string id, WorldAnchor anchor);

		private static IntPtr Create_Internal()
		{
			IntPtr result;
			WorldAnchorTransferBatch.INTERNAL_CALL_Create_Internal(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Create_Internal(out IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Dispose_Internal(IntPtr context);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisposeThreaded_Internal(IntPtr context);
	}
}
