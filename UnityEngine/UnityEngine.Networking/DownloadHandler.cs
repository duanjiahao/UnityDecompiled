using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	[StructLayout(LayoutKind.Sequential)]
	public class DownloadHandler : IDisposable
	{
		[NonSerialized]
		internal IntPtr m_Ptr;

		public extern bool isDone
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public byte[] data
		{
			get
			{
				return this.GetData();
			}
		}

		public string text
		{
			get
			{
				return this.GetText();
			}
		}

		internal DownloadHandler()
		{
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalCreateBuffer();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalCreateScript();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalCreateTexture(bool readable);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalCreateAssetBundle(string url, uint crc);

		internal void InternalCreateAssetBundle(string url, Hash128 hash, uint crc)
		{
			DownloadHandler.INTERNAL_CALL_InternalCreateAssetBundle(this, url, ref hash, crc);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalCreateAssetBundle(DownloadHandler self, string url, ref Hash128 hash, uint crc);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalCreateAudioClip(string url, AudioType audioType);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalDestroy();

		~DownloadHandler()
		{
			this.InternalDestroy();
		}

		public void Dispose()
		{
			this.InternalDestroy();
			GC.SuppressFinalize(this);
		}

		protected virtual byte[] GetData()
		{
			return null;
		}

		protected virtual string GetText()
		{
			byte[] data = this.GetData();
			if (data != null && data.Length > 0)
			{
				return Encoding.UTF8.GetString(data, 0, data.Length);
			}
			return string.Empty;
		}

		[UsedByNativeCode]
		protected virtual bool ReceiveData(byte[] data, int dataLength)
		{
			return true;
		}

		[UsedByNativeCode]
		protected virtual void ReceiveContentLength(int contentLength)
		{
		}

		[UsedByNativeCode]
		protected virtual void CompleteContent()
		{
		}

		[UsedByNativeCode]
		protected virtual float GetProgress()
		{
			return 0f;
		}

		protected static T GetCheckedDownloader<T>(UnityWebRequest www) where T : DownloadHandler
		{
			if (www == null)
			{
				throw new NullReferenceException("Cannot get content from a null UnityWebRequest object");
			}
			if (!www.isDone)
			{
				throw new InvalidOperationException("Cannot get content from an unfinished UnityWebRequest object");
			}
			if (www.isError)
			{
				throw new InvalidOperationException(www.error);
			}
			return (T)((object)www.downloadHandler);
		}
	}
}
