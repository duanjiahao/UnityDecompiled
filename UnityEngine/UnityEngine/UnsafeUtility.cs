using System;
using System.Runtime.CompilerServices;
using UnityEngine.Collections;

namespace UnityEngine
{
	internal static class UnsafeUtility
	{
		public unsafe static void CopyPtrToStructure<T>(IntPtr ptr, out T output) where T : struct
		{
			output = *ptr;
		}

		public unsafe static void CopyStructureToPtr<T>(ref T output, IntPtr ptr) where T : struct
		{
			*ptr = output;
		}

		public unsafe static T ReadArrayElement<T>(IntPtr source, int index)
		{
			return *(source + (IntPtr)index);
		}

		public unsafe static void WriteArrayElement<T>(IntPtr destination, int index, T value)
		{
			*(destination + (IntPtr)index) = value;
		}

		public static IntPtr AddressOf<T>(ref T output) where T : struct
		{
			return ref output;
		}

		public static int SizeOf<T>() where T : struct
		{
			return UnsafeUtility.SizeOfStruct(typeof(T));
		}

		public static int AlignOf<T>() where T : struct
		{
			return 4;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr Malloc(int size, int alignment, Allocator label);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Free(IntPtr memory, Allocator label);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MemCpy(IntPtr destination, IntPtr source, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int SizeOfStruct(Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LogError(string msg, string filename, int linenumber);
	}
}
