using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

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
			internal ArrayList m_ImportProcessors;
		}

		private static ArrayList m_PostprocessStack;

		private static ArrayList m_ImportProcessors;

		private static void LogPostProcessorMissingDefaultConstructor(Type type)
		{
			Debug.LogErrorFormat("{0} requires a default constructor to be used as an asset post processor", new object[]
			{
				type
			});
		}

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

		private static void PreprocessAssembly(string pathName)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPreprocessAssembly", new string[]
				{
					pathName
				});
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

		private static void InitPostprocessors(string pathName)
		{
			AssetPostprocessingInternal.m_ImportProcessors = new ArrayList();
			foreach (Type current in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
			{
				try
				{
					AssetPostprocessor assetPostprocessor = Activator.CreateInstance(current) as AssetPostprocessor;
					assetPostprocessor.assetPath = pathName;
					AssetPostprocessingInternal.m_ImportProcessors.Add(assetPostprocessor);
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
			AssetPostprocessingInternal.m_ImportProcessors.Sort(new AssetPostprocessingInternal.CompareAssetImportPriority());
			AssetPostprocessingInternal.PostprocessStack postprocessStack = new AssetPostprocessingInternal.PostprocessStack();
			postprocessStack.m_ImportProcessors = AssetPostprocessingInternal.m_ImportProcessors;
			if (AssetPostprocessingInternal.m_PostprocessStack == null)
			{
				AssetPostprocessingInternal.m_PostprocessStack = new ArrayList();
			}
			AssetPostprocessingInternal.m_PostprocessStack.Add(postprocessStack);
		}

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

		private static void PreprocessMesh(string pathName)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPreprocessModel", null);
			}
		}

		private static void PreprocessSpeedTree(string pathName)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPreprocessSpeedTree", null);
			}
		}

		private static void PreprocessAnimation(string pathName)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPreprocessAnimation", null);
			}
		}

		private static Material ProcessMeshAssignMaterial(Renderer renderer, Material material)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				object[] args = new object[]
				{
					material,
					renderer
				};
				object obj = AttributeHelper.InvokeMemberIfAvailable(target, "OnAssignMaterialModel", args);
				if (obj as Material)
				{
					return obj as Material;
				}
			}
			return null;
		}

		private static bool ProcessMeshHasAssignMaterial()
		{
			foreach (AssetPostprocessor assetPostprocessor in AssetPostprocessingInternal.m_ImportProcessors)
			{
				if (assetPostprocessor.GetType().GetMethod("OnAssignMaterialModel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null)
				{
					return true;
				}
			}
			return false;
		}

		private static void PostprocessMesh(GameObject gameObject)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				object[] args = new object[]
				{
					gameObject
				};
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessModel", args);
			}
		}

		private static void PostprocessSpeedTree(GameObject gameObject)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				object[] args = new object[]
				{
					gameObject
				};
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessSpeedTree", args);
			}
		}

		private static void PostprocessGameObjectWithUserProperties(GameObject go, string[] prop_names, object[] prop_values)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				object[] args = new object[]
				{
					go,
					prop_names,
					prop_values
				};
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessGameObjectWithUserProperties", args);
			}
		}

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

		private static void PreprocessTexture(string pathName)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPreprocessTexture", null);
			}
		}

		private static void PostprocessTexture(Texture2D tex, string pathName)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				object[] args = new object[]
				{
					tex
				};
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessTexture", args);
			}
		}

		private static void PostprocessSprites(Texture2D tex, string pathName, Sprite[] sprites)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				object[] args = new object[]
				{
					tex,
					sprites
				};
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessSprites", args);
			}
		}

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

		private static void PreprocessAudio(string pathName)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPreprocessAudio", null);
			}
		}

		private static void PostprocessAudio(AudioClip tex, string pathName)
		{
			foreach (AssetPostprocessor target in AssetPostprocessingInternal.m_ImportProcessors)
			{
				object[] args = new object[]
				{
					tex
				};
				AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessAudio", args);
			}
		}

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
