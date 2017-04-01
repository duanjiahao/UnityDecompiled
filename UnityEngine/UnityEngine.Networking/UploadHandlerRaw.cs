using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class UploadHandlerRaw : UploadHandler
	{
		public UploadHandlerRaw(byte[] data)
		{
			base.InternalCreateRaw(data);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string InternalGetContentType();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetContentType(string newContentType);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern byte[] InternalGetData();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float InternalGetProgress();

		internal override string GetContentType()
		{
			return this.InternalGetContentType();
		}

		internal override void SetContentType(string newContentType)
		{
			this.InternalSetContentType(newContentType);
		}

		internal override byte[] GetData()
		{
			return this.InternalGetData();
		}

		internal override float GetProgress()
		{
			return this.InternalGetProgress();
		}
	}
}
