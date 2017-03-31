using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal sealed class AsyncHTTPClient
	{
		private delegate void RequestProgressCallback(AsyncHTTPClient.State status, int downloaded, int totalSize);

		private delegate void RequestDoneCallback(AsyncHTTPClient.State status, int httpStatus);

		internal enum State
		{
			INIT,
			CONNECTING,
			CONNECTED,
			UPLOADING,
			DOWNLOADING,
			CONFIRMING,
			DONE_OK,
			DONE_FAILED,
			ABORTED,
			TIMEOUT
		}

		public delegate void DoneCallback(AsyncHTTPClient client);

		public delegate void StatusCallback(AsyncHTTPClient.State status, int bytesDone, int bytesTotal);

		private IntPtr m_Handle;

		public AsyncHTTPClient.StatusCallback statusCallback;

		public AsyncHTTPClient.DoneCallback doneCallback;

		private string m_ToUrl;

		private string m_FromData;

		private string m_Method;

		public Dictionary<string, string> header;

		public string url
		{
			get
			{
				return this.m_ToUrl;
			}
		}

		public string text
		{
			get
			{
				UTF8Encoding uTF8Encoding = new UTF8Encoding();
				byte[] bytes = this.bytes;
				string result;
				if (bytes == null)
				{
					result = null;
				}
				else
				{
					result = uTF8Encoding.GetString(bytes);
				}
				return result;
			}
		}

		public byte[] bytes
		{
			get
			{
				return AsyncHTTPClient.GetBytesByHandle(this.m_Handle);
			}
		}

		public Texture2D texture
		{
			get
			{
				return AsyncHTTPClient.GetTextureByHandle(this.m_Handle);
			}
		}

		public AsyncHTTPClient.State state
		{
			get;
			private set;
		}

		public int responseCode
		{
			get;
			private set;
		}

		public string tag
		{
			get;
			set;
		}

		public string postData
		{
			set
			{
				this.m_FromData = value;
				if (this.m_Method == "")
				{
					this.m_Method = "POST";
				}
				if (!this.header.ContainsKey("Content-Type"))
				{
					this.header["Content-Type"] = "application/x-www-form-urlencoded";
				}
			}
		}

		public Dictionary<string, string> postDictionary
		{
			set
			{
				this.postData = string.Join("&", (from kv in value
				select this.EscapeLong(kv.Key) + "=" + this.EscapeLong(kv.Value)).ToArray<string>());
			}
		}

		public AsyncHTTPClient(string _toUrl)
		{
			this.m_ToUrl = _toUrl;
			this.m_FromData = null;
			this.m_Method = "";
			this.state = AsyncHTTPClient.State.INIT;
			this.header = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.m_Handle = (IntPtr)0;
			this.tag = "";
			this.statusCallback = null;
		}

		public AsyncHTTPClient(string _toUrl, string _method)
		{
			this.m_ToUrl = _toUrl;
			this.m_FromData = null;
			this.m_Method = _method;
			this.state = AsyncHTTPClient.State.INIT;
			this.header = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.m_Handle = (IntPtr)0;
			this.tag = "";
			this.statusCallback = null;
		}

		private static IntPtr SubmitClientRequest(string tag, string url, string[] headers, string method, string data, AsyncHTTPClient.RequestDoneCallback doneDelegate, [DefaultValue("null")] AsyncHTTPClient.RequestProgressCallback progressDelegate)
		{
			IntPtr result;
			AsyncHTTPClient.INTERNAL_CALL_SubmitClientRequest(tag, url, headers, method, data, doneDelegate, progressDelegate, out result);
			return result;
		}

		[ExcludeFromDocs]
		private static IntPtr SubmitClientRequest(string tag, string url, string[] headers, string method, string data, AsyncHTTPClient.RequestDoneCallback doneDelegate)
		{
			AsyncHTTPClient.RequestProgressCallback progressDelegate = null;
			IntPtr result;
			AsyncHTTPClient.INTERNAL_CALL_SubmitClientRequest(tag, url, headers, method, data, doneDelegate, progressDelegate, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SubmitClientRequest(string tag, string url, string[] headers, string method, string data, AsyncHTTPClient.RequestDoneCallback doneDelegate, AsyncHTTPClient.RequestProgressCallback progressDelegate, out IntPtr value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern byte[] GetBytesByHandle(IntPtr handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D GetTextureByHandle(IntPtr handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AbortByTag(string tag);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AbortByHandle(IntPtr handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CurlRequestCheck();

		public void Abort()
		{
			this.state = AsyncHTTPClient.State.ABORTED;
			AsyncHTTPClient.AbortByHandle(this.m_Handle);
		}

		public bool IsAborted()
		{
			return this.state == AsyncHTTPClient.State.ABORTED;
		}

		public bool IsDone()
		{
			return AsyncHTTPClient.IsDone(this.state);
		}

		public static bool IsDone(AsyncHTTPClient.State state)
		{
			bool result;
			switch (state)
			{
			case AsyncHTTPClient.State.DONE_OK:
			case AsyncHTTPClient.State.DONE_FAILED:
			case AsyncHTTPClient.State.ABORTED:
			case AsyncHTTPClient.State.TIMEOUT:
				result = true;
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public bool IsSuccess()
		{
			return this.state == AsyncHTTPClient.State.DONE_OK;
		}

		public static bool IsSuccess(AsyncHTTPClient.State state)
		{
			return state == AsyncHTTPClient.State.DONE_OK;
		}

		public void Begin()
		{
			if (this.IsAborted())
			{
				this.state = AsyncHTTPClient.State.ABORTED;
			}
			else
			{
				if (this.m_Method == "")
				{
					this.m_Method = "GET";
				}
				string[] headers = (from kv in this.header
				select string.Format("{0}: {1}", kv.Key, kv.Value)).ToArray<string>();
				this.m_Handle = AsyncHTTPClient.SubmitClientRequest(this.tag, this.m_ToUrl, headers, this.m_Method, this.m_FromData, new AsyncHTTPClient.RequestDoneCallback(this.Done), new AsyncHTTPClient.RequestProgressCallback(this.Progress));
			}
		}

		private void Done(AsyncHTTPClient.State status, int i_ResponseCode)
		{
			this.state = status;
			this.responseCode = i_ResponseCode;
			if (this.doneCallback != null)
			{
				this.doneCallback(this);
			}
			this.m_Handle = (IntPtr)0;
		}

		private void Progress(AsyncHTTPClient.State status, int bytesDone, int bytesTotal)
		{
			this.state = status;
			if (this.statusCallback != null)
			{
				this.statusCallback(status, bytesDone, bytesTotal);
			}
		}

		private string EscapeLong(string v)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < v.Length; i += 32766)
			{
				stringBuilder.Append(Uri.EscapeDataString(v.Substring(i, (v.Length - i <= 32766) ? (v.Length - i) : 32766)));
			}
			return stringBuilder.ToString();
		}
	}
}
