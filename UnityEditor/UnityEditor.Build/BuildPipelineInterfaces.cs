using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEditor.Build
{
	internal static class BuildPipelineInterfaces
	{
		private class AttributeCallbackWrapper : IPostprocessBuild, IProcessScene, IOrderedCallback
		{
			private int m_callbackOrder;

			private MethodInfo m_method;

			public int callbackOrder
			{
				get
				{
					return this.m_callbackOrder;
				}
			}

			public AttributeCallbackWrapper(MethodInfo m)
			{
				this.m_callbackOrder = ((CallbackOrderAttribute)Attribute.GetCustomAttribute(m, typeof(CallbackOrderAttribute))).callbackOrder;
				this.m_method = m;
			}

			public void OnPostprocessBuild(BuildTarget target, string path)
			{
				this.m_method.Invoke(null, new object[]
				{
					target,
					path
				});
			}

			public void OnProcessScene(Scene scene)
			{
				this.m_method.Invoke(null, null);
			}
		}

		private static List<IPreprocessBuild> buildPreprocessors;

		private static List<IPostprocessBuild> buildPostprocessors;

		private static List<IProcessScene> sceneProcessors;

		[CompilerGenerated]
		private static Comparison<IPreprocessBuild> <>f__mg$cache0;

		[CompilerGenerated]
		private static Comparison<IPostprocessBuild> <>f__mg$cache1;

		[CompilerGenerated]
		private static Comparison<IProcessScene> <>f__mg$cache2;

		private static int CompareICallbackOrder(IOrderedCallback a, IOrderedCallback b)
		{
			return a.callbackOrder - b.callbackOrder;
		}

		private static void AddToList<T>(object o, ref List<T> list) where T : class
		{
			T t = o as T;
			if (t != null)
			{
				if (list == null)
				{
					list = new List<T>();
				}
				list.Add(t);
			}
		}

		[RequiredByNativeCode]
		internal static void InitializeBuildCallbacks(bool findBuildProcessors, bool findSceneProcessors)
		{
			BuildPipelineInterfaces.CleanupBuildCallbacks();
			HashSet<string> hashSet = new HashSet<string>();
			hashSet.Add("UnityEditor");
			hashSet.Add("UnityEngine.UI");
			hashSet.Add("Unity.PackageManager");
			hashSet.Add("UnityEngine.Networking");
			hashSet.Add("nunit.framework");
			hashSet.Add("UnityEditor.TreeEditor");
			hashSet.Add("UnityEditor.Graphs");
			hashSet.Add("UnityEditor.UI");
			hashSet.Add("UnityEditor.TestRunner");
			hashSet.Add("UnityEngine.TestRunner");
			hashSet.Add("UnityEngine.HoloLens");
			hashSet.Add("SyntaxTree.VisualStudio.Unity.Bridge");
			hashSet.Add("UnityEditor.Android.Extensions");
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			Type[] expectedArguments = new Type[]
			{
				typeof(BuildTarget),
				typeof(string)
			};
			for (int i = 0; i < EditorAssemblies.loadedAssemblies.Length; i++)
			{
				Assembly assembly = EditorAssemblies.loadedAssemblies[i];
				bool flag = !hashSet.Contains(assembly.FullName.Substring(0, assembly.FullName.IndexOf(',')));
				Type[] array = null;
				try
				{
					array = assembly.GetTypes();
				}
				catch (ReflectionTypeLoadException ex)
				{
					array = ex.Types;
				}
				for (int j = 0; j < array.Length; j++)
				{
					Type type = array[j];
					if (type != null)
					{
						object obj = null;
						bool flag2 = false;
						if (findBuildProcessors)
						{
							flag2 = typeof(IOrderedCallback).IsAssignableFrom(type);
							if (flag2)
							{
								if (!type.IsInterface && typeof(IPreprocessBuild).IsAssignableFrom(type) && type != typeof(BuildPipelineInterfaces.AttributeCallbackWrapper))
								{
									obj = Activator.CreateInstance(type);
									BuildPipelineInterfaces.AddToList<IPreprocessBuild>(obj, ref BuildPipelineInterfaces.buildPreprocessors);
								}
								if (!type.IsInterface && typeof(IPostprocessBuild).IsAssignableFrom(type) && type != typeof(BuildPipelineInterfaces.AttributeCallbackWrapper))
								{
									obj = ((obj != null) ? obj : Activator.CreateInstance(type));
									BuildPipelineInterfaces.AddToList<IPostprocessBuild>(obj, ref BuildPipelineInterfaces.buildPostprocessors);
								}
							}
						}
						if (findSceneProcessors)
						{
							if (!findBuildProcessors || flag2)
							{
								if (!type.IsInterface && typeof(IProcessScene).IsAssignableFrom(type) && type != typeof(BuildPipelineInterfaces.AttributeCallbackWrapper))
								{
									obj = ((obj != null) ? obj : Activator.CreateInstance(type));
									BuildPipelineInterfaces.AddToList<IProcessScene>(obj, ref BuildPipelineInterfaces.sceneProcessors);
								}
							}
						}
						if (flag)
						{
							MethodInfo[] methods = type.GetMethods(bindingAttr);
							for (int k = 0; k < methods.Length; k++)
							{
								MethodInfo methodInfo = methods[k];
								if (!methodInfo.IsSpecialName)
								{
									if (findBuildProcessors && BuildPipelineInterfaces.ValidateMethod(methodInfo, typeof(PostProcessBuildAttribute), expectedArguments))
									{
										BuildPipelineInterfaces.AddToList<IPostprocessBuild>(new BuildPipelineInterfaces.AttributeCallbackWrapper(methodInfo), ref BuildPipelineInterfaces.buildPostprocessors);
									}
									if (findSceneProcessors && BuildPipelineInterfaces.ValidateMethod(methodInfo, typeof(PostProcessSceneAttribute), Type.EmptyTypes))
									{
										BuildPipelineInterfaces.AddToList<IProcessScene>(new BuildPipelineInterfaces.AttributeCallbackWrapper(methodInfo), ref BuildPipelineInterfaces.sceneProcessors);
									}
								}
							}
						}
					}
				}
			}
			if (BuildPipelineInterfaces.buildPreprocessors != null)
			{
				List<IPreprocessBuild> arg_379_0 = BuildPipelineInterfaces.buildPreprocessors;
				if (BuildPipelineInterfaces.<>f__mg$cache0 == null)
				{
					BuildPipelineInterfaces.<>f__mg$cache0 = new Comparison<IPreprocessBuild>(BuildPipelineInterfaces.CompareICallbackOrder);
				}
				arg_379_0.Sort(BuildPipelineInterfaces.<>f__mg$cache0);
			}
			if (BuildPipelineInterfaces.buildPostprocessors != null)
			{
				List<IPostprocessBuild> arg_3AA_0 = BuildPipelineInterfaces.buildPostprocessors;
				if (BuildPipelineInterfaces.<>f__mg$cache1 == null)
				{
					BuildPipelineInterfaces.<>f__mg$cache1 = new Comparison<IPostprocessBuild>(BuildPipelineInterfaces.CompareICallbackOrder);
				}
				arg_3AA_0.Sort(BuildPipelineInterfaces.<>f__mg$cache1);
			}
			if (BuildPipelineInterfaces.sceneProcessors != null)
			{
				List<IProcessScene> arg_3DB_0 = BuildPipelineInterfaces.sceneProcessors;
				if (BuildPipelineInterfaces.<>f__mg$cache2 == null)
				{
					BuildPipelineInterfaces.<>f__mg$cache2 = new Comparison<IProcessScene>(BuildPipelineInterfaces.CompareICallbackOrder);
				}
				arg_3DB_0.Sort(BuildPipelineInterfaces.<>f__mg$cache2);
			}
		}

		private static bool ValidateMethod(MethodInfo method, Type attribute, Type[] expectedArguments)
		{
			bool result;
			if (method.IsDefined(attribute, false))
			{
				if (!method.IsStatic)
				{
					string text = attribute.Name.Replace("Attribute", "");
					Debug.LogErrorFormat("Method {0} with {1} attribute must be static.", new object[]
					{
						method.Name,
						text
					});
					result = false;
				}
				else if (method.IsGenericMethod || method.IsGenericMethodDefinition)
				{
					string text2 = attribute.Name.Replace("Attribute", "");
					Debug.LogErrorFormat("Method {0} with {1} attribute cannot be generic.", new object[]
					{
						method.Name,
						text2
					});
					result = false;
				}
				else
				{
					ParameterInfo[] parameters = method.GetParameters();
					bool flag = parameters.Length == expectedArguments.Length;
					if (flag)
					{
						for (int i = 0; i < parameters.Length; i++)
						{
							if (parameters[i].ParameterType != expectedArguments[i])
							{
								flag = false;
								break;
							}
						}
					}
					if (!flag)
					{
						string text3 = attribute.Name.Replace("Attribute", "");
						string text4 = "static void " + method.Name + "(";
						for (int j = 0; j < expectedArguments.Length; j++)
						{
							text4 += expectedArguments[j].Name;
							if (j != expectedArguments.Length - 1)
							{
								text4 += ", ";
							}
						}
						text4 += ")";
						Debug.LogErrorFormat("Method {0} with {1} attribute does not have the correct signature, expected: {2}.", new object[]
						{
							method.Name,
							text3,
							text4
						});
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		[RequiredByNativeCode]
		internal static void OnBuildPreProcess(BuildTarget platform, string path, bool strict)
		{
			if (BuildPipelineInterfaces.buildPreprocessors != null)
			{
				foreach (IPreprocessBuild current in BuildPipelineInterfaces.buildPreprocessors)
				{
					try
					{
						current.OnPreprocessBuild(platform, path);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						if (strict)
						{
							break;
						}
					}
				}
			}
		}

		[RequiredByNativeCode]
		internal static void OnSceneProcess(Scene scene, bool strict)
		{
			if (BuildPipelineInterfaces.sceneProcessors != null)
			{
				foreach (IProcessScene current in BuildPipelineInterfaces.sceneProcessors)
				{
					try
					{
						current.OnProcessScene(scene);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						if (strict)
						{
							break;
						}
					}
				}
			}
		}

		[RequiredByNativeCode]
		internal static void OnBuildPostProcess(BuildTarget platform, string path, bool strict)
		{
			if (BuildPipelineInterfaces.buildPostprocessors != null)
			{
				foreach (IPostprocessBuild current in BuildPipelineInterfaces.buildPostprocessors)
				{
					try
					{
						current.OnPostprocessBuild(platform, path);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						if (strict)
						{
							break;
						}
					}
				}
			}
		}

		[RequiredByNativeCode]
		internal static void CleanupBuildCallbacks()
		{
			BuildPipelineInterfaces.buildPreprocessors = null;
			BuildPipelineInterfaces.buildPostprocessors = null;
			BuildPipelineInterfaces.sceneProcessors = null;
		}
	}
}
