using Mono.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Security
	{
		private const string publicVerificationKey = "<RSAKeyValue><Modulus>uP7lsvrE6fNoQWhUIdJnQrgKoGXBkgWgs5l1xmS9gfyNkFSXgugIpfmN/0YrtL57PezYFXN0CogAnOpOtcUmpcIrh524VL/7bIh+jDUaOCG292PIx92dtzqCTvbUdCYUmaag9VlrdAw05FxYQJi2iZ/X6EiuO1TnqpVNFCDb6pXPAssoO4Uxn9JXBzL0muNRdcmFGRiLp7JQOL7a2aeU9mF9qjMprnww0k8COa6tHdnNWJqaxdFO+Etk3os0ns/gQ2FWrztKemM1Wfu7lk/B1F+V2g0adwlTiuyNHw6to+5VQXWK775RXB9wAGr8KhsVD5IJvmxrdBT8KVEWve+OXQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

		private static List<Assembly> _verifiedAssemblies = new List<Assembly>();

		private static readonly string kSignatureExtension = ".signature";

		[ExcludeFromDocs]
		public static bool PrefetchSocketPolicy(string ip, int atPort)
		{
			int timeout = 3000;
			return Security.PrefetchSocketPolicy(ip, atPort, timeout);
		}

		public static bool PrefetchSocketPolicy(string ip, int atPort, [DefaultValue("3000")] int timeout)
		{
			MethodInfo unityCrossDomainHelperMethod = Security.GetUnityCrossDomainHelperMethod("PrefetchSocketPolicy");
			object obj = unityCrossDomainHelperMethod.Invoke(null, new object[]
			{
				ip,
				atPort,
				timeout
			});
			return (bool)obj;
		}

		[SecuritySafeCritical]
		public static string GetChainOfTrustValue(string name)
		{
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			if (!Security._verifiedAssemblies.Contains(callingAssembly))
			{
				throw new ArgumentException("Calling assembly needs to be verified by Security.LoadAndVerifyAssembly");
			}
			byte[] publicKeyToken = callingAssembly.GetName().GetPublicKeyToken();
			return Security.GetChainOfTrustValueInternal(name, Security.TokenToHex(publicKeyToken));
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetChainOfTrustValueInternal(string name, string publicKeyToken);

		private static MethodInfo GetUnityCrossDomainHelperMethod(string methodname)
		{
			Type type = Types.GetType("UnityEngine.UnityCrossDomainHelper", "CrossDomainPolicyParser, Version=1.0.0.0, Culture=neutral");
			if (type == null)
			{
				throw new SecurityException("Cant find type UnityCrossDomainHelper");
			}
			MethodInfo method = type.GetMethod(methodname);
			if (method == null)
			{
				throw new SecurityException("Cant find " + methodname);
			}
			return method;
		}

		internal static string TokenToHex(byte[] token)
		{
			if (token == null || 8 > token.Length)
			{
				return string.Empty;
			}
			return string.Format("{0:x2}{1:x2}{2:x2}{3:x2}{4:x2}{5:x2}{6:x2}{7:x2}", new object[]
			{
				token[0],
				token[1],
				token[2],
				token[3],
				token[4],
				token[5],
				token[6],
				token[7]
			});
		}

		internal static void ClearVerifiedAssemblies()
		{
			Security._verifiedAssemblies.Clear();
		}

		[SecuritySafeCritical]
		public static Assembly LoadAndVerifyAssembly(byte[] assemblyData, string authorizationKey)
		{
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.FromXmlString("<RSAKeyValue><Modulus>uP7lsvrE6fNoQWhUIdJnQrgKoGXBkgWgs5l1xmS9gfyNkFSXgugIpfmN/0YrtL57PezYFXN0CogAnOpOtcUmpcIrh524VL/7bIh+jDUaOCG292PIx92dtzqCTvbUdCYUmaag9VlrdAw05FxYQJi2iZ/X6EiuO1TnqpVNFCDb6pXPAssoO4Uxn9JXBzL0muNRdcmFGRiLp7JQOL7a2aeU9mF9qjMprnww0k8COa6tHdnNWJqaxdFO+Etk3os0ns/gQ2FWrztKemM1Wfu7lk/B1F+V2g0adwlTiuyNHw6to+5VQXWK775RXB9wAGr8KhsVD5IJvmxrdBT8KVEWve+OXQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
			bool flag = false;
			SHA1 sHA = SHA1.Create();
			byte[] rgbHash = sHA.ComputeHash(assemblyData);
			byte[] rgbSignature = Convert.FromBase64String(authorizationKey);
			try
			{
				flag = rSACryptoServiceProvider.VerifyHash(rgbHash, CryptoConfig.MapNameToOID("SHA1"), rgbSignature);
			}
			catch (CryptographicException)
			{
				Debug.LogError("Unable to verify that this assembly has been authorized by Unity.  The assembly will not be loaded.");
				flag = false;
			}
			if (!flag)
			{
				return null;
			}
			return Security.LoadAndVerifyAssemblyInternal(assemblyData);
		}

		[SecuritySafeCritical]
		public static Assembly LoadAndVerifyAssembly(byte[] assemblyData)
		{
			Debug.LogError("Unable to verify assembly data; you must provide an authorization key when loading this assembly.");
			return null;
		}

		[SecuritySafeCritical]
		private static Assembly LoadAndVerifyAssemblyInternal(byte[] assemblyData)
		{
			Assembly assembly = Assembly.Load(assemblyData);
			byte[] publicKey = assembly.GetName().GetPublicKey();
			if (publicKey == null || publicKey.Length == 0)
			{
				return null;
			}
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.ImportCspBlob(publicKey);
			StrongName strongName = new StrongName(rSACryptoServiceProvider);
			Assembly result;
			using (MemoryStream memoryStream = new MemoryStream(assemblyData))
			{
				if (strongName.Verify(memoryStream))
				{
					Security._verifiedAssemblies.Add(assembly);
					result = assembly;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		internal static bool VerifySignature(string file, byte[] publicKey)
		{
			try
			{
				string path = file + Security.kSignatureExtension;
				if (!File.Exists(path))
				{
					bool result = false;
					return result;
				}
				using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider())
				{
					rSACryptoServiceProvider.ImportCspBlob(publicKey);
					using (SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider())
					{
						bool result = rSACryptoServiceProvider.VerifyData(File.ReadAllBytes(file), sHA1CryptoServiceProvider, File.ReadAllBytes(path));
						return result;
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return false;
		}
	}
}
