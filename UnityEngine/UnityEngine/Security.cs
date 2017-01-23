using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class Security
	{
		private static readonly string kSignatureExtension = ".signature";

		[Obsolete("Security.PrefetchSocketPolicy is no longer supported, since the Unity Web Player is no longer supported by Unity."), ExcludeFromDocs]
		public static bool PrefetchSocketPolicy(string ip, int atPort)
		{
			int timeout = 3000;
			return Security.PrefetchSocketPolicy(ip, atPort, timeout);
		}

		[Obsolete("Security.PrefetchSocketPolicy is no longer supported, since the Unity Web Player is no longer supported by Unity.")]
		public static bool PrefetchSocketPolicy(string ip, int atPort, [DefaultValue("3000")] int timeout)
		{
			return false;
		}

		[RequiredByNativeCode]
		internal static bool VerifySignature(string file, byte[] publicKey)
		{
			bool result;
			try
			{
				string path = file + Security.kSignatureExtension;
				if (!File.Exists(path))
				{
					result = false;
					return result;
				}
				using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider())
				{
					rSACryptoServiceProvider.ImportCspBlob(publicKey);
					using (SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider())
					{
						result = rSACryptoServiceProvider.VerifyData(File.ReadAllBytes(file), sHA1CryptoServiceProvider, File.ReadAllBytes(path));
						return result;
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			result = false;
			return result;
		}

		[Obsolete("This was an internal method which is no longer used", true)]
		public static Assembly LoadAndVerifyAssembly(byte[] assemblyData, string authorizationKey)
		{
			return null;
		}

		[Obsolete("This was an internal method which is no longer used", true)]
		public static Assembly LoadAndVerifyAssembly(byte[] assemblyData)
		{
			return null;
		}
	}
}
