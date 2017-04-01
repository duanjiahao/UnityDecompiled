using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerTexture : DownloadHandler
	{
		public Texture2D texture
		{
			get
			{
				return this.InternalGetTexture();
			}
		}

		public DownloadHandlerTexture()
		{
			base.InternalCreateTexture(true);
		}

		public DownloadHandlerTexture(bool readable)
		{
			base.InternalCreateTexture(readable);
		}

		protected override byte[] GetData()
		{
			return this.InternalGetData();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Texture2D InternalGetTexture();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern byte[] InternalGetData();

		public static Texture2D GetContent(UnityWebRequest www)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerTexture>(www).texture;
		}
	}
}
