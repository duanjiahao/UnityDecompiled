using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace UnityEngine.Networking
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class UnityWebRequest : IDisposable
	{
		internal enum UnityWebRequestMethod
		{
			Get,
			Post,
			Put,
			Head,
			Custom
		}

		internal enum UnityWebRequestError
		{
			OK,
			Unknown,
			SDKError,
			UnsupportedProtocol,
			MalformattedUrl,
			CannotResolveProxy,
			CannotResolveHost,
			CannotConnectToHost,
			AccessDenied,
			GenericHTTPError,
			WriteError,
			ReadError,
			OutOfMemory,
			Timeout,
			HTTPPostError,
			SSLCannotConnect,
			Aborted,
			TooManyRedirects,
			ReceivedNoData,
			SSLNotSupported,
			FailedToSendData,
			FailedToReceiveData,
			SSLCertificateError,
			SSLCipherNotAvailable,
			SSLCACertError,
			UnrecognizedContentEncoding,
			LoginFailed,
			SSLShutdownFailed
		}

		[NonSerialized]
		internal IntPtr m_Ptr;

		public const string kHttpVerbGET = "GET";

		public const string kHttpVerbHEAD = "HEAD";

		public const string kHttpVerbPOST = "POST";

		public const string kHttpVerbPUT = "PUT";

		public const string kHttpVerbCREATE = "CREATE";

		public const string kHttpVerbDELETE = "DELETE";

		private static Regex domainRegex = new Regex("^\\s*\\w+(?:\\.\\w+)+\\s*$");

		private static readonly string[] forbiddenHeaderKeys = new string[]
		{
			"accept-charset",
			"access-control-request-headers",
			"access-control-request-method",
			"connection",
			"content-length",
			"date",
			"dnt",
			"expect",
			"host",
			"keep-alive",
			"origin",
			"referer",
			"te",
			"trailer",
			"transfer-encoding",
			"upgrade",
			"user-agent",
			"via",
			"x-unity-version"
		};

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
			get
			{
				string result;
				switch (this.InternalGetMethod())
				{
				case 0:
					result = "GET";
					break;
				case 1:
					result = "POST";
					break;
				case 2:
					result = "PUT";
					break;
				case 3:
					result = "HEAD";
					break;
				default:
					result = this.InternalGetCustomMethod();
					break;
				}
				return result;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Cannot set a UnityWebRequest's method to an empty or null string");
				}
				string text = value.ToUpper();
				if (text != null)
				{
					if (text == "GET")
					{
						this.InternalSetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
						return;
					}
					if (text == "POST")
					{
						this.InternalSetMethod(UnityWebRequest.UnityWebRequestMethod.Post);
						return;
					}
					if (text == "PUT")
					{
						this.InternalSetMethod(UnityWebRequest.UnityWebRequestMethod.Put);
						return;
					}
					if (text == "HEAD")
					{
						this.InternalSetMethod(UnityWebRequest.UnityWebRequestMethod.Head);
						return;
					}
				}
				this.InternalSetCustomMethod(value.ToUpper());
			}
		}

		public extern string error
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool useHttpContinue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public string url
		{
			get
			{
				return this.InternalGetUrl();
			}
			set
			{
				string text = value;
				string uriString = "http://localhost/";
				Uri uri = new Uri(uriString);
				if (text.StartsWith("//"))
				{
					text = uri.Scheme + ":" + text;
				}
				if (text.StartsWith("/"))
				{
					text = uri.Scheme + "://" + uri.Host + text;
				}
				if (UnityWebRequest.domainRegex.IsMatch(text))
				{
					text = uri.Scheme + "://" + text;
				}
				Uri uri2 = null;
				try
				{
					uri2 = new Uri(text);
				}
				catch (FormatException ex)
				{
					try
					{
						uri2 = new Uri(uri, text);
					}
					catch (FormatException)
					{
						throw ex;
					}
				}
				this.InternalSetUrl(uri2.AbsoluteUri);
			}
		}

		public extern long responseCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float uploadProgress
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isModifiable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isDone
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isError
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float downloadProgress
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ulong uploadedBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ulong downloadedBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int redirectLimit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool chunkedTransfer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern UploadHandler uploadHandler
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern DownloadHandler downloadHandler
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public UnityWebRequest()
		{
			this.InternalCreate();
			this.InternalSetDefaults();
		}

		public UnityWebRequest(string url)
		{
			this.InternalCreate();
			this.InternalSetDefaults();
			this.url = url;
		}

		public UnityWebRequest(string url, string method)
		{
			this.InternalCreate();
			this.InternalSetDefaults();
			this.url = url;
			this.method = method;
		}

		public UnityWebRequest(string url, string method, DownloadHandler downloadHandler, UploadHandler uploadHandler)
		{
			this.InternalCreate();
			this.InternalSetDefaults();
			this.url = url;
			this.method = method;
			this.downloadHandler = downloadHandler;
			this.uploadHandler = uploadHandler;
		}

		public static UnityWebRequest Get(string uri)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerBuffer(), null);
		}

		public static UnityWebRequest Delete(string uri)
		{
			return new UnityWebRequest(uri, "DELETE");
		}

		public static UnityWebRequest Head(string uri)
		{
			return new UnityWebRequest(uri, "HEAD");
		}

		public static UnityWebRequest GetTexture(string uri)
		{
			return UnityWebRequest.GetTexture(uri, false);
		}

		public static UnityWebRequest GetTexture(string uri, bool nonReadable)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerTexture(nonReadable), null);
		}

		public static UnityWebRequest GetAudioClip(string uri, AudioType audioType)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAudioClip(uri, audioType), null);
		}

		public static UnityWebRequest GetAssetBundle(string uri)
		{
			return UnityWebRequest.GetAssetBundle(uri, 0u);
		}

		public static UnityWebRequest GetAssetBundle(string uri, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, crc), null);
		}

		public static UnityWebRequest GetAssetBundle(string uri, uint version, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, version, crc), null);
		}

		public static UnityWebRequest GetAssetBundle(string uri, Hash128 hash, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, hash, crc), null);
		}

		public static UnityWebRequest Put(string uri, byte[] bodyData)
		{
			return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(bodyData));
		}

		public static UnityWebRequest Put(string uri, string bodyData)
		{
			return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData)));
		}

		public static UnityWebRequest Post(string uri, string postData)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			string s = WWWTranscoder.URLEncode(postData, Encoding.UTF8);
			unityWebRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(s));
			unityWebRequest.uploadHandler.contentType = "application/x-www-form-urlencoded";
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			return unityWebRequest;
		}

		public static UnityWebRequest Post(string uri, WWWForm formData)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			unityWebRequest.uploadHandler = new UploadHandlerRaw(formData.data);
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			Dictionary<string, string> headers = formData.headers;
			foreach (KeyValuePair<string, string> current in headers)
			{
				unityWebRequest.SetRequestHeader(current.Key, current.Value);
			}
			return unityWebRequest;
		}

		public static UnityWebRequest Post(string uri, List<IMultipartFormSection> multipartFormSections)
		{
			byte[] boundary = UnityWebRequest.GenerateBoundary();
			return UnityWebRequest.Post(uri, multipartFormSections, boundary);
		}

		public static UnityWebRequest Post(string uri, List<IMultipartFormSection> multipartFormSections, byte[] boundary)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			byte[] data = UnityWebRequest.SerializeFormSections(multipartFormSections, boundary);
			unityWebRequest.uploadHandler = new UploadHandlerRaw(data)
			{
				contentType = "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary, 0, boundary.Length)
			};
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			return unityWebRequest;
		}

		public static UnityWebRequest Post(string uri, Dictionary<string, string> formFields)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			byte[] data = UnityWebRequest.SerializeSimpleForm(formFields);
			unityWebRequest.uploadHandler = new UploadHandlerRaw(data)
			{
				contentType = "application/x-www-form-urlencoded"
			};
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			return unityWebRequest;
		}

		public static byte[] SerializeFormSections(List<IMultipartFormSection> multipartFormSections, byte[] boundary)
		{
			byte[] bytes = Encoding.UTF8.GetBytes("\r\n");
			int num = 0;
			foreach (IMultipartFormSection current in multipartFormSections)
			{
				num += 64 + current.sectionData.Length;
			}
			List<byte> list = new List<byte>(num);
			foreach (IMultipartFormSection current2 in multipartFormSections)
			{
				string str = "form-data";
				string sectionName = current2.sectionName;
				string fileName = current2.fileName;
				if (!string.IsNullOrEmpty(fileName))
				{
					str = "file";
				}
				string text = "Content-Disposition: " + str;
				if (!string.IsNullOrEmpty(sectionName))
				{
					text = text + "; name=\"" + sectionName + "\"";
				}
				if (!string.IsNullOrEmpty(fileName))
				{
					text = text + "; filename=\"" + fileName + "\"";
				}
				text += "\r\n";
				string contentType = current2.contentType;
				if (!string.IsNullOrEmpty(contentType))
				{
					text = text + "Content-Type: " + contentType + "\r\n";
				}
				list.AddRange(boundary);
				list.AddRange(bytes);
				list.AddRange(Encoding.UTF8.GetBytes(text));
				list.AddRange(bytes);
				list.AddRange(current2.sectionData);
			}
			list.TrimExcess();
			return list.ToArray();
		}

		public static byte[] GenerateBoundary()
		{
			byte[] array = new byte[40];
			for (int i = 0; i < 40; i++)
			{
				int num = UnityEngine.Random.Range(48, 110);
				if (num > 57)
				{
					num += 7;
				}
				if (num > 90)
				{
					num += 6;
				}
				array[i] = (byte)num;
			}
			return array;
		}

		public static byte[] SerializeSimpleForm(Dictionary<string, string> formFields)
		{
			string text = "";
			foreach (KeyValuePair<string, string> current in formFields)
			{
				if (text.Length > 0)
				{
					text += "&";
				}
				text = text + Uri.EscapeDataString(current.Key) + "=" + Uri.EscapeDataString(current.Value);
			}
			return Encoding.UTF8.GetBytes(text);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalCreate();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalDestroy();

		private void InternalSetDefaults()
		{
			this.disposeDownloadHandlerOnDispose = true;
			this.disposeUploadHandlerOnDispose = true;
		}

		~UnityWebRequest()
		{
			this.InternalDestroy();
		}

		public void Dispose()
		{
			if (this.disposeDownloadHandlerOnDispose)
			{
				DownloadHandler downloadHandler = this.downloadHandler;
				if (downloadHandler != null)
				{
					downloadHandler.Dispose();
				}
			}
			if (this.disposeUploadHandlerOnDispose)
			{
				UploadHandler uploadHandler = this.uploadHandler;
				if (uploadHandler != null)
				{
					uploadHandler.Dispose();
				}
			}
			this.InternalDestroy();
			GC.SuppressFinalize(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern AsyncOperation InternalBegin();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalAbort();

		public AsyncOperation Send()
		{
			return this.InternalBegin();
		}

		public void Abort()
		{
			this.InternalAbort();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalSetMethod(UnityWebRequest.UnityWebRequestMethod methodType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalSetCustomMethod(string customMethodName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int InternalGetMethod();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string InternalGetCustomMethod();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int InternalGetError();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string InternalGetUrl();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetUrl(string url);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetRequestHeader(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalSetRequestHeader(string name, string value);

		public void SetRequestHeader(string name, string value)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Cannot set a Request Header with a null or empty name");
			}
			if (value == null)
			{
				throw new ArgumentException("Cannot set a Request header with a null");
			}
			if (!UnityWebRequest.IsHeaderNameLegal(name))
			{
				throw new ArgumentException("Cannot set Request Header " + name + " - name contains illegal characters or is not user-overridable");
			}
			if (!UnityWebRequest.IsHeaderValueLegal(value))
			{
				throw new ArgumentException("Cannot set Request Header - value contains illegal characters");
			}
			this.InternalSetRequestHeader(name, value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetResponseHeader(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string[] InternalGetResponseHeaderKeys();

		public Dictionary<string, string> GetResponseHeaders()
		{
			string[] array = this.InternalGetResponseHeaderKeys();
			Dictionary<string, string> result;
			if (array == null)
			{
				result = null;
			}
			else
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>(array.Length, StringComparer.OrdinalIgnoreCase);
				for (int i = 0; i < array.Length; i++)
				{
					string responseHeader = this.GetResponseHeader(array[i]);
					dictionary.Add(array[i], responseHeader);
				}
				result = dictionary;
			}
			return result;
		}

		private static bool ContainsForbiddenCharacters(string s, int firstAllowedCharCode)
		{
			bool result;
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if ((int)c < firstAllowedCharCode || c == '\u007f')
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private static bool IsHeaderNameLegal(string headerName)
		{
			bool result;
			if (string.IsNullOrEmpty(headerName))
			{
				result = false;
			}
			else
			{
				headerName = headerName.ToLower();
				if (UnityWebRequest.ContainsForbiddenCharacters(headerName, 33))
				{
					result = false;
				}
				else if (headerName.StartsWith("sec-") || headerName.StartsWith("proxy-"))
				{
					result = false;
				}
				else
				{
					string[] array = UnityWebRequest.forbiddenHeaderKeys;
					for (int i = 0; i < array.Length; i++)
					{
						string b = array[i];
						if (string.Equals(headerName, b))
						{
							result = false;
							return result;
						}
					}
					result = true;
				}
			}
			return result;
		}

		private static bool IsHeaderValueLegal(string headerValue)
		{
			return !UnityWebRequest.ContainsForbiddenCharacters(headerValue, 32);
		}

		private static string GetErrorDescription(UnityWebRequest.UnityWebRequestError errorCode)
		{
			string result;
			switch (errorCode)
			{
			case UnityWebRequest.UnityWebRequestError.OK:
				result = "No Error";
				return result;
			case UnityWebRequest.UnityWebRequestError.SDKError:
				result = "Internal Error With Transport Layer";
				return result;
			case UnityWebRequest.UnityWebRequestError.UnsupportedProtocol:
				result = "Specified Transport Protocol is Unsupported";
				return result;
			case UnityWebRequest.UnityWebRequestError.MalformattedUrl:
				result = "URL is Malformatted";
				return result;
			case UnityWebRequest.UnityWebRequestError.CannotResolveProxy:
				result = "Unable to resolve specified proxy server";
				return result;
			case UnityWebRequest.UnityWebRequestError.CannotResolveHost:
				result = "Unable to resolve host specified in URL";
				return result;
			case UnityWebRequest.UnityWebRequestError.CannotConnectToHost:
				result = "Unable to connect to host specified in URL";
				return result;
			case UnityWebRequest.UnityWebRequestError.AccessDenied:
				result = "Remote server denied access to the specified URL";
				return result;
			case UnityWebRequest.UnityWebRequestError.GenericHTTPError:
				result = "Unknown/Generic HTTP Error - Check HTTP Error code";
				return result;
			case UnityWebRequest.UnityWebRequestError.WriteError:
				result = "Error when transmitting request to remote server - transmission terminated prematurely";
				return result;
			case UnityWebRequest.UnityWebRequestError.ReadError:
				result = "Error when reading response from remote server - transmission terminated prematurely";
				return result;
			case UnityWebRequest.UnityWebRequestError.OutOfMemory:
				result = "Out of Memory";
				return result;
			case UnityWebRequest.UnityWebRequestError.Timeout:
				result = "Timeout occurred while waiting for response from remote server";
				return result;
			case UnityWebRequest.UnityWebRequestError.HTTPPostError:
				result = "Error while transmitting HTTP POST body data";
				return result;
			case UnityWebRequest.UnityWebRequestError.SSLCannotConnect:
				result = "Unable to connect to SSL server at remote host";
				return result;
			case UnityWebRequest.UnityWebRequestError.Aborted:
				result = "Request was manually aborted by local code";
				return result;
			case UnityWebRequest.UnityWebRequestError.TooManyRedirects:
				result = "Redirect limit exceeded";
				return result;
			case UnityWebRequest.UnityWebRequestError.ReceivedNoData:
				result = "Received an empty response from remote host";
				return result;
			case UnityWebRequest.UnityWebRequestError.SSLNotSupported:
				result = "SSL connections are not supported on the local machine";
				return result;
			case UnityWebRequest.UnityWebRequestError.FailedToSendData:
				result = "Failed to transmit body data";
				return result;
			case UnityWebRequest.UnityWebRequestError.FailedToReceiveData:
				result = "Failed to receive response body data";
				return result;
			case UnityWebRequest.UnityWebRequestError.SSLCertificateError:
				result = "Failure to authenticate SSL certificate of remote host";
				return result;
			case UnityWebRequest.UnityWebRequestError.SSLCipherNotAvailable:
				result = "SSL cipher received from remote host is not supported on the local machine";
				return result;
			case UnityWebRequest.UnityWebRequestError.SSLCACertError:
				result = "Failure to authenticate Certificate Authority of the SSL certificate received from the remote host";
				return result;
			case UnityWebRequest.UnityWebRequestError.UnrecognizedContentEncoding:
				result = "Remote host returned data with an unrecognized/unparseable content encoding";
				return result;
			case UnityWebRequest.UnityWebRequestError.LoginFailed:
				result = "HTTP authentication failed";
				return result;
			case UnityWebRequest.UnityWebRequestError.SSLShutdownFailed:
				result = "Failure while shutting down SSL connection";
				return result;
			}
			result = "Unknown error";
			return result;
		}
	}
}
