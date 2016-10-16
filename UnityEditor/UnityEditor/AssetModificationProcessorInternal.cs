using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetModificationProcessorInternal
	{
		private enum FileMode
		{
			Binary,
			Text
		}

		private static IEnumerable<Type> assetModificationProcessors;

		internal static MethodInfo[] isOpenForEditMethods;

		private static IEnumerable<Type> AssetModificationProcessors
		{
			get
			{
				if (AssetModificationProcessorInternal.assetModificationProcessors == null)
				{
					List<Type> list = new List<Type>();
					list.AddRange(EditorAssemblies.SubclassesOf(typeof(AssetModificationProcessor)));
					list.AddRange(EditorAssemblies.SubclassesOf(typeof(global::AssetModificationProcessor)));
					AssetModificationProcessorInternal.assetModificationProcessors = list.ToArray();
				}
				return AssetModificationProcessorInternal.assetModificationProcessors;
			}
		}

		private static bool CheckArgumentTypes(Type[] types, MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			if (types.Length != parameters.Length)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Parameter count did not match. Expected: ",
					types.Length.ToString(),
					" Got: ",
					parameters.Length.ToString(),
					" in ",
					method.DeclaringType.ToString(),
					".",
					method.Name
				}));
				return false;
			}
			int num = 0;
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				ParameterInfo parameterInfo = parameters[num];
				if (type != parameterInfo.ParameterType)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"Parameter type mismatch at parameter ",
						num,
						". Expected: ",
						type.ToString(),
						" Got: ",
						parameterInfo.ParameterType.ToString(),
						" in ",
						method.DeclaringType.ToString(),
						".",
						method.Name
					}));
					return false;
				}
				num++;
			}
			return true;
		}

		private static bool CheckArgumentTypesAndReturnType(Type[] types, MethodInfo method, Type returnType)
		{
			if (returnType != method.ReturnType)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Return type mismatch. Expected: ",
					returnType.ToString(),
					" Got: ",
					method.ReturnType.ToString(),
					" in ",
					method.DeclaringType.ToString(),
					".",
					method.Name
				}));
				return false;
			}
			return AssetModificationProcessorInternal.CheckArgumentTypes(types, method);
		}

		private static bool CheckArguments(object[] args, MethodInfo method)
		{
			Type[] array = new Type[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				array[i] = args[i].GetType();
			}
			return AssetModificationProcessorInternal.CheckArgumentTypes(array, method);
		}

		private static bool CheckArgumentsAndReturnType(object[] args, MethodInfo method, Type returnType)
		{
			Type[] array = new Type[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				array[i] = args[i].GetType();
			}
			return AssetModificationProcessorInternal.CheckArgumentTypesAndReturnType(array, method, returnType);
		}

		private static void OnWillCreateAsset(string path)
		{
			foreach (Type current in AssetModificationProcessorInternal.AssetModificationProcessors)
			{
				MethodInfo method = current.GetMethod("OnWillCreateAsset", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					object[] array = new object[]
					{
						path
					};
					if (AssetModificationProcessorInternal.CheckArguments(array, method))
					{
						method.Invoke(null, array);
					}
				}
			}
		}

		private static void FileModeChanged(string[] assets, UnityEditor.VersionControl.FileMode mode)
		{
			if (Provider.enabled && Provider.PromptAndCheckoutIfNeeded(assets, string.Empty))
			{
				Provider.SetFileMode(assets, mode);
			}
		}

		private static void OnWillSaveAssets(string[] assets, out string[] assetsThatShouldBeSaved, out string[] assetsThatShouldBeReverted, int explicitlySaveScene)
		{
			assetsThatShouldBeReverted = new string[0];
			assetsThatShouldBeSaved = assets;
			bool flag = assets.Length > 0 && EditorPrefs.GetBool("VerifySavingAssets", false) && InternalEditorUtility.isHumanControllingUs;
			if (explicitlySaveScene != 0 && assets.Length == 1 && assets[0].EndsWith(".unity"))
			{
				flag = false;
			}
			if (flag)
			{
				AssetSaveDialog.ShowWindow(assets, out assetsThatShouldBeSaved);
			}
			else
			{
				assetsThatShouldBeSaved = assets;
			}
			foreach (Type current in AssetModificationProcessorInternal.AssetModificationProcessors)
			{
				MethodInfo method = current.GetMethod("OnWillSaveAssets", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					object[] array = new object[]
					{
						assetsThatShouldBeSaved
					};
					if (AssetModificationProcessorInternal.CheckArguments(array, method))
					{
						string[] array2 = (string[])method.Invoke(null, array);
						if (array2 != null)
						{
							assetsThatShouldBeSaved = array2;
						}
					}
				}
			}
			if (assetsThatShouldBeSaved == null)
			{
				return;
			}
			List<string> list = new List<string>();
			string[] array3 = assetsThatShouldBeSaved;
			for (int i = 0; i < array3.Length; i++)
			{
				string text = array3[i];
				if (!AssetDatabase.IsOpenForEdit(text))
				{
					list.Add(text);
				}
			}
			assets = list.ToArray();
			if (assets.Length != 0 && !Provider.PromptAndCheckoutIfNeeded(assets, string.Empty))
			{
				Debug.LogError("Could not check out the following files in version control before saving: " + string.Join(", ", assets));
				assetsThatShouldBeSaved = new string[0];
				return;
			}
		}

		private static void RequireTeamLicense()
		{
			if (!InternalEditorUtility.HasTeamLicense())
			{
				throw new MethodAccessException("Requires team license");
			}
		}

		private static AssetMoveResult OnWillMoveAsset(string fromPath, string toPath, string[] newPaths, string[] NewMetaPaths)
		{
			AssetMoveResult assetMoveResult = AssetMoveResult.DidNotMove;
			if (!InternalEditorUtility.HasTeamLicense())
			{
				return assetMoveResult;
			}
			assetMoveResult = AssetModificationHook.OnWillMoveAsset(fromPath, toPath);
			foreach (Type current in AssetModificationProcessorInternal.AssetModificationProcessors)
			{
				MethodInfo method = current.GetMethod("OnWillMoveAsset", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					AssetModificationProcessorInternal.RequireTeamLicense();
					object[] array = new object[]
					{
						fromPath,
						toPath
					};
					if (AssetModificationProcessorInternal.CheckArgumentsAndReturnType(array, method, assetMoveResult.GetType()))
					{
						assetMoveResult |= (AssetMoveResult)((int)method.Invoke(null, array));
					}
				}
			}
			return assetMoveResult;
		}

		private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
		{
			AssetDeleteResult assetDeleteResult = AssetDeleteResult.DidNotDelete;
			if (!InternalEditorUtility.HasTeamLicense())
			{
				return assetDeleteResult;
			}
			foreach (Type current in AssetModificationProcessorInternal.AssetModificationProcessors)
			{
				MethodInfo method = current.GetMethod("OnWillDeleteAsset", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					AssetModificationProcessorInternal.RequireTeamLicense();
					object[] array = new object[]
					{
						assetPath,
						options
					};
					if (AssetModificationProcessorInternal.CheckArgumentsAndReturnType(array, method, assetDeleteResult.GetType()))
					{
						assetDeleteResult |= (AssetDeleteResult)((int)method.Invoke(null, array));
					}
				}
			}
			if (assetDeleteResult != AssetDeleteResult.DidNotDelete)
			{
				return assetDeleteResult;
			}
			assetDeleteResult = AssetModificationHook.OnWillDeleteAsset(assetPath, options);
			return assetDeleteResult;
		}

		internal static MethodInfo[] GetIsOpenForEditMethods()
		{
			if (AssetModificationProcessorInternal.isOpenForEditMethods == null)
			{
				List<MethodInfo> list = new List<MethodInfo>();
				foreach (Type current in AssetModificationProcessorInternal.AssetModificationProcessors)
				{
					MethodInfo method = current.GetMethod("IsOpenForEdit", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					if (method != null)
					{
						AssetModificationProcessorInternal.RequireTeamLicense();
						string empty = string.Empty;
						bool flag = false;
						Type[] types = new Type[]
						{
							empty.GetType(),
							empty.GetType().MakeByRefType()
						};
						if (AssetModificationProcessorInternal.CheckArgumentTypesAndReturnType(types, method, flag.GetType()))
						{
							list.Add(method);
						}
					}
				}
				AssetModificationProcessorInternal.isOpenForEditMethods = list.ToArray();
			}
			return AssetModificationProcessorInternal.isOpenForEditMethods;
		}

		internal static bool IsOpenForEdit(string assetPath, out string message)
		{
			message = string.Empty;
			if (string.IsNullOrEmpty(assetPath))
			{
				return true;
			}
			bool result = AssetModificationHook.IsOpenForEdit(assetPath, out message);
			MethodInfo[] array = AssetModificationProcessorInternal.GetIsOpenForEditMethods();
			for (int i = 0; i < array.Length; i++)
			{
				MethodInfo methodInfo = array[i];
				object[] array2 = new object[]
				{
					assetPath,
					message
				};
				if (!(bool)methodInfo.Invoke(null, array2))
				{
					message = (array2[1] as string);
					return false;
				}
			}
			return result;
		}

		internal static void OnStatusUpdated()
		{
			WindowPending.OnStatusUpdated();
			foreach (Type current in AssetModificationProcessorInternal.AssetModificationProcessors)
			{
				MethodInfo method = current.GetMethod("OnStatusUpdated", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					AssetModificationProcessorInternal.RequireTeamLicense();
					object[] array = new object[0];
					if (AssetModificationProcessorInternal.CheckArgumentsAndReturnType(array, method, typeof(void)))
					{
						method.Invoke(null, array);
					}
				}
			}
		}
	}
}
