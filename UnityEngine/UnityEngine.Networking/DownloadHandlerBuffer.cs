using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerBuffer : DownloadHandler
	{
		public DownloadHandlerBuffer()
		{
			base.InternalCreateBuffer();
		}

		protected override byte[] GetData()
		{
			return this.InternalGetData();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern byte[] InternalGetData();

		public static string GetContent(UnityWebRequest www)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerBuffer>(www).text;
		}
	}
}
