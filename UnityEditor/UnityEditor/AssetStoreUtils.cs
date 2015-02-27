using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Internal;
namespace UnityEditor
{
	internal sealed class AssetStoreUtils
	{
		public delegate void DownloadDoneCallback(string package_id, string message, int bytes, int total);
		private const string kAssetStoreUrl = "https://shawarma.unity3d.com";
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Download(string id, string url, string[] destination, string key, string jsonData, bool resumeOK, [DefaultValue("null")] AssetStoreUtils.DownloadDoneCallback doneCallback);
		[ExcludeFromDocs]
		public static void Download(string id, string url, string[] destination, string key, string jsonData, bool resumeOK)
		{
			AssetStoreUtils.DownloadDoneCallback doneCallback = null;
			AssetStoreUtils.Download(id, url, destination, key, jsonData, resumeOK, doneCallback);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string CheckDownload(string id, string url, string[] destination, string key);
		private static void HexStringToByteArray(string hex, byte[] array, int offset)
		{
			if (offset + array.Length * 2 > hex.Length)
			{
				throw new ArgumentException("Hex string too short");
			}
			for (int i = 0; i < array.Length; i++)
			{
				string s = hex.Substring(i * 2 + offset, 2);
				array[i] = byte.Parse(s, NumberStyles.HexNumber);
			}
		}
		public static void DecryptFile(string inputFile, string outputFile, string keyIV)
		{
			byte[] array = new byte[32];
			byte[] array2 = new byte[16];
			AssetStoreUtils.HexStringToByteArray(keyIV, array, 0);
			AssetStoreUtils.HexStringToByteArray(keyIV, array2, 64);
			FileStream fileStream = File.Open(inputFile, FileMode.Open);
			FileStream fileStream2 = File.Open(outputFile, FileMode.CreateNew);
			AesManaged aesManaged = new AesManaged();
			aesManaged.Key = array;
			aesManaged.IV = array2;
			CryptoStream cryptoStream = new CryptoStream(fileStream, aesManaged.CreateDecryptor(aesManaged.Key, aesManaged.IV), CryptoStreamMode.Read);
			try
			{
				byte[] array3 = new byte[40960];
				int count;
				while ((count = cryptoStream.Read(array3, 0, array3.Length)) > 0)
				{
					fileStream2.Write(array3, 0, count);
				}
			}
			finally
			{
				cryptoStream.Close();
				fileStream.Close();
				fileStream2.Close();
			}
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RegisterDownloadDelegate(ScriptableObject d);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnRegisterDownloadDelegate(ScriptableObject d);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetLoaderPath();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UpdatePreloading();
		public static string GetOfflinePath()
		{
			return EditorApplication.applicationContentsPath + "/Resources/offline.html";
		}
		public static string GetAssetStoreUrl()
		{
			return "https://shawarma.unity3d.com";
		}
		public static string GetAssetStoreSearchUrl()
		{
			return AssetStoreUtils.GetAssetStoreUrl().Replace("https", "http");
		}
	}
}
