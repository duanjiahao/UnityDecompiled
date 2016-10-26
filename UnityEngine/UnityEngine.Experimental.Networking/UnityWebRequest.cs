using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.Networking
{
	[Obsolete("UnityWebRequest has moved into the UnityEngine.Networking namespace (UnityUpgradable) -> UnityEngine.Networking.UnityWebRequest")]
	public class UnityWebRequest : IDisposable
	{
		public const string kHttpVerbGET = "GET";

		public const string kHttpVerbHEAD = "HEAD";

		public const string kHttpVerbPOST = "POST";

		public const string kHttpVerbPUT = "PUT";

		public const string kHttpVerbCREATE = "CREATE";

		public const string kHttpVerbDELETE = "DELETE";

		public bool disposeDownloadHandlerOnDispose
		{
			get;
			set;
		}

		public bool disposeUploadHandlerOnDispose
		{
			get;
			set;
		}

		public string method
		{
			get;
			set;
		}

		public string error
		{
			get
			{
				return null;
			}
		}

		public bool useHttpContinue
		{
			get;
			set;
		}

		public string url
		{
			get;
			set;
		}

		public long responseCode
		{
			get
			{
				return 0L;
			}
		}

		public float uploadProgress
		{
			get
			{
				return 0f;
			}
		}

		public bool isModifiable
		{
			get
			{
				return false;
			}
		}

		public bool isDone
		{
			get
			{
				return false;
			}
		}

		public bool isError
		{
			get
			{
				return false;
			}
		}

		public float downloadProgress
		{
			get
			{
				return 0f;
			}
		}

		public ulong uploadedBytes
		{
			get
			{
				return 0uL;
			}
		}

		public ulong downloadedBytes
		{
			get
			{
				return 0uL;
			}
		}

		public int redirectLimit
		{
			get;
			set;
		}

		public bool chunkedTransfer
		{
			get;
			set;
		}

		public UploadHandler uploadHandler
		{
			get;
			set;
		}

		public DownloadHandler downloadHandler
		{
			get;
			set;
		}

		public UnityWebRequest()
		{
		}

		public UnityWebRequest(string url)
		{
		}

		public UnityWebRequest(string url, string method)
		{
		}

		public UnityWebRequest(string url, string method, DownloadHandler downloadHandler, UploadHandler uploadHandler)
		{
		}

		public void Dispose()
		{
		}

		public AsyncOperation Send()
		{
			return null;
		}

		public void Abort()
		{
		}

		public string GetRequestHeader(string name)
		{
			return null;
		}

		public void SetRequestHeader(string name, string value)
		{
		}

		public string GetResponseHeader(string name)
		{
			return null;
		}

		public Dictionary<string, string> GetResponseHeaders()
		{
			return null;
		}
	}
}
