using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement
{
	public struct Scene
	{
		internal enum LoadingState
		{
			NotLoaded,
			Loading,
			Loaded
		}

		private int m_Handle;

		internal int handle
		{
			get
			{
				return this.m_Handle;
			}
		}

		internal Scene.LoadingState loadingState
		{
			get
			{
				return Scene.GetLoadingStateInternal(this.handle);
			}
		}

		public string path
		{
			get
			{
				return Scene.GetPathInternal(this.handle);
			}
		}

		public string name
		{
			get
			{
				return Scene.GetNameInternal(this.handle);
			}
			internal set
			{
				Scene.SetNameInternal(this.handle, value);
			}
		}

		internal string guid
		{
			get
			{
				return Scene.GetGUIDInternal(this.handle);
			}
		}

		public bool isLoaded
		{
			get
			{
				return Scene.GetIsLoadedInternal(this.handle);
			}
		}

		public int buildIndex
		{
			get
			{
				return Scene.GetBuildIndexInternal(this.handle);
			}
		}

		public bool isDirty
		{
			get
			{
				return Scene.GetIsDirtyInternal(this.handle);
			}
		}

		public int rootCount
		{
			get
			{
				return Scene.GetRootCountInternal(this.handle);
			}
		}

		public bool IsValid()
		{
			return Scene.IsValidInternal(this.handle);
		}

		public GameObject[] GetRootGameObjects()
		{
			List<GameObject> list = new List<GameObject>(this.rootCount);
			this.GetRootGameObjects(list);
			return list.ToArray();
		}

		public void GetRootGameObjects(List<GameObject> rootGameObjects)
		{
			if (rootGameObjects.Capacity < this.rootCount)
			{
				rootGameObjects.Capacity = this.rootCount;
			}
			rootGameObjects.Clear();
			if (!this.IsValid())
			{
				throw new ArgumentException("The scene is invalid.");
			}
			if (!Application.isPlaying && !this.isLoaded)
			{
				throw new ArgumentException("The scene is not loaded.");
			}
			if (this.rootCount != 0)
			{
				Scene.GetRootGameObjectsInternal(this.handle, rootGameObjects);
			}
		}

		public static bool operator ==(Scene lhs, Scene rhs)
		{
			return lhs.handle == rhs.handle;
		}

		public static bool operator !=(Scene lhs, Scene rhs)
		{
			return lhs.handle != rhs.handle;
		}

		public override int GetHashCode()
		{
			return this.m_Handle;
		}

		public override bool Equals(object other)
		{
			bool result;
			if (!(other is Scene))
			{
				result = false;
			}
			else
			{
				Scene scene = (Scene)other;
				result = (this.handle == scene.handle);
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValidInternal(int sceneHandle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetPathInternal(int sceneHandle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetNameInternal(int sceneHandle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetNameInternal(int sceneHandle, string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetGUIDInternal(int sceneHandle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIsLoadedInternal(int sceneHandle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Scene.LoadingState GetLoadingStateInternal(int sceneHandle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIsDirtyInternal(int sceneHandle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetBuildIndexInternal(int sceneHandle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRootCountInternal(int sceneHandle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRootGameObjectsInternal(int sceneHandle, object resultRootList);
	}
}
