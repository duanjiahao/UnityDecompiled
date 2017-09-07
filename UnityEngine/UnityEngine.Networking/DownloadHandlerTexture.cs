using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerTexture : DownloadHandler
	{
		private Texture2D mTexture;

		private bool mHasTexture;

		private bool mNonReadable;

		public Texture2D texture
		{
			get
			{
				return this.InternalGetTexture();
			}
		}

		public DownloadHandlerTexture()
		{
			this.InternalCreateTexture(true);
		}

		[RequiredByNativeCode]
		public DownloadHandlerTexture(bool readable)
		{
			this.InternalCreateTexture(readable);
			this.mNonReadable = !readable;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalCreateTexture(bool readable);

		protected override byte[] GetData()
		{
			return this.InternalGetData();
		}

		private Texture2D InternalGetTexture()
		{
			if (this.mHasTexture)
			{
				if (this.mTexture == null)
				{
					this.mTexture = new Texture2D(2, 2);
					this.mTexture.LoadImage(this.GetData(), this.mNonReadable);
				}
			}
			else if (this.mTexture == null)
			{
				this.mTexture = this.InternalGetTextureNative();
				this.mHasTexture = true;
			}
			return this.mTexture;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Texture2D InternalGetTextureNative();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern byte[] InternalGetData();

		public static Texture2D GetContent(UnityWebRequest www)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerTexture>(www).texture;
		}
	}
}
