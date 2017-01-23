using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public class AssetImporter : UnityEngine.Object
	{
		public extern string assetPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ulong assetTimeStamp
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string userData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string assetBundleName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string assetBundleVariant
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAssetBundleNameAndVariant(string assetBundleName, string assetBundleVariant);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AssetImporter GetAtPath(string path);

		public void SaveAndReimport()
		{
			AssetDatabase.ImportAsset(this.assetPath);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int LocalFileIDToClassID(long fileId);
	}
}
