using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class Gradient
	{
		internal IntPtr m_Ptr;

		public extern GradientColorKey[] colorKeys
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern GradientAlphaKey[] alphaKeys
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern GradientMode mode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal Color constantColor
		{
			get
			{
				Color result;
				this.INTERNAL_get_constantColor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_constantColor(ref value);
			}
		}

		[RequiredByNativeCode]
		public Gradient()
		{
			this.Init();
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Init();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Cleanup();

		~Gradient()
		{
			this.Cleanup();
		}

		public Color Evaluate(float time)
		{
			Color result;
			Gradient.INTERNAL_CALL_Evaluate(this, time, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Evaluate(Gradient self, float time, out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_constantColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_constantColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys);
	}
}
