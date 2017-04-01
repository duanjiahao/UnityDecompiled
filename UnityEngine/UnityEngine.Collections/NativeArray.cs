using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityEngine.Collections
{
	[DebuggerDisplay("Length = {Length}"), DebuggerTypeProxy(typeof(NativeArrayDebugView<>)), NativeContainer, NativeContainerSupportsMinMaxWriteRestriction]
	public struct NativeArray<T> : IDisposable, IEnumerable<T>, IEnumerable where T : struct
	{
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private NativeArray<T> array;

			private int index;

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public T Current
			{
				get
				{
					return this.array[this.index];
				}
			}

			public Enumerator(ref NativeArray<T> array)
			{
				this.array = array;
				this.index = -1;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				this.index++;
				return this.index < this.array.Length;
			}

			public void Reset()
			{
				this.index = -1;
			}
		}

		private IntPtr m_Buffer;

		private int m_Length;

		private int m_Stride;

		private readonly int m_MinIndex;

		private readonly int m_MaxIndex;

		private readonly AtomicSafetyHandle m_Safety;

		private DisposeSentinel m_DisposeSentinel;

		private readonly Allocator m_AllocatorLabel;

		public int Length
		{
			get
			{
				return this.m_Length;
			}
		}

		public unsafe T this[int index]
		{
			get
			{
				AtomicSafetyHandleVersionMask* ptr = (AtomicSafetyHandleVersionMask*)((void*)this.m_Safety.versionNode);
				if ((this.m_Safety.version & AtomicSafetyHandleVersionMask.Read) == (AtomicSafetyHandleVersionMask)0 && this.m_Safety.version != (*ptr & AtomicSafetyHandleVersionMask.ReadInv))
				{
					AtomicSafetyHandle.CheckReadAndThrowNoEarlyOut(this.m_Safety);
				}
				if (index < this.m_MinIndex || index > this.m_MaxIndex)
				{
					this.FailOutOfRangeError(index);
				}
				return UnsafeUtility.ReadArrayElement<T>(this.m_Buffer, index * this.m_Stride);
			}
			set
			{
				AtomicSafetyHandleVersionMask* ptr = (AtomicSafetyHandleVersionMask*)((void*)this.m_Safety.versionNode);
				if ((this.m_Safety.version & AtomicSafetyHandleVersionMask.Write) == (AtomicSafetyHandleVersionMask)0 && this.m_Safety.version != (*ptr & AtomicSafetyHandleVersionMask.WriteInv))
				{
					AtomicSafetyHandle.CheckWriteAndThrowNoEarlyOut(this.m_Safety);
				}
				if (index < this.m_MinIndex || index > this.m_MaxIndex)
				{
					this.FailOutOfRangeError(index);
				}
				UnsafeUtility.WriteArrayElement<T>(this.m_Buffer, index * this.m_Stride, value);
			}
		}

		public NativeArray(int length, Allocator allocMode)
		{
			NativeArray<T>.Allocate(length, allocMode, out this);
		}

		public NativeArray(T[] array, Allocator allocMode)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			NativeArray<T>.Allocate(array.Length, allocMode, out this);
			this.FromArray(array);
		}

		internal NativeArray(IntPtr dataPointer, int length)
		{
			this = new NativeArray<T>(dataPointer, length, Allocator.None);
		}

		internal NativeArray(IntPtr dataPointer, int length, int stride, AtomicSafetyHandle safety, Allocator allocMode)
		{
			this.m_Buffer = dataPointer;
			this.m_Length = length;
			this.m_Stride = stride;
			this.m_AllocatorLabel = allocMode;
			this.m_MinIndex = 0;
			this.m_MaxIndex = length - 1;
			this.m_Safety = safety;
			this.m_DisposeSentinel = null;
		}

		private NativeArray(IntPtr dataPointer, int length, Allocator allocMode)
		{
			if (dataPointer == IntPtr.Zero)
			{
				throw new ArgumentOutOfRangeException("dataPointer", "Pointer must not be zero");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Length must be >= 0");
			}
			this.m_Buffer = dataPointer;
			this.m_Length = length;
			this.m_Stride = UnsafeUtility.SizeOf<T>();
			this.m_AllocatorLabel = allocMode;
			this.m_MinIndex = 0;
			this.m_MaxIndex = length - 1;
			this.m_Safety = AtomicSafetyHandle.Create();
			this.m_DisposeSentinel = ((allocMode != Allocator.None) ? DisposeSentinel.Create(IntPtr.Zero, Allocator.Invalid, 0, null) : null);
		}

		public void Dispose()
		{
			AtomicSafetyHandle.CheckDeallocateAndThrow(this.m_Safety);
			AtomicSafetyHandle.Release(this.m_Safety);
			DisposeSentinel.Clear(ref this.m_DisposeSentinel);
			UnsafeUtility.Free(this.m_Buffer, this.m_AllocatorLabel);
			this.m_Buffer = IntPtr.Zero;
			this.m_Length = 0;
		}

		public IntPtr GetUnsafeReadBufferPtr()
		{
			AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
			return this.m_Buffer;
		}

		public IntPtr GetUnsafeWriteBufferPtr()
		{
			AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
			return this.m_Buffer;
		}

		public void FromArray(T[] array)
		{
			AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
			if (this.Length != array.Length)
			{
				throw new ArgumentException("Array length does not match the length of this instance");
			}
			for (int i = 0; i < this.Length; i++)
			{
				UnsafeUtility.WriteArrayElement<T>(this.m_Buffer, i * this.m_Stride, array[i]);
			}
		}

		public T[] ToArray()
		{
			AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
			T[] array = new T[this.Length];
			for (int i = 0; i < this.Length; i++)
			{
				array[i] = UnsafeUtility.ReadArrayElement<T>(this.m_Buffer, i * this.m_Stride);
			}
			return array;
		}

		private void FailOutOfRangeError(int index)
		{
			if (index < this.Length && (this.m_MinIndex != 0 || this.m_MaxIndex != this.Length - 1))
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} is out of restricted IComputeForEach range [{1}...{2}] in ReadWriteBuffer.\nReadWriteBuffers are restricted to only read & write the element at the job index. You can use double buffering strategies to avoid race conditions due to reading & writing in parallel to the same elements from a job.", index, this.m_MinIndex, this.m_MaxIndex));
			}
			throw new IndexOutOfRangeException(string.Format("Index {0} is out of range of '{1}' Length.", index, this.Length));
		}

		private static void Allocate(int length, Allocator allocMode, out NativeArray<T> outArray)
		{
			if (allocMode <= Allocator.None)
			{
				throw new ArgumentOutOfRangeException("allocMode", "Allocator must be Temp, Job or Persistent");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Length must be >= 0");
			}
			long num = (long)UnsafeUtility.SizeOf<T>() * (long)length;
			if (num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("length", "Length * sizeof(T) cannot exceed " + 2147483647 + "bytes");
			}
			outArray = new NativeArray<T>(UnsafeUtility.Malloc((int)num, UnsafeUtility.AlignOf<T>(), allocMode), length, allocMode);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new NativeArray<T>.Enumerator(ref this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
