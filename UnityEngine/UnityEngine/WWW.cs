using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public sealed class WWW : IDisposable
	{
		[RequiredByNativeCode]
		internal IntPtr m_Ptr;

		private static readonly char[] forbiddenCharacters = new char[]
		{
			'\0',
			'\u0001',
			'\u0002',
			'\u0003',
			'\u0004',
			'\u0005',
			'\u0006',
			'\a',
			'\b',
			'\t',
			'\n',
			'\v',
			'\f',
			'\r',
			'\u000e',
			'\u000f',
			'\u0010',
			'\u0011',
			'\u0012',
			'\u0013',
			'\u0014',
			'\u0015',
			'\u0016',
			'\u0017',
			'\u0018',
			'\u0019',
			'\u001a',
			'\u001b',
			'\u001c',
			'\u001d',
			'\u001e',
			'\u001f',
			'\u007f'
		};

		private static readonly char[] forbiddenCharactersForNames = new char[]
		{
			' '
		};

		private static readonly string[] forbiddenHeaderKeys = new string[]
		{
			"Accept-Charset",
			"Accept-Encoding",
			"Access-Control-Request-Headers",
			"Access-Control-Request-Method",
			"Connection",
			"Content-Length",
			"Cookie",
			"Cookie2",
			"Date",
			"DNT",
			"Expect",
			"Host",
			"Keep-Alive",
			"Origin",
			"Referer",
			"TE",
			"Trailer",
			"Transfer-Encoding",
			"Upgrade",
			"User-Agent",
			"Via",
			"X-Unity-Version"
		};

		public Dictionary<string, string> responseHeaders
		{
			get
			{
				if (!this.isDone)
				{
					throw new UnityException("WWW is not finished downloading yet");
				}
				return WWW.ParseHTTPHeaderString(this.responseHeadersString);
			}
		}

		private extern string responseHeadersString
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public string text
		{
			get
			{
				if (!this.isDone)
				{
					throw new UnityException("WWW is not ready downloading yet");
				}
				byte[] bytes = this.bytes;
				return this.GetTextEncoder().GetString(bytes, 0, bytes.Length);
			}
		}

		internal static Encoding DefaultEncoding
		{
			get
			{
				return Encoding.ASCII;
			}
		}

		[Obsolete("Please use WWW.text instead")]
		public string data
		{
			get
			{
				return this.text;
			}
		}

		public extern byte[] bytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int size
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string error
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Texture2D texture
		{
			get
			{
				return this.GetTexture(false);
			}
		}

		public Texture2D textureNonReadable
		{
			get
			{
				return this.GetTexture(true);
			}
		}

		public AudioClip audioClip
		{
			get
			{
				return this.GetAudioClip(true);
			}
		}

		public extern MovieTexture movie
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isDone
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float progress
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float uploadProgress
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int bytesDownloaded
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property WWW.oggVorbis has been deprecated. Use WWW.audioClip instead (UnityUpgradable).", true)]
		public AudioClip oggVorbis
		{
			get
			{
				return null;
			}
		}

		public extern string url
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern AssetBundle assetBundle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ThreadPriority threadPriority
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public WWW(string url)
		{
			this.InitWWW(url, null, null);
		}

		public WWW(string url, WWWForm form)
		{
			string[] array = WWW.FlattenedHeadersFrom(form.headers);
			if (this.enforceWebSecurityRestrictions())
			{
				WWW.CheckSecurityOnHeaders(array);
			}
			this.InitWWW(url, form.data, array);
		}

		public WWW(string url, byte[] postData)
		{
			this.InitWWW(url, postData, null);
		}

		public WWW(string url, byte[] postData, Dictionary<string, string> headers)
		{
			string[] array = WWW.FlattenedHeadersFrom(headers);
			if (this.enforceWebSecurityRestrictions())
			{
				WWW.CheckSecurityOnHeaders(array);
			}
			this.InitWWW(url, postData, array);
		}

		internal WWW(string url, Hash128 hash, uint crc)
		{
			WWW.INTERNAL_CALL_WWW(this, url, ref hash, crc);
		}

		public void Dispose()
		{
			this.DestroyWWW(true);
		}

		~WWW()
		{
			this.DestroyWWW(false);
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DestroyWWW(bool cancel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitWWW(string url, byte[] postData, string[] iHeaders);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool enforceWebSecurityRestrictions();

		[ExcludeFromDocs]
		public static string EscapeURL(string s)
		{
			Encoding uTF = Encoding.UTF8;
			return WWW.EscapeURL(s, uTF);
		}

		public static string EscapeURL(string s, [UnityEngine.Internal.DefaultValue("System.Text.Encoding.UTF8")] Encoding e)
		{
			string result;
			if (s == null)
			{
				result = null;
			}
			else if (s == "")
			{
				result = "";
			}
			else if (e == null)
			{
				result = null;
			}
			else
			{
				result = WWWTranscoder.URLEncode(s, e);
			}
			return result;
		}

		[ExcludeFromDocs]
		public static string UnEscapeURL(string s)
		{
			Encoding uTF = Encoding.UTF8;
			return WWW.UnEscapeURL(s, uTF);
		}

		public static string UnEscapeURL(string s, [UnityEngine.Internal.DefaultValue("System.Text.Encoding.UTF8")] Encoding e)
		{
			string result;
			if (s == null)
			{
				result = null;
			}
			else if (s.IndexOf('%') == -1 && s.IndexOf('+') == -1)
			{
				result = s;
			}
			else
			{
				result = WWWTranscoder.URLDecode(s, e);
			}
			return result;
		}

		private Encoding GetTextEncoder()
		{
			string text = null;
			Encoding result;
			if (this.responseHeaders.TryGetValue("CONTENT-TYPE", out text))
			{
				int num = text.IndexOf("charset", StringComparison.OrdinalIgnoreCase);
				if (num > -1)
				{
					int num2 = text.IndexOf('=', num);
					if (num2 > -1)
					{
						string text2 = text.Substring(num2 + 1).Trim().Trim(new char[]
						{
							'\'',
							'"'
						}).Trim();
						int num3 = text2.IndexOf(';');
						if (num3 > -1)
						{
							text2 = text2.Substring(0, num3);
						}
						try
						{
							result = Encoding.GetEncoding(text2);
							return result;
						}
						catch (Exception)
						{
							Debug.Log("Unsupported encoding: '" + text2 + "'");
						}
					}
				}
			}
			result = Encoding.UTF8;
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Texture2D GetTexture(bool markNonReadable);

		public AudioClip GetAudioClip(bool threeD)
		{
			return this.GetAudioClip(threeD, false);
		}

		public AudioClip GetAudioClip(bool threeD, bool stream)
		{
			return this.GetAudioClip(threeD, stream, AudioType.UNKNOWN);
		}

		public AudioClip GetAudioClip(bool threeD, bool stream, AudioType audioType)
		{
			return this.GetAudioClipInternal(threeD, stream, false, audioType);
		}

		public AudioClip GetAudioClipCompressed()
		{
			return this.GetAudioClipCompressed(true);
		}

		public AudioClip GetAudioClipCompressed(bool threeD)
		{
			return this.GetAudioClipCompressed(threeD, AudioType.UNKNOWN);
		}

		public AudioClip GetAudioClipCompressed(bool threeD, AudioType audioType)
		{
			return this.GetAudioClipInternal(threeD, false, true, audioType);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AudioClip GetAudioClipInternal(bool threeD, bool stream, bool compressed, AudioType audioType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void LoadImageIntoTexture(Texture2D tex);

		[Obsolete("All blocking WWW functions have been deprecated, please use one of the asynchronous functions instead.", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetURL(string url);

		[Obsolete("All blocking WWW functions have been deprecated, please use one of the asynchronous functions instead.", true)]
		public static Texture2D GetTextureFromURL(string url)
		{
			return new WWW(url).texture;
		}

		[Obsolete("LoadUnityWeb is no longer supported. Please use javascript to reload the web player on a different url instead", true)]
		public void LoadUnityWeb()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_WWW(WWW self, string url, ref Hash128 hash, uint crc);

		[ExcludeFromDocs]
		public static WWW LoadFromCacheOrDownload(string url, int version)
		{
			uint crc = 0u;
			return WWW.LoadFromCacheOrDownload(url, version, crc);
		}

		public static WWW LoadFromCacheOrDownload(string url, int version, [UnityEngine.Internal.DefaultValue("0")] uint crc)
		{
			Hash128 hash = new Hash128(0u, 0u, 0u, (uint)version);
			return WWW.LoadFromCacheOrDownload(url, hash, crc);
		}

		[ExcludeFromDocs]
		public static WWW LoadFromCacheOrDownload(string url, Hash128 hash)
		{
			uint crc = 0u;
			return WWW.LoadFromCacheOrDownload(url, hash, crc);
		}

		public static WWW LoadFromCacheOrDownload(string url, Hash128 hash, [UnityEngine.Internal.DefaultValue("0")] uint crc)
		{
			return new WWW(url, hash, crc);
		}

		private static void CheckSecurityOnHeaders(string[] headers)
		{
			for (int i = 0; i < headers.Length; i += 2)
			{
				string[] array = WWW.forbiddenHeaderKeys;
				for (int j = 0; j < array.Length; j++)
				{
					string b = array[j];
					if (string.Equals(headers[i], b, StringComparison.CurrentCultureIgnoreCase))
					{
						throw new ArgumentException("Cannot overwrite header: " + headers[i]);
					}
				}
				if (headers[i].StartsWith("Sec-") || headers[i].StartsWith("Proxy-"))
				{
					throw new ArgumentException("Cannot overwrite header: " + headers[i]);
				}
				if (headers[i].IndexOfAny(WWW.forbiddenCharacters) > -1 || headers[i].IndexOfAny(WWW.forbiddenCharactersForNames) > -1 || headers[i + 1].IndexOfAny(WWW.forbiddenCharacters) > -1)
				{
					throw new ArgumentException("Cannot include control characters in a HTTP header, either as key or value.");
				}
			}
		}

		private static string[] FlattenedHeadersFrom(Dictionary<string, string> headers)
		{
			string[] result;
			if (headers == null)
			{
				result = null;
			}
			else
			{
				string[] array = new string[headers.Count * 2];
				int num = 0;
				foreach (KeyValuePair<string, string> current in headers)
				{
					array[num++] = current.Key.ToString();
					array[num++] = current.Value.ToString();
				}
				result = array;
			}
			return result;
		}

		internal static Dictionary<string, string> ParseHTTPHeaderString(string input)
		{
			if (input == null)
			{
				throw new ArgumentException("input was null to ParseHTTPHeaderString");
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			StringReader stringReader = new StringReader(input);
			int num = 0;
			while (true)
			{
				string text = stringReader.ReadLine();
				if (text == null)
				{
					break;
				}
				if (num++ == 0 && text.StartsWith("HTTP"))
				{
					dictionary["STATUS"] = text;
				}
				else
				{
					int num2 = text.IndexOf(": ");
					if (num2 != -1)
					{
						string key = text.Substring(0, num2).ToUpper();
						string text2 = text.Substring(num2 + 2);
						string str;
						if (dictionary.TryGetValue(key, out str))
						{
							text2 = str + "," + text2;
						}
						dictionary[key] = text2;
					}
				}
			}
			return dictionary;
		}
	}
}
