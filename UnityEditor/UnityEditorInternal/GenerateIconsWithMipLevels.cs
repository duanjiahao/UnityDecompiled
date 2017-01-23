using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	public class GenerateIconsWithMipLevels
	{
		private class InputData
		{
			public string sourceFolder;

			public string targetFolder;

			public string mipIdentifier;

			public string mipFileExtension;

			public List<string> generatedFileNames = new List<string>();

			public string GetMipFileName(string baseName, int mipResolution)
			{
				return string.Concat(new object[]
				{
					this.sourceFolder,
					baseName,
					this.mipIdentifier,
					mipResolution,
					".",
					this.mipFileExtension
				});
			}
		}

		private static string k_IconSourceFolder = "Assets/MipLevels For Icons/";

		private static string k_IconTargetFolder = "Assets/Editor Default Resources/Icons/Processed";

		private static string k_IconMipIdentifier = "@";

		private static GenerateIconsWithMipLevels.InputData GetInputData()
		{
			return new GenerateIconsWithMipLevels.InputData
			{
				sourceFolder = GenerateIconsWithMipLevels.k_IconSourceFolder,
				targetFolder = GenerateIconsWithMipLevels.k_IconTargetFolder,
				mipIdentifier = GenerateIconsWithMipLevels.k_IconMipIdentifier,
				mipFileExtension = "png"
			};
		}

		public static void GenerateAllIconsWithMipLevels()
		{
			GenerateIconsWithMipLevels.InputData inputData = GenerateIconsWithMipLevels.GetInputData();
			GenerateIconsWithMipLevels.EnsureFolderIsCreated(inputData.targetFolder);
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			GenerateIconsWithMipLevels.GenerateIconsWithMips(inputData);
			Debug.Log(string.Format("Generated {0} icons with mip levels in {1} seconds", inputData.generatedFileNames.Count, Time.realtimeSinceStartup - realtimeSinceStartup));
			GenerateIconsWithMipLevels.RemoveUnusedFiles(inputData.generatedFileNames);
			AssetDatabase.Refresh();
			InternalEditorUtility.RepaintAllViews();
		}

		public static bool VerifyIconPath(string assetPath, bool logError)
		{
			bool result;
			if (string.IsNullOrEmpty(assetPath))
			{
				result = false;
			}
			else if (assetPath.IndexOf(GenerateIconsWithMipLevels.k_IconSourceFolder) < 0)
			{
				if (logError)
				{
					Debug.Log("Selection is not a valid mip texture, it should be located in: " + GenerateIconsWithMipLevels.k_IconSourceFolder);
				}
				result = false;
			}
			else if (assetPath.IndexOf(GenerateIconsWithMipLevels.k_IconMipIdentifier) < 0)
			{
				if (logError)
				{
					Debug.Log("Selection does not have a valid mip identifier " + assetPath + "  " + GenerateIconsWithMipLevels.k_IconMipIdentifier);
				}
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public static void GenerateSelectedIconsWithMips()
		{
			if (Selection.activeInstanceID == 0)
			{
				Debug.Log("Ensure to select a mip texture..." + Selection.activeInstanceID);
			}
			else
			{
				GenerateIconsWithMipLevels.InputData inputData = GenerateIconsWithMipLevels.GetInputData();
				int activeInstanceID = Selection.activeInstanceID;
				string assetPath = AssetDatabase.GetAssetPath(activeInstanceID);
				if (GenerateIconsWithMipLevels.VerifyIconPath(assetPath, true))
				{
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					string text = assetPath.Replace(inputData.sourceFolder, "");
					text = text.Substring(0, text.LastIndexOf(inputData.mipIdentifier));
					List<string> iconAssetPaths = GenerateIconsWithMipLevels.GetIconAssetPaths(inputData.sourceFolder, inputData.mipIdentifier, inputData.mipFileExtension);
					GenerateIconsWithMipLevels.EnsureFolderIsCreated(inputData.targetFolder);
					GenerateIconsWithMipLevels.GenerateIcon(inputData, text, iconAssetPaths, null, null);
					Debug.Log(string.Format("Generated {0} icon with mip levels in {1} seconds", text, Time.realtimeSinceStartup - realtimeSinceStartup));
					InternalEditorUtility.RepaintAllViews();
				}
			}
		}

		public static void GenerateIconWithMipLevels(string assetPath, Dictionary<int, Texture2D> mipTextures, FileInfo fileInfo)
		{
			if (GenerateIconsWithMipLevels.VerifyIconPath(assetPath, true))
			{
				GenerateIconsWithMipLevels.InputData inputData = GenerateIconsWithMipLevels.GetInputData();
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				string text = assetPath.Replace(inputData.sourceFolder, "");
				text = text.Substring(0, text.LastIndexOf(inputData.mipIdentifier));
				List<string> iconAssetPaths = GenerateIconsWithMipLevels.GetIconAssetPaths(inputData.sourceFolder, inputData.mipIdentifier, inputData.mipFileExtension);
				GenerateIconsWithMipLevels.EnsureFolderIsCreated(inputData.targetFolder);
				if (GenerateIconsWithMipLevels.GenerateIcon(inputData, text, iconAssetPaths, mipTextures, fileInfo))
				{
					Debug.Log(string.Format("Generated {0} icon with mip levels in {1} seconds", text, Time.realtimeSinceStartup - realtimeSinceStartup));
				}
				InternalEditorUtility.RepaintAllViews();
			}
		}

		public static int MipLevelForAssetPath(string assetPath, string separator)
		{
			int result;
			if (string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(separator))
			{
				result = -1;
			}
			else
			{
				int num = assetPath.IndexOf(separator);
				if (num == -1)
				{
					Debug.LogError("\"" + separator + "\" could not be found in asset path: " + assetPath);
					result = -1;
				}
				else
				{
					int num2 = num + separator.Length;
					int num3 = assetPath.IndexOf(".", num2);
					if (num3 == -1)
					{
						Debug.LogError("Could not find path extension in asset path: " + assetPath);
						result = -1;
					}
					else
					{
						result = int.Parse(assetPath.Substring(num2, num3 - num2));
					}
				}
			}
			return result;
		}

		private static void GenerateIconsWithMips(GenerateIconsWithMipLevels.InputData inputData)
		{
			List<string> iconAssetPaths = GenerateIconsWithMipLevels.GetIconAssetPaths(inputData.sourceFolder, inputData.mipIdentifier, inputData.mipFileExtension);
			if (iconAssetPaths.Count == 0)
			{
				Debug.LogWarning("No mip files found for generating icons! Searching in: " + inputData.sourceFolder + ", for files with extension: " + inputData.mipFileExtension);
			}
			string[] baseNames = GenerateIconsWithMipLevels.GetBaseNames(inputData, iconAssetPaths);
			string[] array = baseNames;
			for (int i = 0; i < array.Length; i++)
			{
				string baseName = array[i];
				GenerateIconsWithMipLevels.GenerateIcon(inputData, baseName, iconAssetPaths, null, null);
			}
		}

		private static bool GenerateIcon(GenerateIconsWithMipLevels.InputData inputData, string baseName, List<string> assetPathsOfAllIcons, Dictionary<int, Texture2D> mipTextures, FileInfo sourceFileInfo)
		{
			string text = inputData.targetFolder + "/" + baseName + " Icon.asset";
			bool result;
			if (sourceFileInfo != null && File.Exists(text))
			{
				FileInfo fileInfo = new FileInfo(text);
				if (fileInfo.LastWriteTime > sourceFileInfo.LastWriteTime)
				{
					result = false;
					return result;
				}
			}
			Debug.Log("Generating MIP levels for " + text);
			GenerateIconsWithMipLevels.EnsureFolderIsCreatedRecursively(Path.GetDirectoryName(text));
			Texture2D texture2D = GenerateIconsWithMipLevels.CreateIconWithMipLevels(inputData, baseName, assetPathsOfAllIcons, mipTextures);
			if (texture2D == null)
			{
				Debug.Log("CreateIconWithMipLevels failed");
				result = false;
			}
			else
			{
				texture2D.name = baseName + " Icon.png";
				AssetDatabase.CreateAsset(texture2D, text);
				inputData.generatedFileNames.Add(text);
				result = true;
			}
			return result;
		}

		private static Texture2D CreateIconWithMipLevels(GenerateIconsWithMipLevels.InputData inputData, string baseName, List<string> assetPathsOfAllIcons, Dictionary<int, Texture2D> mipTextures)
		{
			List<string> list = assetPathsOfAllIcons.FindAll((string o) => o.IndexOf('/' + baseName + inputData.mipIdentifier) >= 0);
			List<Texture2D> list2 = new List<Texture2D>();
			foreach (string current in list)
			{
				int key = GenerateIconsWithMipLevels.MipLevelForAssetPath(current, inputData.mipIdentifier);
				Texture2D texture2D;
				if (mipTextures != null && mipTextures.ContainsKey(key))
				{
					texture2D = mipTextures[key];
				}
				else
				{
					texture2D = GenerateIconsWithMipLevels.GetTexture2D(current);
				}
				if (texture2D != null)
				{
					list2.Add(texture2D);
				}
				else
				{
					Debug.LogError("Mip not found " + current);
				}
			}
			list2.Sort(delegate(Texture2D first, Texture2D second)
			{
				int result2;
				if (first.width == second.width)
				{
					result2 = 0;
				}
				else if (first.width < second.width)
				{
					result2 = 1;
				}
				else
				{
					result2 = -1;
				}
				return result2;
			});
			int num = 99999;
			int num2 = 0;
			foreach (Texture2D current2 in list2)
			{
				int width = current2.width;
				if (width > num2)
				{
					num2 = width;
				}
				if (width < num)
				{
					num = width;
				}
			}
			Texture2D result;
			if (num2 == 0)
			{
				result = null;
			}
			else
			{
				Texture2D texture2D2 = new Texture2D(num2, num2, TextureFormat.RGBA32, true, true);
				if (GenerateIconsWithMipLevels.BlitMip(texture2D2, list2, 0))
				{
					texture2D2.Apply(true);
					int num3 = num2;
					for (int i = 1; i < texture2D2.mipmapCount; i++)
					{
						num3 /= 2;
						if (num3 < num)
						{
							break;
						}
						GenerateIconsWithMipLevels.BlitMip(texture2D2, list2, i);
					}
					texture2D2.Apply(false, true);
					result = texture2D2;
				}
				else
				{
					result = texture2D2;
				}
			}
			return result;
		}

		private static bool BlitMip(Texture2D iconWithMips, List<Texture2D> sortedTextures, int mipLevel)
		{
			bool result;
			if (mipLevel < 0 || mipLevel >= sortedTextures.Count)
			{
				Debug.LogError("Invalid mip level: " + mipLevel);
				result = false;
			}
			else
			{
				Texture2D texture2D = sortedTextures[mipLevel];
				if (texture2D)
				{
					GenerateIconsWithMipLevels.Blit(texture2D, iconWithMips, mipLevel);
					result = true;
				}
				else
				{
					Debug.LogError("No texture at mip level: " + mipLevel);
					result = false;
				}
			}
			return result;
		}

		private static Texture2D GetTexture2D(string path)
		{
			return AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
		}

		private static List<string> GetIconAssetPaths(string folderPath, string mustHaveIdentifier, string extension)
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			string text = Path.Combine(currentDirectory, folderPath);
			Uri uri = new Uri(text);
			List<string> list = new List<string>(Directory.GetFiles(text, "*." + extension, SearchOption.AllDirectories));
			list.RemoveAll((string o) => o.IndexOf(mustHaveIdentifier) < 0);
			for (int i = 0; i < list.Count; i++)
			{
				Uri uri2 = new Uri(list[i]);
				Uri uri3 = uri.MakeRelativeUri(uri2);
				list[i] = folderPath + uri3.ToString();
			}
			return list;
		}

		private static void Blit(Texture2D source, Texture2D dest, int mipLevel)
		{
			Color32[] pixels = source.GetPixels32();
			for (int i = 0; i < pixels.Length; i++)
			{
				Color32 color = pixels[i];
				if (color.a >= 3)
				{
					color.a -= 3;
				}
				pixels[i] = color;
			}
			dest.SetPixels32(pixels, mipLevel);
		}

		private static void EnsureFolderIsCreatedRecursively(string targetFolder)
		{
			if (AssetDatabase.GetMainAssetInstanceID(targetFolder) == 0)
			{
				GenerateIconsWithMipLevels.EnsureFolderIsCreatedRecursively(Path.GetDirectoryName(targetFolder));
				Debug.Log("Created target folder " + targetFolder);
				AssetDatabase.CreateFolder(Path.GetDirectoryName(targetFolder), Path.GetFileName(targetFolder));
			}
		}

		private static void EnsureFolderIsCreated(string targetFolder)
		{
			if (AssetDatabase.GetMainAssetInstanceID(targetFolder) == 0)
			{
				Debug.Log("Created target folder " + targetFolder);
				AssetDatabase.CreateFolder(Path.GetDirectoryName(targetFolder), Path.GetFileName(targetFolder));
			}
		}

		private static void DeleteFile(string file)
		{
			if (AssetDatabase.GetMainAssetInstanceID(file) != 0)
			{
				Debug.Log("Deleted unused file: " + file);
				AssetDatabase.DeleteAsset(file);
			}
		}

		private static void RemoveUnusedFiles(List<string> generatedFiles)
		{
			for (int i = 0; i < generatedFiles.Count; i++)
			{
				string text = generatedFiles[i].Replace("Icons/Processed", "Icons");
				text = text.Replace(".asset", ".png");
				GenerateIconsWithMipLevels.DeleteFile(text);
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				if (!fileNameWithoutExtension.StartsWith("d_"))
				{
					text = text.Replace(fileNameWithoutExtension, "d_" + fileNameWithoutExtension);
					GenerateIconsWithMipLevels.DeleteFile(text);
				}
			}
			AssetDatabase.Refresh();
		}

		private static string[] GetBaseNames(GenerateIconsWithMipLevels.InputData inputData, List<string> files)
		{
			string[] array = new string[files.Count];
			int length = inputData.sourceFolder.Length;
			for (int i = 0; i < files.Count; i++)
			{
				array[i] = files[i].Substring(length, files[i].IndexOf(inputData.mipIdentifier) - length);
			}
			HashSet<string> hashSet = new HashSet<string>(array);
			array = new string[hashSet.Count];
			hashSet.CopyTo(array);
			return array;
		}
	}
}
