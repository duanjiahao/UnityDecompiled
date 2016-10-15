using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEngineInternal
{
	public sealed class APIUpdaterRuntimeServices
	{
		private static IList<Type> ComponentsFromUnityEngine;

		static APIUpdaterRuntimeServices()
		{
			Type typeFromHandle = typeof(Component);
			APIUpdaterRuntimeServices.ComponentsFromUnityEngine = typeFromHandle.Assembly.GetTypes().Where(new Func<Type, bool>(typeFromHandle.IsAssignableFrom)).ToList<Type>();
		}

		[Obsolete("AddComponent(string) has been deprecated. Use GameObject.AddComponent<T>() / GameObject.AddComponent(Type) instead.\nAPI Updater could not automatically update the original call to AddComponent(string name), because it was unable to resolve the type specified in parameter 'name'.\nInstead, this call has been replaced with a call to APIUpdaterRuntimeServices.AddComponent() so you can try to test your game in the editor.\nIn order to be able to build the game, replace this call (APIUpdaterRuntimeServices.AddComponent()) with a call to GameObject.AddComponent<T>() / GameObject.AddComponent(Type).")]
		public static Component AddComponent(GameObject go, string sourceInfo, string name)
		{
			Debug.LogWarningFormat("Performing a potentially slow search for component {0}.", new object[]
			{
				name
			});
			Type type = APIUpdaterRuntimeServices.ResolveType(name, Assembly.GetCallingAssembly(), sourceInfo);
			return (type != null) ? go.AddComponent(type) : null;
		}

		private static Type ResolveType(string name, Assembly callingAssembly, string sourceInfo)
		{
			Type type = APIUpdaterRuntimeServices.ComponentsFromUnityEngine.FirstOrDefault((Type t) => (t.Name == name || t.FullName == name) && !APIUpdaterRuntimeServices.IsMarkedAsObsolete(t));
			if (type != null)
			{
				Debug.LogWarningFormat("[{1}] Type '{0}' found in UnityEngine, consider replacing with go.AddComponent<{0}>();", new object[]
				{
					name,
					sourceInfo
				});
				return type;
			}
			Type type2 = callingAssembly.GetType(name);
			if (type2 != null)
			{
				Debug.LogWarningFormat("[{1}] Component type '{0}' found on caller assembly. Consider replacing the call method call with: AddComponent<{0}>()", new object[]
				{
					type2.FullName,
					sourceInfo
				});
				return type2;
			}
			type2 = AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly a) => a.GetTypes()).SingleOrDefault((Type t) => t.Name == name && typeof(Component).IsAssignableFrom(t));
			if (type2 != null)
			{
				Debug.LogWarningFormat("[{2}] Component type '{0}' found on assembly {1}. Consider replacing the call method with: AddComponent<{0}>()", new object[]
				{
					type2.FullName,
					type2.Assembly.Location,
					sourceInfo
				});
				return type2;
			}
			Debug.LogErrorFormat("[{1}] Component Type '{0}' not found.", new object[]
			{
				name,
				sourceInfo
			});
			return null;
		}

		private static bool IsMarkedAsObsolete(Type t)
		{
			return t.GetCustomAttributes(typeof(ObsoleteAttribute), false).Any<object>();
		}
	}
}
