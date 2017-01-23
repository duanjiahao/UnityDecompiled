using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.Networking
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerAudioClip : DownloadHandler
	{
		public extern AudioClip audioClip
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public DownloadHandlerAudioClip(string url, AudioType audioType)
		{
			base.InternalCreateAudioClip(url, audioType);
		}

		protected override byte[] GetData()
		{
			return this.InternalGetData();
		}

		protected override string GetText()
		{
			throw new NotSupportedException("String access is not supported for audio clips");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern byte[] InternalGetData();

		public static AudioClip GetContent(UnityWebRequest www)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerAudioClip>(www).audioClip;
		}
	}
}
