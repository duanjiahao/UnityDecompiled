using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal class AssetPostprocessingInternal
	{
		internal class CompareAssetImportPriority : IComparer
		{
			int IComparer.Compare(object xo, object yo)
			{
				int postprocessOrder = ((AssetPostprocessor)xo).GetPostprocessOrder();
				int postprocessOrder2 = ((AssetPostprocessor)yo).GetPostprocessOrder();
				return postprocessOrder.CompareTo(postprocessOrder2);
			}
		}

		internal class PostprocessStack
		{
			internal ArrayList m_ImportProcessors = null;
		}

		private static ArrayList m_PostprocessStack = null;

		private static ArrayList m_ImportProcessors = null;

		private static ArrayList m_PostprocessorClasses = null;

		private static void LogPostProcessorMissingDefaultConstructor(Type type)
		{
			Debug.LogErrorFormat("{0} requires a default constructor to be used as an asset post processor", new object[]
			{
				type
			});
		}

		[RequiredByNativeCode]
		private static void PostprocessAllAssets(string[] importedAssets, string[] addedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPathAssets)
		{
			object[] parameters = new object[]
			{
				importedAssets,
				deletedAssets,
				movedAssets,
				movedFromPathAssets
			};
			foreach (Type current in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
			{
				MethodInfo method = current.GetMethod("OnPostprocessAllAssets", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					method.Invoke(null, parameters);
				}
			}
			SyncVS.PostprocessSyncProject(importedAssets, addedAssets, deletedAssets, movedAssets, movedFromPathAssets);
		}

		[RequiredByNativeCode]
		private static void PreprocessAssembly(string pathName)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					AttributeHelper.InvokeMemberIfAvailable(target, "OnPreprocessAssembly", new string[]
					{
						pathName
					});
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		internal static void CallOnGeneratedCSProjectFiles()
		{
			object[] parameters = new object[0];
			foreach (MethodInfo current in AssetPostprocessingInternal.AllPostProcessorMethodsNamed("OnGeneratedCSProjectFiles"))
			{
				current.Invoke(null, parameters);
			}
		}

		internal static bool OnPreGeneratingCSProjectFiles()
		{
			object[] parameters = new object[0];
			bool flag = false;
			foreach (MethodInfo current in AssetPostprocessingInternal.AllPostProcessorMethodsNamed("OnPreGeneratingCSProjectFiles"))
			{
				object obj = current.Invoke(null, parameters);
				if (current.ReturnType == typeof(bool))
				{
					flag |= (bool)obj;
				}
			}
			return flag;
		}

		private static IEnumerable<MethodInfo> AllPostProcessorMethodsNamed(string callbackName)
		{
			return from assetPostprocessorClass in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor))
			select assetPostprocessorClass.GetMethod(callbackName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) into method
			where method != null
			select method;
		}

		private static ArrayList GetCachedAssetPostprocessorClasses()
		{
			if (AssetPostprocessingInternal.m_PostprocessorClasses == null)
			{
				AssetPostprocessingInternal.m_PostprocessorClasses = new ArrayList();
				foreach (Type current in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
				{
					AssetPostprocessingInternal.m_PostprocessorClasses.Add(current);
				}
			}
			return AssetPostprocessingInternal.m_PostprocessorClasses;
		}

		[RequiredByNativeCode]
		private static void InitPostprocessors(string pathName)
		{
			AssetPostprocessingInternal.m_ImportProcessors = new ArrayList();
			IEnumerator enumerator = AssetPostprocessingInternal.GetCachedAssetPostprocessorClasses().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Type type = (Type)enumerator.Current;
					try
					{
						AssetPostprocessor assetPostprocessor = Activator.CreateInstance(type) as AssetPostprocessor;
						assetPostprocessor.assetPath = pathName;
						AssetPostprocessingInternal.m_ImportProcessors.Add(assetPostprocessor);
					}
					catch (MissingMethodException)
					{
						AssetPostprocessingInternal.LogPostProcessorMissingDefaultConstructor(type);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			AssetPostprocessingInternal.m_ImportProcessors.Sort(new AssetPostprocessingInternal.CompareAssetImportPriority());
			AssetPostprocessingInternal.PostprocessStack postprocessStack = new AssetPostprocessingInternal.PostprocessStack();
			postprocessStack.m_ImportProcessors = AssetPostprocessingInternal.m_ImportProcessors;
			if (AssetPostprocessingInternal.m_PostprocessStack == null)
			{
				AssetPostprocessingInternal.m_PostprocessStack = new ArrayList();
			}
			AssetPostprocessingInternal.m_PostprocessStack.Add(postprocessStack);
		}

		[RequiredByNativeCode]
		private static void CleanupPostprocessors()
		{
			if (AssetPostprocessingInternal.m_PostprocessStack != null)
			{
				AssetPostprocessingInternal.m_PostprocessStack.RemoveAt(AssetPostprocessingInternal.m_PostprocessStack.Count - 1);
				if (AssetPostprocessingInternal.m_PostprocessStack.Count != 0)
				{
					AssetPostprocessingInternal.PostprocessStack postprocessStack = (AssetPostprocessingInternal.PostprocessStack)AssetPostprocessingInternal.m_PostprocessStack[AssetPostprocessingInternal.m_PostprocessStack.Count - 1];
					AssetPostprocessingInternal.m_ImportProcessors = postprocessStack.m_ImportProcessors;
				}
			}
		}

		[RequiredByNativeCode]
		private static uint[] GetMeshProcessorVersions()
		{
			List<uint> list = new List<uint>();
			foreach (Type current in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
			{
				try
				{
					AssetPostprocessor assetPostprocessor = Activator.CreateInstance(current) as AssetPostprocessor;
					Type type = assetPostprocessor.GetType();
					bool flag = type.GetMethod("OnPreprocessModel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
					bool flag2 = type.GetMethod("OnProcessMeshAssingModel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
					bool flag3 = type.GetMethod("OnPostprocessModel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
					uint version = assetPostprocessor.GetVersion();
					if (version != 0u && (flag || flag2 || flag3))
					{
						list.Add(version);
					}
				}
				catch (MissingMethodException)
				{
					AssetPostprocessingInternal.LogPostProcessorMissingDefaultConstructor(current);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			return list.ToArray();
		}

		[RequiredByNativeCode]
		private static void PreprocessMesh(string pathName)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					AttributeHelper.InvokeMemberIfAvailable(target, "OnPreprocessModel", null);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		[RequiredByNativeCode]
		private static void PreprocessSpeedTree(string pathName)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					AttributeHelper.InvokeMemberIfAvailable(target, "OnPreprocessSpeedTree", null);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		[RequiredByNativeCode]
		private static void PreprocessAnimation(string pathName)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					AttributeHelper.InvokeMemberIfAvailable(target, "OnPreprocessAnimation", null);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		[RequiredByNativeCode]
		private static Material ProcessMeshAssignMaterial(Renderer renderer, Material material)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			Material result;
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					object[] args = new object[]
					{
						material,
						renderer
					};
					object obj = AttributeHelper.InvokeMemberIfAvailable(target, "OnAssignMaterialModel", args);
					if (obj as Material)
					{
						result = (obj as Material);
						return result;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			result = null;
			return result;
		}

		[RequiredByNativeCode]
		private static bool ProcessMeshHasAssignMaterial()
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			bool result;
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor assetPostprocessor = (AssetPostprocessor)enumerator.Current;
					if (assetPostprocessor.GetType().GetMethod("OnAssignMaterialModel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null)
					{
						result = true;
						return result;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			result = false;
			return result;
		}

		private static void PostprocessMesh(GameObject gameObject)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					object[] args = new object[]
					{
						gameObject
					};
					AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessModel", args);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		private static void PostprocessSpeedTree(GameObject gameObject)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					object[] args = new object[]
					{
						gameObject
					};
					AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessSpeedTree", args);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		[RequiredByNativeCode]
		private static void PostprocessGameObjectWithUserProperties(GameObject go, string[] prop_names, object[] prop_values)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					object[] args = new object[]
					{
						go,
						prop_names,
						prop_values
					};
					AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessGameObjectWithUserProperties", args);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		[RequiredByNativeCode]
		private static uint[] GetTextureProcessorVersions()
		{
			List<uint> list = new List<uint>();
			foreach (Type current in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
			{
				try
				{
					AssetPostprocessor assetPostprocessor = Activator.CreateInstance(current) as AssetPostprocessor;
					Type type = assetPostprocessor.GetType();
					bool flag = type.GetMethod("OnPreprocessTexture", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
					bool flag2 = type.GetMethod("OnPostprocessTexture", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
					uint version = assetPostprocessor.GetVersion();
					if (version != 0u && (flag || flag2))
					{
						list.Add(version);
					}
				}
				catch (MissingMethodException)
				{
					AssetPostprocessingInternal.LogPostProcessorMissingDefaultConstructor(current);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			return list.ToArray();
		}

		[RequiredByNativeCode]
		private static void PreprocessTexture(string pathName)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					AttributeHelper.InvokeMemberIfAvailable(target, "OnPreprocessTexture", null);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		[RequiredByNativeCode]
		private static void PostprocessTexture(Texture2D tex, string pathName)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					object[] args = new object[]
					{
						tex
					};
					AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessTexture", args);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		[RequiredByNativeCode]
		private static void PostprocessSprites(Texture2D tex, string pathName, Sprite[] sprites)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					object[] args = new object[]
					{
						tex,
						sprites
					};
					AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessSprites", args);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		[RequiredByNativeCode]
		private static uint[] GetAudioProcessorVersions()
		{
			List<uint> list = new List<uint>();
			foreach (Type current in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
			{
				try
				{
					AssetPostprocessor assetPostprocessor = Activator.CreateInstance(current) as AssetPostprocessor;
					Type type = assetPostprocessor.GetType();
					bool flag = type.GetMethod("OnPreprocessAudio", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
					bool flag2 = type.GetMethod("OnPostprocessAudio", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
					uint version = assetPostprocessor.GetVersion();
					if (version != 0u && (flag || flag2))
					{
						list.Add(version);
					}
				}
				catch (MissingMethodException)
				{
					AssetPostprocessingInternal.LogPostProcessorMissingDefaultConstructor(current);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			return list.ToArray();
		}

		[RequiredByNativeCode]
		private static void PreprocessAudio(string pathName)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					AttributeHelper.InvokeMemberIfAvailable(target, "OnPreprocessAudio", null);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		[RequiredByNativeCode]
		private static void PostprocessAudio(AudioClip tex, string pathName)
		{
			IEnumerator enumerator = AssetPostprocessingInternal.m_ImportProcessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssetPostprocessor target = (AssetPostprocessor)enumerator.Current;
					object[] args = new object[]
					{
						tex
					};
					AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessAudio", args);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		[RequiredByNativeCode]
		private static void PostprocessAssetbundleNameChanged(string assetPAth, string prevoiusAssetBundleName, string newAssetBundleName)
		{
			object[] args = new object[]
			{
				assetPAth,
				prevoiusAssetBundleName,
				newAssetBundleName
			};
			foreach (Type current in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
			{
				AssetPostprocessor target = Activator.CreateInstance(current) as AssetPostprocessor;
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessAssetbundleNameChanged", args);
			}
		}
	}
}
