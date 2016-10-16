using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

internal class GenerateIconsWithMipLevels
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

	private static string k_IconTargetFolder = "Assets/Editor Default Resources/Icons/Generated";

	public static void DeleteGeneratedFolder()
	{
		GenerateIconsWithMipLevels.InputData inputData = GenerateIconsWithMipLevels.GetInputData();
		if (AssetDatabase.GetMainAssetInstanceID(inputData.targetFolder) != 0)
		{
			AssetDatabase.DeleteAsset(inputData.targetFolder);
			AssetDatabase.Refresh();
		}
	}

	private static GenerateIconsWithMipLevels.InputData GetInputData()
	{
		return new GenerateIconsWithMipLevels.InputData
		{
			sourceFolder = GenerateIconsWithMipLevels.k_IconSourceFolder,
			targetFolder = GenerateIconsWithMipLevels.k_IconTargetFolder,
			mipIdentifier = "@",
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

	public static void GenerateSelectedIconsWithMips()
	{
		if (Selection.activeInstanceID == 0)
		{
			Debug.Log("Ensure to select a mip texture..." + Selection.activeInstanceID);
			return;
		}
		GenerateIconsWithMipLevels.InputData inputData = GenerateIconsWithMipLevels.GetInputData();
		int activeInstanceID = Selection.activeInstanceID;
		string assetPath = AssetDatabase.GetAssetPath(activeInstanceID);
		if (assetPath.IndexOf(inputData.sourceFolder) < 0)
		{
			Debug.Log("Selection is not a valid mip texture, it should be located in: " + inputData.sourceFolder);
			return;
		}
		if (assetPath.IndexOf(inputData.mipIdentifier) < 0)
		{
			Debug.Log("Selection does not have a valid mip identifier " + assetPath + "  " + inputData.mipIdentifier);
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		string text = assetPath.Replace(inputData.sourceFolder, string.Empty);
		text = text.Substring(0, text.LastIndexOf(inputData.mipIdentifier));
		List<string> iconAssetPaths = GenerateIconsWithMipLevels.GetIconAssetPaths(inputData.sourceFolder, inputData.mipIdentifier, inputData.mipFileExtension);
		GenerateIconsWithMipLevels.EnsureFolderIsCreated(inputData.targetFolder);
		GenerateIconsWithMipLevels.GenerateIcon(inputData, text, iconAssetPaths);
		Debug.Log(string.Format("Generated {0} icon with mip levels in {1} seconds", text, Time.realtimeSinceStartup - realtimeSinceStartup));
		InternalEditorUtility.RepaintAllViews();
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
			GenerateIconsWithMipLevels.GenerateIcon(inputData, baseName, iconAssetPaths);
		}
	}

	private static void GenerateIcon(GenerateIconsWithMipLevels.InputData inputData, string baseName, List<string> assetPathsOfAllIcons)
	{
		string text = inputData.targetFolder + "/" + baseName + " Icon.asset";
		GenerateIconsWithMipLevels.EnsureFolderIsCreatedRecursively(Path.GetDirectoryName(text));
		Texture2D texture2D = GenerateIconsWithMipLevels.CreateIconWithMipLevels(inputData, baseName, assetPathsOfAllIcons);
		if (texture2D == null)
		{
			Debug.Log("CreateIconWithMipLevels failed");
			return;
		}
		texture2D.name = baseName + " Icon.png";
		AssetDatabase.CreateAsset(texture2D, text);
		inputData.generatedFileNames.Add(text);
	}

	private static Texture2D CreateIconWithMipLevels(GenerateIconsWithMipLevels.InputData inputData, string baseName, List<string> assetPathsOfAllIcons)
	{
		List<string> list = assetPathsOfAllIcons.FindAll((string o) => o.IndexOf('/' + baseName + inputData.mipIdentifier) >= 0);
		List<Texture2D> list2 = new List<Texture2D>();
		foreach (string current in list)
		{
			Texture2D texture2D = GenerateIconsWithMipLevels.GetTexture2D(current);
			if (texture2D != null)
			{
				list2.Add(texture2D);
			}
			else
			{
				Debug.LogError("Mip not found " + current);
			}
		}
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
		if (num2 == 0)
		{
			return null;
		}
		Texture2D texture2D2 = new Texture2D(num2, num2, TextureFormat.ARGB32, true, true);
		if (GenerateIconsWithMipLevels.BlitMip(texture2D2, inputData.GetMipFileName(baseName, num2), 0))
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
				GenerateIconsWithMipLevels.BlitMip(texture2D2, inputData.GetMipFileName(baseName, num3), i);
			}
			texture2D2.Apply(false, true);
			return texture2D2;
		}
		return texture2D2;
	}

	private static bool BlitMip(Texture2D iconWithMips, string mipFile, int mipLevel)
	{
		Texture2D texture2D = GenerateIconsWithMipLevels.GetTexture2D(mipFile);
		if (texture2D)
		{
			GenerateIconsWithMipLevels.Blit(texture2D, iconWithMips, mipLevel);
			return true;
		}
		Debug.Log("Mip file NOT found: " + mipFile);
		return false;
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
			string text = generatedFiles[i].Replace("Icons/Generated", "Icons");
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
