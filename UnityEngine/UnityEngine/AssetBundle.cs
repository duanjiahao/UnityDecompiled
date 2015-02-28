using System;
using System.Runtime.CompilerServices;
using UnityEngineInternal;
namespace UnityEngine
{
	public sealed class AssetBundle : Object
	{
		public extern Object mainAsset
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AssetBundleCreateRequest CreateFromMemory(byte[] binary);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AssetBundle CreateFromMemoryImmediate(byte[] binary);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AssetBundle CreateFromFile(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Contains(string name);
		[Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
		public Object Load(string name)
		{
			return null;
		}
		[Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
		public T Load<T>(string name) where T : Object
		{
			return (T)((object)null);
		}
		[Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true), WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Object Load(string name, Type type);
		[Obsolete("Method LoadAsync has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAssetAsync instead and check the documentation for details.", true), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AssetBundleRequest LoadAsync(string name, Type type);
		[Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Object[] LoadAll(Type type);
		[Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
		public Object[] LoadAll()
		{
			return null;
		}
		[Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
		public T[] LoadAll<T>() where T : Object
		{
			return null;
		}
		public Object LoadAsset(string name)
		{
			return this.LoadAsset(name, typeof(Object));
		}
		public T LoadAsset<T>(string name) where T : Object
		{
			return (T)((object)this.LoadAsset(name, typeof(T)));
		}
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		public Object LoadAsset(string name, Type type)
		{
			if (name == null)
			{
				throw new NullReferenceException("The input asset name cannot be null.");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("The input asset name cannot be empty.");
			}
			if (type == null)
			{
				throw new NullReferenceException("The input type cannot be null.");
			}
			return this.LoadAsset_Internal(name, type);
		}
		[WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Object LoadAsset_Internal(string name, Type type);
		public AssetBundleRequest LoadAssetAsync(string name)
		{
			return this.LoadAssetAsync(name, typeof(Object));
		}
		public AssetBundleRequest LoadAssetAsync<T>(string name)
		{
			return this.LoadAssetAsync(name, typeof(T));
		}
		public AssetBundleRequest LoadAssetAsync(string name, Type type)
		{
			if (name == null)
			{
				throw new NullReferenceException("The input asset name cannot be null.");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("The input asset name cannot be empty.");
			}
			if (type == null)
			{
				throw new NullReferenceException("The input type cannot be null.");
			}
			return this.LoadAssetAsync_Internal(name, type);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AssetBundleRequest LoadAssetAsync_Internal(string name, Type type);
		public Object[] LoadAssetWithSubAssets(string name)
		{
			return this.LoadAssetWithSubAssets(name, typeof(Object));
		}
		public T[] LoadAssetWithSubAssets<T>(string name) where T : Object
		{
			return Resources.ConvertObjects<T>(this.LoadAssetWithSubAssets(name, typeof(T)));
		}
		public Object[] LoadAssetWithSubAssets(string name, Type type)
		{
			if (name == null)
			{
				throw new NullReferenceException("The input asset name cannot be null.");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("The input asset name cannot be empty.");
			}
			if (type == null)
			{
				throw new NullReferenceException("The input type cannot be null.");
			}
			return this.LoadAssetWithSubAssets_Internal(name, type);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Object[] LoadAssetWithSubAssets_Internal(string name, Type type);
		public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name)
		{
			return this.LoadAssetWithSubAssetsAsync(name, typeof(Object));
		}
		public AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string name)
		{
			return this.LoadAssetWithSubAssetsAsync(name, typeof(T));
		}
		public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name, Type type)
		{
			if (name == null)
			{
				throw new NullReferenceException("The input asset name cannot be null.");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("The input asset name cannot be empty.");
			}
			if (type == null)
			{
				throw new NullReferenceException("The input type cannot be null.");
			}
			return this.LoadAssetWithSubAssetsAsync_Internal(name, type);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AssetBundleRequest LoadAssetWithSubAssetsAsync_Internal(string name, Type type);
		public Object[] LoadAllAssets()
		{
			return this.LoadAllAssets(typeof(Object));
		}
		public T[] LoadAllAssets<T>() where T : Object
		{
			return Resources.ConvertObjects<T>(this.LoadAllAssets(typeof(T)));
		}
		public Object[] LoadAllAssets(Type type)
		{
			if (type == null)
			{
				throw new NullReferenceException("The input type cannot be null.");
			}
			return this.LoadAssetWithSubAssets_Internal(string.Empty, type);
		}
		public AssetBundleRequest LoadAllAssetsAsync()
		{
			return this.LoadAllAssetsAsync(typeof(Object));
		}
		public AssetBundleRequest LoadAllAssetsAsync<T>()
		{
			return this.LoadAllAssetsAsync(typeof(T));
		}
		public AssetBundleRequest LoadAllAssetsAsync(Type type)
		{
			if (type == null)
			{
				throw new NullReferenceException("The input type cannot be null.");
			}
			return this.LoadAssetWithSubAssetsAsync_Internal(string.Empty, type);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Unload(bool unloadAllLoadedObjects);
		[Obsolete("This method is deprecated. Use GetAllAssetNames() instead.")]
		public string[] AllAssetNames()
		{
			return this.GetAllAssetNames();
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllAssetNames();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllScenePaths();
	}
}
