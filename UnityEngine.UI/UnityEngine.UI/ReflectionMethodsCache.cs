using System;
using System.Reflection;
using UnityEngineInternal;

namespace UnityEngine.UI
{
	internal class ReflectionMethodsCache
	{
		public delegate bool Raycast3DCallback(Ray r, out RaycastHit hit, float f, int i);

		public delegate RaycastHit2D Raycast2DCallback(Vector2 p1, Vector2 p2, float f, int i);

		public delegate RaycastHit[] RaycastAllCallback(Ray r, float f, int i);

		public delegate RaycastHit2D[] GetRayIntersectionAllCallback(Ray r, float f, int i);

		public ReflectionMethodsCache.Raycast3DCallback raycast3D = null;

		public ReflectionMethodsCache.RaycastAllCallback raycast3DAll = null;

		public ReflectionMethodsCache.Raycast2DCallback raycast2D = null;

		public ReflectionMethodsCache.GetRayIntersectionAllCallback getRayIntersectionAll = null;

		private static ReflectionMethodsCache s_ReflectionMethodsCache = null;

		public static ReflectionMethodsCache Singleton
		{
			get
			{
				if (ReflectionMethodsCache.s_ReflectionMethodsCache == null)
				{
					ReflectionMethodsCache.s_ReflectionMethodsCache = new ReflectionMethodsCache();
				}
				return ReflectionMethodsCache.s_ReflectionMethodsCache;
			}
		}

		public ReflectionMethodsCache()
		{
			MethodInfo method = typeof(Physics).GetMethod("Raycast", new Type[]
			{
				typeof(Ray),
				typeof(RaycastHit).MakeByRefType(),
				typeof(float),
				typeof(int)
			});
			if (method != null)
			{
				this.raycast3D = (ReflectionMethodsCache.Raycast3DCallback)ScriptingUtils.CreateDelegate(typeof(ReflectionMethodsCache.Raycast3DCallback), method);
			}
			MethodInfo method2 = typeof(Physics2D).GetMethod("Raycast", new Type[]
			{
				typeof(Vector2),
				typeof(Vector2),
				typeof(float),
				typeof(int)
			});
			if (method2 != null)
			{
				this.raycast2D = (ReflectionMethodsCache.Raycast2DCallback)ScriptingUtils.CreateDelegate(typeof(ReflectionMethodsCache.Raycast2DCallback), method2);
			}
			MethodInfo method3 = typeof(Physics).GetMethod("RaycastAll", new Type[]
			{
				typeof(Ray),
				typeof(float),
				typeof(int)
			});
			if (method3 != null)
			{
				this.raycast3DAll = (ReflectionMethodsCache.RaycastAllCallback)ScriptingUtils.CreateDelegate(typeof(ReflectionMethodsCache.RaycastAllCallback), method3);
			}
			MethodInfo method4 = typeof(Physics2D).GetMethod("GetRayIntersectionAll", new Type[]
			{
				typeof(Ray),
				typeof(float),
				typeof(int)
			});
			if (method4 != null)
			{
				this.getRayIntersectionAll = (ReflectionMethodsCache.GetRayIntersectionAllCallback)ScriptingUtils.CreateDelegate(typeof(ReflectionMethodsCache.GetRayIntersectionAllCallback), method4);
			}
		}
	}
}
