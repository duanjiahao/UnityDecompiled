using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerMovieTexture : DownloadHandler
	{
		public extern MovieTexture movieTexture
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public DownloadHandlerMovieTexture()
		{
			this.InternalCreateDHMovieTexture();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalCreateDHMovieTexture();

		protected override byte[] GetData()
		{
			return this.InternalGetData();
		}

		protected override string GetText()
		{
			throw new NotSupportedException("String access is not supported for movies");
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern byte[] InternalGetData();

		public static MovieTexture GetContent(UnityWebRequest uwr)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerMovieTexture>(uwr).movieTexture;
		}
	}
}
