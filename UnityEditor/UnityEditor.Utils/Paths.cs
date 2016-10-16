using System;
using System.Collections.Generic;
using System.IO;

namespace UnityEditor.Utils
{
	internal static class Paths
	{
		public static string Combine(params string[] components)
		{
			if (components.Length < 1)
			{
				throw new ArgumentException("At least one component must be provided!");
			}
			string text = components[0];
			for (int i = 1; i < components.Length; i++)
			{
				text = Path.Combine(text, components[i]);
			}
			return text;
		}

		public static string[] Split(string path)
		{
			List<string> list = new List<string>(path.Split(new char[]
			{
				Path.DirectorySeparatorChar
			}));
			int i = 0;
			while (i < list.Count)
			{
				list[i] = list[i].Trim();
				if (list[i].Equals(string.Empty))
				{
					list.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			return list.ToArray();
		}

		public static string GetFileOrFolderName(string path)
		{
			string result;
			if (File.Exists(path))
			{
				result = Path.GetFileName(path);
			}
			else
			{
				if (!Directory.Exists(path))
				{
					throw new ArgumentException("Target '" + path + "' does not exist.");
				}
				string[] array = Paths.Split(path);
				result = array[array.Length - 1];
			}
			return result;
		}

		public static string CreateTempDirectory()
		{
			string tempFileName = Path.GetTempFileName();
			File.Delete(tempFileName);
			Directory.CreateDirectory(tempFileName);
			return tempFileName;
		}

		public static string NormalizePath(this string path)
		{
			if (Path.DirectorySeparatorChar == '\\')
			{
				return path.Replace('/', Path.DirectorySeparatorChar);
			}
			return path.Replace('\\', Path.DirectorySeparatorChar);
		}

		public static bool AreEqual(string pathA, string pathB, bool ignoreCase)
		{
			return (pathA == string.Empty && pathB == string.Empty) || (!string.IsNullOrEmpty(pathA) && !string.IsNullOrEmpty(pathB) && string.Compare(Path.GetFullPath(pathA), Path.GetFullPath(pathB), (!ignoreCase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == 0);
		}
	}
}
